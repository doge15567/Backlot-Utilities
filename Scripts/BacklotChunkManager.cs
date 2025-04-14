// Script for managing chunks and stuff
// Goals: Automatically create chunks when bounds are hit
//      Manage overall extending of bounds and stuff
//      some other stuff
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EvroDev.BacklotUtilities.Voxels;

public enum VisualizationMode
{
    Gizmos,
    Backlot
}

public class BacklotChunkManager : MonoBehaviour
{
    public int ChunkSize = 32;
    public List<PosToChunk> chunks;

    public VisualizationMode visualizationMode = VisualizationMode.Gizmos;

    public void GenerateChangedChunks()
    {
        foreach(var chunkHolder in chunks)
        {
            if(chunkHolder.chunk.isDirty)
            {
                chunkHolder.chunk.GenerateBacklots();
            }
        }
    }


    [ContextMenu("Set Material to Selected")]
    public void SetWithSelectedMaterial()
    {
#if UNITY_EDITOR
        foreach (Object g in Selection.objects)
        {
            if (g is Material)
            {
                PaintSelection(g as Material);
                break;
            }
        }
#endif
    }

    public void PaintSelection(Material m)
    {
#if UNITY_EDITOR
#endif
    }

    /// <summary>
    /// Check if there is a chunk at a certain chunk position
    /// </summary>
    private bool ChunkAt(Vector3Int pos, out BacklotVoxelChunk outChunk)
    {
        foreach(var chunk in chunks)
        {
            if(chunk.pos == pos)
            {
                outChunk = chunk.chunk;
                return true;
            }
        }
        outChunk = null;
        return false;
    }

    private Vector3Int GetChunkPos(BacklotVoxelChunk chunk)
    {
        foreach(var chunk in chunks)
        {
            if(chunk.chunk == chunk)
            {
                return chunk.pos;
            }
        }
        return Vector3Int.zero;
    }

    /// <summary>
    /// Gets a voxel from relative coordinates to a specific chunk
    /// Will overflow into nearby chunks if needed
    /// </summary>
    public Voxel GetVoxel(BacklotVoxelChunk chunk, Vector3Int position)
    {
        var realChunk = GetRelativeChunk(chunk, position);

        return realChunk.SafeSampleVoxel(position.x, position.y, position.z);
    }

    /// <summary>
    /// Sets a voxel from relative coordinates to a specific chunk
    /// Will overflow into nearby chunks if needed
    /// </summary>
    public void SetVoxel(BacklotVoxelChunk chunk, Vector3Int position, Voxel voxel)
    {
        var realChunk = GetRelativeChunk(chunk, position, true);

        realChunk.SafeSetVoxel(position, voxel);
    }


    private BacklotVoxelChunk GetRelativeChunk(BacklotVoxelChunk chunk, Vector3Int position, bool createIfNull = false)
    {
        Vector3Int chunkPos = GetChunkPos(chunk);
        Vector3Int originalPosition = chunkPos;

        if(position.x < 0)
        {
            chunkPos -= new Vector3Int(1, 0, 0);
            position += new Vector3Int(ChunkSize, 0, 0);
        }
        else if(position.x >= ChunkSize)
        {
            chunkPos += new Vector3Int(1, 0, 0);
            position -= new Vector3Int(ChunkSize, 0, 0);
        }
        if(position.y < 0)
        {
            chunkPos -= new Vector3Int(0, 1, 0);
            position += new Vector3Int(0, ChunkSize, 0);
        }
        else if(position.y >= ChunkSize)
        {
            chunkPos += new Vector3Int(0, 1, 0);
            position -= new Vector3Int(0, ChunkSize, 0);
        }
        if(position.z < 0)
        {
            chunkPos -= new Vector3Int(0, 0, 1);
            position += new Vector3Int(0, 0, ChunkSize);
        }
        else if(position.z >= ChunkSize)
        {
            chunkPos += new Vector3Int(0, 0, 1);
            position -= new Vector3Int(0, ChunkSize);
        }

        if(originalPosition == chunkPos)
        {
            return chunk;
        }
        else if(ChunkAt(chunkPos, out var newChunk))
        {
            return newChunk;
        }
        else if(createIfNull)
        {
            return CreateNewChunk(chunkPos);
        }
        else
        {
            // Default to this, anything should use safesample and evaluate any voxels to empty
            return chunk;
        }

    }

    private BacklotVoxelChunk CreateNewChunk(Vector3Int chunkPosition)
    {
        GameObject chunkGo = new GameObject($"Chunk ({chunkPosition.x}, {chunkPosition.y}, {chunkPosition.z})");

        chunkGo.transform.SetParent(transform);
        chunkGo.transform.localPosition = chunkPosition * chunkSize;

        BacklotVoxelChunk chunk = chunkGo.AddComponent<BacklotVoxelChunk>();

        chunk.manager = this;
        chunk.ChunkSize = ChunkSize;
        chunk.Regen();

        chunks.Add(new PosToChunk(){ 
            pos = chunkPosition, 
            chunk = chunk
        });

        return chunk;
    }

    void Reset()
    {
        var chunk = CreateNewChunk(Vector3Int.zero);
        SetVoxel(chunk, Vector3Int.zero, new Voxel()
        {
            IsEmpty = false;
        });
    }
}

[Serializable]
public struct PosToChunk
{
    public Vector3Int pos;
    public BacklotVoxelChunk chunk;
}
