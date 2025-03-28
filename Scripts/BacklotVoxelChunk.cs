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
        public int ChunkSize = 32;
        int ChunkSizeP = ChunkSize + 2;
        int ChunkSizeP2 = ChunkSizeP * ChunkSizeP;
        public Voxel[] voxels;

        private Transform tempGizmosParent;
        private List<SelectableFace> gizmoFaces;


        Voxel SafeSampleVoxel(int x, int y, int z)
        {
            if(x < 0 || x >= ChunkSize)
            {
                return null;
            }
            if(y < 0 || y >= ChunkSize)
            {
                return null;
            }
            if(z < 0 || z >= ChunkSize)
            {
                return null;
            }


            int index = x + ChunkSize * (y + ChunkSize * z);
            return voxels[index];
        }


        void SetVoxel(int x, int y, int z, Voxel voxel)
        {
            if(x < 0 || x >= ChunkSize)
            {
                return;
            }
            if(y < 0 || y >= ChunkSize)
            {
                return;
            }
            if(z < 0 || z >= ChunkSize)
            {
                return;
            }
            int index = x + ChunkSize * (y + ChunkSize * z);
            voxels[index] = voxel;
        }


        [ContextMenu("Regen Full")]
        public void Regen()
        {
            voxels = new Voxel[ChunkSize * ChunkSize * ChunkSize];
            for (int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        SetVoxel(x, y, z, new Voxel());
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


            for(int x = 0; x < ChunkSize; x++)
            {
                for (int y = 0; y < ChunkSize; y++)
                {
                    for (int z = 0; z < ChunkSize; z++)
                    {
                        Voxel thisVoxel = SafeSampleVoxel(x,y,z);
                        if(thisVoxel == null || thisVoxel.IsEmpty) continue;


                        if(SafeSampleVoxel(x,y+1,z).IsEmpty) // if up is air,.,,,
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Up, thisVoxel);
                        }
                        if(SafeSampleVoxel(x,y-1,z).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Down, thisVoxel);
                        }
                        if(SafeSampleVoxel(x,y,z+1).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Forward, thisVoxel);
                        }
                        if(SafeSampleVoxel(x,y,z-1).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Backward, thisVoxel);
                        }
                        if(SafeSampleVoxel(x+1,y,z).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Right, thisVoxel);
                        }
                        if(SafeSampleVoxel(x-1,y,z).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Left, thisVoxel);
                        }
                    }
                }
            }
        }




        void GenerateBacklots()
        {
            // Build a list of Backlot Instances, with things like position, scale, material etc
            int[] axis_cols = new int[ChunkSizeP2 * 3];
            List<int> col_face_masks = new List<int>();
            
            for(int y = 0; y < ChunkSize; y++)
            {
                for(int z = 0; z < ChunkSize; z++)
                {
                    for(int x = 0; x < ChunkSize; x++)
                    {
                        Vector3Int pos = new (x, y, z);
                        Voxel b = SafeSampleVoxel(x, y, z);
                        if(!b.IsEmpty)
                        {
                            axis_cols[x + (z * ChunkSizeP)] |= 1 << y;
                            axis_cols[z + (y * ChunkSizeP) + ChunkSizeP2] |= 1 << x;
                            axis_cols[y + (x * ChunkSizeP) + ChunkSizeP2*2] |= 1 << z;
                        }
                    }
                }
            }

            for(int axis = 0; axis < 3; axis++)
            {
                for(int i = 0; i < ChunkSizeP2; i++)
                {
                    int col = axis_cols[(ChunkSizeP2 * axis) + i];
                    col_face_masks[(ChunkSizeP2 * (axis * 2 + 1)) + i] = col & !(col >> 1);
                    col_face_masks[(ChunkSizeP2 * (axis * 2 + 0)) + i] = col & !(col << 1);
                }
            }

            for(int axis2 = 0; axis2 < 6; axis2++)
            {
                for(int z = 0; z < ChunkSize; z++)
                {
                    for(int x = 0; x < ChunkSize; x++)
                    {
                        int col_index = 1 + x + ((z+1) * ChunkSizeP) + ChunkSizeP2 * axis2;

                        int col = col_face_masks[col_index] >> 1;
                        col = col & !(1 << ChunkSize);

                        while (col != 0)
                        {
                            int y = BitOperations.TrailingZeroCount(col);

                            col &= col - 1;

                            Vector3Int voxelPos = new (x, z, y);

                            switch (axis2)
                            {
                                case(0):
                                    voxelPos = new Vector3Int(x, y, z);
                                    break;
                                case(1):
                                    voxelPos = new Vector3Int(x, y, z);
                                    break;
                                case(2):
                                    voxelPos = new Vector3Int(y, z, x);
                                    break;
                                case(3):
                                    voxelPos = new Vector3Int(y, z, x);
                                    break;
                            }

                            Voxel currentVoxel = SafeSampleVoxel(voxelPos.x, voxelPos.y, voxelPos.z);

                            // EVRONOTE continue here
                        }
                    }
                }
            }


            // Instantiate those backlots in world space, parent to this
            foreach(_)
            {

            }
        }

        int[] GetSliceFromAxis(FaceDirection direction, int index)
        {
            switch(direction)
            {
                case(FaceDirection.Up):

                    break;
            }
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
                    SetVoxel(p.x, p.y, p.z, new Voxel());
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
                    SafeSampleVoxel(p.x, p.y, p.z).IsEmpty = true;
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
                    Voxel voxel = SafeSampleVoxel(p.x, p.y, p.z);


                    voxel.SetMaterial(face.FaceDirection, m);
                }
            }
#endif
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position + (Dimensions/2), Dimensions);
        }
    }


    [System.Serializable]
    public class Voxel
    {
        public bool IsEmpty = false;
        private Material[] _materials = new Material[6];

        public void SetMaterial(FaceDirection dir, Material mat)
        {
            _materials[(int)dir] = mat;
        }


        public Material GetMaterial(FaceDirection dir)
        {
            return _materials[(int)dir];
        }
    }


    public class GreedyGridwall
    {
        public class ResultingBacklot
        {
            public Vector2 planePos;
            public Vector3 worldPos;
            public Vector2 scale;
            public Material material;
            public SurfaceDataCardReference surfaceData;
        }

        public List<ResultingBacklot> GreedTheGrid(int[] data, Material theMat)
        {
            List<ResultingBacklot> outpt = new List<ResultingBacklot>();
            for(int x = 0; x < grid.Length; x++)
            {
                int y = 0;
                while(y < 32)
                {
                    y += BitOperations.TrailingZeroCount(data[x] >> y);
                    int height = BitOperations.TrailingZeroCount(~(data[x] >> y));

                    if(height == 0) break;

                    int h_as_mask = (1 << height) - 1;
                    int mask = h_as_mask << y;
                    int w = 1;


                    while (x + w < data.length)
                    {
                        int nextRow = (data[x+w] >> y) & h_as_mask;
                        if(nextRow != h_as_mask)
                            break;

                        data[x+w] = data[x+w] & !mask;
                        w += 1;
                    }

                    Vector2 scale = new Vector2(height, w);

                    outpt.Add(new ResultingBacklot()
                    {
                        planePos = new Vector2(x, y) + (scale/2),
                        scale = scale,
                        material = theMat
                    });

                }
            }
            return outpt;
        }
    }
}
