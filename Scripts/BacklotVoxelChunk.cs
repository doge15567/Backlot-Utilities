using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EvroDev.BacklotUtilities.Voxels
{
    public class BacklotVoxelChunk : MonoBehaviour
    {
        public Vector3Int Dimensions;
        public Voxel[,,] voxels;

        private Transform tempGizmosParent;
        private List<SelectableFace> gizmoFaces;

        Voxel SafeSampleVoxel(int x, int y, int z)
        {
            if(x < 0 || x >= Dimensions.x)
            {
                return null;
            }
            if(y < 0 || y >= Dimensions.y)
            {
                return null;
            }
            if(z < 0 || z >= Dimensions.z)
            {
                return null;
            }

            return voxels[x, y, z];
        }

        void SetVoxel()
        {

        }

        [ContextMenu("Regen Full")]
        public void Regen()
        {
            voxels = new Voxel[Dimensions.x, Dimensions.y, Dimensions.z];
            for (int x = 0; x < Dimensions.x; x++)
            {
                for (int y = 0; y < Dimensions.x; y++)
                {
                    for (int z = 0; z < Dimensions.x; z++)
                    {
                        voxels[x, y, z] = new Voxel();
                    }
                }
            }
            RegenGizmo();
        }

        void RegenGizmo()
        {
            if(tempGizmosParent == null)
            {
                GameObject gizmoParent = new GameObject("TEMP Gizmo Parent");
                gizmoParent.transform.parent = transform;
                gizmoParent.transform.localPosition = Vector3.zero;
                tempGizmosParent = gizmoParent.transform;
            }
            else
            {
                for (int c = tempGizmosParent.childCount - 1; c >= 0; c--)
                {
                    DestroyImmediate(tempGizmosParent.GetChild(c).gameObject);
                }
                gizmoFaces.Clear();
            }

            for(int x = 0; x < Dimensions.x; x++)
            {
                for (int y = 0; y < Dimensions.y; y++)
                {
                    for (int z = 0; z < Dimensions.z; z++)
                    {
                        if(voxels[x,y,z] == null) continue;

                        if(SafeSampleVoxel(x,y+1,z) == null) // if up is air,.,,,
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Up, voxels[x,y,z]);
                        }
                        if(SafeSampleVoxel(x,y-1,z) == null)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Down, voxels[x,y,z]);
                        }
                        if(SafeSampleVoxel(x,y,z+1) == null)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Forward, voxels[x,y,z]);
                        }
                        if(SafeSampleVoxel(x,y,z-1) == null)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Backward, voxels[x,y,z]);
                        }
                        if(SafeSampleVoxel(x+1,y,z) == null)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Right, voxels[x,y,z]);
                        }
                        if(SafeSampleVoxel(x-1,y,z) == null)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Left, voxels[x,y,z]);
                        }
                    }
                }
            }
        }

        void GenerateBacklots()
        {
            // doit later lmao
        }

        [ContextMenu("Extrude Selection (Debug)")]
        public void ExtrudeSelection()
        {
#if UNITY_EDITOR
            foreach (GameObject g in Selection.gameObjects)
            {
                if(g.TryGetComponent(out SelectableFace face))
                {
                    Vector3Int p = face.GetTargetAir();
                    voxels[p.x, p.y, p.z] = new Voxel();
                }
            }
            RegenGizmo();
#endif
        }
        
        [ContextMenu("Intrude Selection (Debug)")]
        public void IntrudeSelection()
        {
#if UNITY_EDITOR
            foreach (GameObject g in Selection.gameObjects)
            {
                if(g.TryGetComponent(out SelectableFace face))
                {
                    Vector3Int p = face.voxelPosition;
                    voxels[p.x, p.y, p.z] = null;
                }
            }
            RegenGizmo();
#endif
        }

        public void PaintSelection(Material m)
        {
#if UNITY_EDITOR
            foreach (GameObject g in Selection.gameObjects)
            {
                if(g.TryGetComponent(out SelectableFace face))
                {
                    Vector3Int p = face.voxelPosition;
                    Voxel voxel = voxels[p.x, p.y, p.z];

                    voxel.materials[face.FaceDirection] = m;
                }
            }
#endif
        }
    }

    public class Voxel
    {
        public Dictionary<FaceDirection, Material> materials = new Dictionary<FaceDirection, Material>();
    }
}
