using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace EvroDev.BacklotUtilities.Voxels
{
    public class BacklotVoxelChunk : MonoBehaviour
    {
        public BacklotChunkManager manager;
        public int ChunkSize = 32;
        int ChunkSizeP => ChunkSize + 2;
        int ChunkSizeP2 => ChunkSizeP * ChunkSizeP;
        public Voxel[] voxels;

        public Transform tempGizmosParent;
        public Transform backlotsParent;
        private List<SelectableFace> gizmoFaces;

        [HideInInspector]
        public bool isDirty = false;

        public Voxel GetVoxel(Vector3Int position)
        {
            manager.GetVoxel(this, position);
        }

        public Voxel GetVoxel(int x, int y, int z)
        {
            manager.GetVoxel(this, new Vector3Int(x,y,z));
        }


        public Voxel SafeSampleVoxel(int x, int y, int z)
        {
            if(x < 0 || x >= ChunkSize)
            {
                return new Voxel()
                {
                    IsEmpty = true
                };
            }
            if(y < 0 || y >= ChunkSize)
            {
                return new Voxel()
                {
                    IsEmpty = true
                };
            }
            if(z < 0 || z >= ChunkSize)
            {
                return new Voxel()
                {
                    IsEmpty = true
                };
            }


            int index = x + ChunkSize * (y + ChunkSize * z);
            return voxels[index];
        }


        public void SetVoxel(int x, int y, int z, Voxel voxel)
        {
            manager.SetVoxel(this, new Vector3Int(x,y,z), voxel);
        }

        public void SafeSetVoxel()
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

        [ContextMenu("Regen Gizmos")]
        void RegenGizmo()
        {
            if(tempGizmosParent == null)
            {
                GameObject gizmoParent = new GameObject("TEMP Gizmo Parent");
                gizmoParent.hideFlags = HideFlags.HideInHierarchy;
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
                foreach(SelectableFace f in GetComponentsInChildren<SelectableFace>())
                {
                    DestroyImmediate(f.gameObject);
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

                        if(GetVoxel(x,y+1,z).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Up, thisVoxel, this);
                        }
                        if(GetVoxel(x,y-1,z).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Down, thisVoxel, this);
                        }
                        if(GetVoxel(x,y,z+1).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Forward, thisVoxel, this);
                        }
                        if(GetVoxel(x,y,z-1).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Backward, thisVoxel, this);
                        }
                        if(GetVoxel(x+1,y,z).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Right, thisVoxel, this);
                        }
                        if(GetVoxel(x-1,y,z).IsEmpty)
                        {
                            SelectableFace.Create(tempGizmosParent, x, y, z, FaceDirection.Left, thisVoxel, this);
                        }
                    }
                }
            }
        }

        public struct VoxelFaceSelection
        {
            public Vector3Int position;
            public FaceDirection direction;

            public VoxelFaceSelection(Vector3Int pos, FaceDirection dir)
            {
                position = pos;
                direction = dir;
            }

            public static List<VoxelFaceSelection> FromSelection()
            {
                List<VoxelFaceSelection> outpt = new List<VoxelFaceSelection>();
                foreach(GameObject g in Selection.gameObjects)
                {
                    if(g.TryGetComponent(out SelectableFace face))
                    {
                        outpt.Add(new VoxelFaceSelection(face.voxelPosition, face.FaceDirection));
                    }
                }
                return outpt;
            }
        }

        void RegenGizmo(List<VoxelFaceSelection> newSelection)
        {
            RegenGizmo();
            List<GameObject> newFounds = new List<GameObject>();
            foreach(VoxelFaceSelection selection in newSelection)
            {
                var matching = GetComponentsInChildren<SelectableFace>().Where(p => p.voxelPosition == selection.position && p.FaceDirection == selection.direction).ToArray();
                if(matching.Length > 0)
                {
                    newFounds.Add(matching[0].gameObject);
                }
            }
            if(newFound.Count != 0)
            {
                Selection.objects = Selection.gameObjects.Concat(newFounds).Distinct().ToArray();
            }
        }


        [ContextMenu("Set Material to Selected")]
        void SetWithSelectedMaterial()
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

        [ContextMenu("Generate Backlot")]
        public void GenerateBacklots()
        {
            ulong[,,] axis_cols = new ulong[3,ChunkSizeP,ChunkSizeP];
            ulong[,,] col_face_masks = new ulong[6,ChunkSizeP,ChunkSizeP];

            for (int y = -1; y < ChunkSize + 1; y++)
            {
                for(int z = -1; z < ChunkSize + 1; z++)
                {
                    for(int x = -1; x < ChunkSize + 1; x++)
                    {
                        Vector3Int pos = new (x, y, z);
                        Voxel b = SafeSampleVoxel(x, y, z);
                        if(!b.IsEmpty)
                        {
                            axis_cols[0,z+1,x+1] |= 1ul << (y+1);
                            axis_cols[1,y+1,z+1] |= 1ul << (x+1);
                            axis_cols[2,y+1,x+1] |= 1ul << (z+1);
                        }
                    }
                }
            }

            // For every main axis
            for(int axis = 0; axis < 3; axis++)
            {
                for(int x = 0; x < ChunkSizeP; x++)
                {
                    for(int z = 0; z < ChunkSizeP; z++)
                    {
                        ulong col = axis_cols[axis, z, x];
                        col_face_masks[2*axis+0, z, x] = col & ~(col << 1);
                        col_face_masks[2*axis+1, z, x] = col & ~(col >> 1);
                    }
                }
            }

            Dictionary<Material, Dictionary<int, uint[]>>[] data = new Dictionary<Material, Dictionary<int, uint[]>>[6] 
            { 
                new(), 
                new(), 
                new(), 
                new(), 
                new(), 
                new() 
            };

            // For each of the 6 directions
            for(byte axis2 = 0; axis2 < 6; axis2++)
            {
                for(int z = 0; z < ChunkSize; z++)
                {
                    for(int x = 0; x < ChunkSize; x++)
                    {
                        int col_index = 1 + x + ((z+1) * ChunkSizeP) + ChunkSizeP2 * axis2;

                        ulong col = col_face_masks[axis2, z + 1, x + 1];
                        col >>= 1;
                        col &= ~(1ul << ChunkSize);

                        while (col != 0)
                        {
                            int y = GreedyGridwall.CountTrailingZeros(col);

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

                            byte facingDirection = 0;

                            facingDirection = axis2 switch
                            {
                                (4) => 5,
                                (5) => 4,
                                _ => axis2,
                            };
                            Material voxelID = currentVoxel.GetMaterial((FaceDirection)facingDirection);

                            if(voxelID == null)
                            {
                                voxelID = BacklotManager.DefaultGridMaterial();
                            }
                            //int voxelID = 0;

                            // WHAT THE FUCK :fireEmoji:
                            if (!data[axis2].ContainsKey(voxelID))
                            {
                                data[axis2].Add(voxelID, new Dictionary<int, uint[]>());
                            }
                            if (!data[axis2][voxelID].ContainsKey(y))
                            {
                                data[axis2][voxelID].Add(y, new uint[ChunkSize]);
                            }
                            data[axis2][voxelID][y][x] |= 1u << z;
                        }
                    }
                }
            }

            List<GreedyGridwall.ResultingBacklot> backlotGenResults = new List<GreedyGridwall.ResultingBacklot>();

            // add a list of Backlot Faces rn for here

            for (int axis = 0; axis < data.Length; axis++)
            {
                Dictionary<Material, Dictionary<int, uint[]>> block_mat_data = data[axis];

                foreach(var material in block_mat_data.Keys)
                {
                    var axis_plane = block_mat_data[material];

                    foreach (var axisPos in axis_plane.Keys)
                    {
                        uint[] plane = axis_plane[axisPos];

                        // now FINALLY greedymesh it
                        Debug.Log($"Axis: {axis_plane}\nAxis Pos: {axisPos}\n\"Axis plane int: {plane[0]}");
                        var generatedGrids = GreedyGridwall.GreedTheGrid(plane, material, (FaceDirection)axis, axisPos);
                        backlotGenResults.AddRange(generatedGrids);
                    }
                }
            }

            if (backlotsParent == null)
            {
                GameObject backParent = new GameObject("Generated Backlots");
                backParent.transform.parent = transform;
                backParent.transform.localPosition = Vector3.zero;
                backlotsParent = backParent.transform;
            }
            else
            {
                for (int c = backlotsParent.childCount - 1; c >= 0; c--)
                {
                    DestroyImmediate(backlotsParent.GetChild(c).gameObject);
                }
            }

            foreach (var backlot in backlotGenResults)
            {
                Debug.Log(backlot.scale);
                var gridwall = BacklotManager.FindGridWall(backlot.scale);
                if(gridwall != null)
                {
                    GameObject inst = PrefabUtility.InstantiatePrefab(gridwall, backlotsParent) as GameObject;
                    inst.GetComponentInChildren<MeshRenderer>().sharedMaterial = backlot.material;
                    inst.transform.localPosition = backlot.localPos;
                    inst.transform.rotation = backlot.rotation;
                }
            }
            isDirty = false;
        }

        [ContextMenu("Extrude Selection (Debug)")]
        public void ExtrudeSelection()
        {
#if UNITY_EDITOR
            List<VoxelFaceSelection> newSelection = new List<VoxelFaceSelection>();
            foreach (GameObject g in Selection.gameObjects)
            {
                if(g.TryGetComponent(out SelectableFace face))
                {
                    if(face.chunk != this) continue;
                    Vector3Int p = face.GetTargetAir();
                    Voxel newVox = new Voxel();
                    newVox.SetMaterial(face.FaceDirection, face.material);
                    SetVoxel(p.x, p.y, p.z, newVox);
                    newSelection.Add(new VoxelFaceSelection(p, face.FaceDirection));
                }
            }
            RegenGizmo(newSelection);
            isDirty = true;
#endif
        }

        public void ExtrudeFaceGizmo(SelectableFace face)
        {
            if(face.chunk != this) return;
            Vector3Int p = face.GetTargetAir();
            Voxel newVox = new Voxel();
            newVox.SetMaterial(face.FaceDirection, face.material);
            SetVoxel(p.x, p.y, p.z, newVox);
            RegenGizmo();
            isDirty = true;
        }

        [ContextMenu("Intrude Selection (Debug)")]
        public void IntrudeSelection()
        {
#if UNITY_EDITOR
            List<VoxelFaceSelection> newSelection = new List<VoxelFaceSelection>();
            foreach (GameObject g in Selection.gameObjects)
            {
                if(g.TryGetComponent(out SelectableFace face))
                {
                    if(face.chunk != this) continue;
                    Vector3Int newFace = face.GetBackwardPos();
                    SafeSampleVoxel(newFace.x, newFace.y, newFace.z).SetMaterial(face.FaceDirection, face.material);
                    Vector3Int p = face.voxelPosition;
                    SafeSampleVoxel(p.x, p.y, p.z).IsEmpty = true;
                    newSelection.Add(new VoxelFaceSelection(newFace, face.FaceDirection));
                }
            }
            RegenGizmo(newSelection);
            isDirty = true;
#endif
        }

        public void IntrudeFaceGizmo(SelectableFace face)
        {
            if(face.chunk != this) return;
            Vector3Int newFace = face.GetBackwardPos();
            SafeSampleVoxel(newFace.x, newFace.y, newFace.z).SetMaterial(face.FaceDirection, face.material);
            Vector3Int p = face.voxelPosition;
            SafeSampleVoxel(p.x, p.y, p.z).IsEmpty = true;
            RegenGizmo();
            isDirty = true;
        }

        public void PaintSelection(Material m)
        {
#if UNITY_EDITOR
            List<VoxelFaceSelection> newSelection = new List<VoxelFaceSelection>();
            foreach (GameObject g in Selection.gameObjects)
            {
                if(g.TryGetComponent(out SelectableFace face))
                {
                    if(face.chunk != this) continue;

                    Vector3Int p = face.voxelPosition;
                    Voxel voxel = SafeSampleVoxel(p.x, p.y, p.z);
                    voxel.SetMaterial(face.FaceDirection, m);
                    newSelection.Add(new VoxelFaceSelection(p, face.FaceDirection));
                }
            }
            RegenGizmo(newSelection);
            isDirty = true;
#endif
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Vector3 size = new Vector3(ChunkSize, ChunkSize, ChunkSize);
            Gizmos.DrawWireCube(transform.position + (size/2), size);
        }
    }


    [System.Serializable]
    public class Voxel
    {
        public bool IsEmpty = false;
        [SerializeField]
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
            public Vector3 localPos;
            public Vector2 scale;
            public FaceDirection axis;
            public int axisIndex;
            public Quaternion rotation;
            public Material material;
        }

        public static int CountTrailingZeros(ulong n)
        {
            if (n == 0) return 64; // Assuming 32-bit integer
            int count = 0;
            while ((n & 1) == 0)
            {
                n >>= 1;
                count++;
            }
            return count;
        }
        public static int CountTrailingZeros(int n)
        {
            if (n == 0) return 32; // Assuming 32-bit integer
            int count = 0;
            while ((n & 1) == 0)
            {
                n >>= 1;
                count++;
            }
            return count;
        }

        public static int[] ValidLengths = new int[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            8,
            10
            // Fill out later
        };

        public static int GetValidBacklotLength(int realLength)
        {
            if(ValidLengths.Contains(realLength)) return realLength;

            for(int i = 1; i < ValidLengths.Length; i++)
            {
                if(ValidLengths[i-1] < realLength && ValidLengths[i] > realLength)
                {
                    return ValidLengths[i-1];
                }
            }
            return ValidLengths[ValidLengths.Length-1];
        }

        public static List<ResultingBacklot> GreedTheGrid(uint[] data, Material theMat, FaceDirection axis, int axisPos)
        {
            List<ResultingBacklot> outpt = new List<ResultingBacklot>();
            for(int x = 0; x < data.Length; x++)
            {
                int y = 0;
                while(y < 32)
                {
                    y += CountTrailingZeros(data[x] >> y);
                    int height = GetValidBacklotLength(CountTrailingZeros(~(data[x] >> y)));

                    if(height == 0) break;

                    uint h_as_mask = (1u << height) - 1;
                    uint mask = h_as_mask << y;
                    int w = 1;


                    while (x + w < data.Length)
                    {
                        long nextRow = (data[x+w] >> y) & h_as_mask;
                        bool brokenInBigJump = false;

                        if (nextRow != h_as_mask)
                            break;

                        // if the next one is NOT a valid
                        // 6, 8
                        // If w = 6 (cant do 7 because not valid)
                        if(!ValidLengths.Contains(w+1))
                        {
                            int validW = w; // w = 6
                            // If it can, &= ~mask them all
                            // If it cant, break (out of all ugh)
                            int nextValidSize = ValidLengths[ValidLengths.IndexOf(w)+1];
                            // See if it can extend ALL THE WAY to the next valid one
                            // Will break out after checking extraW as 2, bc 8 is contained
                            for(int extraW = 1; !ValidLengths.Contains(w+extraW-1); extraW++)
                            {
                                // checks at 7
                                // checks at 8
                                long nextNextRow = (data[x + w + extraW] >> y) & h_as_mask;
                                if (nextNextRow != h_as_mask || x + w + extraW > data.Length - 1)
                                {
                                    w = validW;
                                    brokenInBigJump = true;
                                    break;
                                }
                                else if(ValidLengths.Contains(w+extraW))
                                {
                                    // Should happen when evaluating at 8, and none of the previous were broken
                                    // Means it can extend all the way to the next valid length. Set w to the new value and &= ~ the stuffs

                                    for(int g = 0; g < extraW; g++)
                                    {
                                        data[x+w+g] &= ~mask;
                                    }
                                    w += extraW;
                                    break;
                                }
                                // im actually dying this code hurts me
                            }
                        }

                        if(brokenInBigJump) break;

                        data[x+w] &= ~mask;
                        w += 1;
                    }

                    Vector2 scale = new Vector2(w, height);

                    var backlot = new ResultingBacklot() {
                        planePos = new Vector2(x, y) + (scale / 2),
                        scale = scale,
                        material = theMat,
                        axis = axis,
                        axisIndex = axisPos
                    };

                    bool normal = scale.y >= scale.x;

                    switch (axis)
                    {
                        case FaceDirection.Forward:
                            backlot.rotation = Quaternion.LookRotation(normal ? Vector3.up : Vector3.right, Vector3.forward);
                            backlot.localPos = new Vector3(backlot.planePos.x, backlot.planePos.y, axisPos);
                            break;
                        case FaceDirection.Backward:
                            backlot.rotation = Quaternion.LookRotation(normal ? Vector3.up : Vector3.right, Vector3.back);
                            backlot.localPos = new Vector3(backlot.planePos.x, backlot.planePos.y, axisPos + 1);
                            break;
                        case FaceDirection.Up:
                            backlot.rotation = Quaternion.LookRotation(normal ? Vector3.forward : Vector3.right, Vector3.down);
                            backlot.localPos = new Vector3(backlot.planePos.x, axisPos + 1, backlot.planePos.y);
                            break;
                        case FaceDirection.Down:
                            backlot.rotation = Quaternion.LookRotation(normal ? Vector3.forward : Vector3.right, Vector3.up);
                            backlot.localPos = new Vector3(backlot.planePos.x, axisPos, backlot.planePos.y);
                            break;
                        case FaceDirection.Left:
                            backlot.rotation = Quaternion.LookRotation(normal ? Vector3.up : Vector3.forward, Vector3.right);
                            backlot.localPos = new Vector3(axisPos, backlot.planePos.y, backlot.planePos.x);
                            break;
                        case FaceDirection.Right:
                            backlot.rotation = Quaternion.LookRotation(normal ? Vector3.up : Vector3.forward, Vector3.left);
                            backlot.localPos = new Vector3(axisPos + 1, backlot.planePos.y, backlot.planePos.x);
                            break;
                    }

                    outpt.Add(backlot);

                    y += height;

                }
            }
            return outpt;
        }
    }
}
