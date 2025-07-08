#if UNITY_EDITOR
using EvroDev.BacklotUtilities.Extensions;
using SLZ.Marrow.Warehouse;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EvroDev.BacklotUtilities.Voxels
{
    public enum VisualizationMode
    {
        Gizmos,
        Backlot
    }

    public class BacklotChunkManager : MonoBehaviour
    {
        public int ChunkSize = 32;
        public List<PosToChunk> chunks = new List<PosToChunk>() ;
        [SerializeField]
        public StringCache surfaceDataCache = new() { {0, "SLZ.Backlot.SurfaceDataCard.Concrete" } };
        
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

        private void OnValidate()
        {
            if(visualizationMode == VisualizationMode.Gizmos)
            {
                foreach(var realChunk in chunks)
                {
                    var chunk = realChunk.chunk;
                    if (chunk.backlotsParent != null)
                        chunk.backlotsParent.gameObject.SetActive(false);
                    chunk.RegenGizmos();
                }
                // Hide Backlots
            }
            else
            {
                foreach (var realChunk in chunks)
                {
                    var chunk = realChunk.chunk;
                    if(chunk.backlotsParent != null)
                        chunk.backlotsParent.gameObject.SetActive(true);

                    EditorApplication.delayCall += () =>
                    {
                        foreach (SelectableFace f in chunk.GetComponentsInChildren<SelectableFace>())
                        {
                            DestroyImmediate(f.gameObject);
                        }
                    };
                }
                GenerateChangedChunks();
                // show backlots
            }
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
            foreach(var chunkPos in chunks)
            {
                if(chunkPos.chunk == chunk)
                {
                    return chunkPos.pos;
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
            BacklotVoxelChunk realChunk;
            Vector3Int realPos;

            if (position.InBounds(chunk.ChunkSize)) (realChunk, realPos) = (chunk, position);
            else (realChunk, realPos) = GetRelativeChunk(chunk, position);

            return realChunk.SafeSampleVoxel(realPos.x, realPos.y, realPos.z);
        }

        /// <summary>
        /// Sets a voxel from relative coordinates to a specific chunk
        /// Will overflow into nearby chunks if needed
        /// </summary>
        public void SetVoxel(BacklotVoxelChunk chunk, Vector3Int position, Voxel voxel)
        {
            BacklotVoxelChunk realChunk;
            Vector3Int realPos;

            if (position.InBounds(chunk.ChunkSize)) (realChunk, realPos) = (chunk, position);
            else (realChunk, realPos) = GetRelativeChunk(chunk, position, true);

            realChunk.SafeSetVoxel(realPos, voxel);
        }


        private (BacklotVoxelChunk, Vector3Int) GetRelativeChunk(BacklotVoxelChunk chunk, Vector3Int position, bool createIfNull = false)
        {
            Vector3Int chunkPos = GetChunkPos(chunk);
            Vector3Int originalPosition = position;

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
                position -= new Vector3Int(0, 0, ChunkSize);
            }

            if(originalPosition == position)
            {
                return (chunk, originalPosition);
            }
            else if(ChunkAt(chunkPos, out var newChunk))
            {
                return (newChunk, position);
            }
            else if(createIfNull)
            {
                return (CreateNewChunk(chunkPos), position);
            }
            else
            {
                // Default to this, anything should use safesample and evaluate any voxels to empty
                return (chunk, originalPosition);
            }

        }

        private BacklotVoxelChunk CreateNewChunk(Vector3Int chunkPosition)
        {
            GameObject chunkGo = new GameObject($"Chunk ({chunkPosition.x}, {chunkPosition.y}, {chunkPosition.z})");

            chunkGo.transform.SetParent(transform);
            chunkGo.transform.localPosition = chunkPosition * ChunkSize;

            BacklotVoxelChunk chunk = chunkGo.AddComponent<BacklotVoxelChunk>();

            chunk.manager = this;
            chunk.ChunkSize = ChunkSize;

            chunks.Add(new PosToChunk(){ 
                pos = chunkPosition, 
                chunk = chunk
            });

            chunk.Regen();

            return chunk;
        }

        void Reset()
        {
            var chunk = CreateNewChunk(Vector3Int.zero);
            SetVoxel(chunk, Vector3Int.zero, new Voxel()
            {
                IsEmpty = false,
                chunk = chunk
            });
        }
    }

    [Serializable]
    public struct PosToChunk
    {
        public Vector3Int pos;
        public BacklotVoxelChunk chunk;
    }
}
#endif