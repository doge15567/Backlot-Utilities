using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EvroDev.BacklotUtilities.Voxels
{
    public enum FaceDirection
    {
        Forward,
        Backward,
        Up,
        Down,
        Left,
        Right
    }

    public class SelectableFace : MonoBehaviour
    {
        public Vector3Int voxelPosition;
        public FaceDirection FaceDirection;
        public Material material;

        void OnDrawGizmos()
        {
            Vector3 centerPos = transform.position + voxelPosition + (Vector3.one/2);
            Vector3 scale = Vector3.one;
            if(FaceDirection == FaceDirection.Up)
            {
                centerPos += new Vector3(0, 0.5f, 0);
                scale = new Vector3(0.9f, 0.01f, 0.9f);
            }
            else if(FaceDirection == FaceDirection.Down)
            {
                centerPos += new Vector3(0, -0.5f, 0);
                scale = new Vector3(0.9f, 0.01f, 0.9f);
            }
            else if(FaceDirection == FaceDirection.Forward)
            {
                centerPos += new Vector3(0, 0, 0.5f);
                scale = new Vector3(0.9f, 0.9f, 0.01f);
            }
            else if(FaceDirection == FaceDirection.Backward)
            {
                centerPos += new Vector3(0, 0, -0.5f);
                scale = new Vector3(0.9f, 0.9f, 0.01f);
            }
            else if(FaceDirection == FaceDirection.Right)
            {
                centerPos += new Vector3(0.5f, 0, 0);
                scale = new Vector3(0.01f, 0.9f, 0.9f);
            }
            else if(FaceDirection == FaceDirection.Left)
            {
                centerPos += new Vector3(-0.5f, 0, 0);
                scale = new Vector3(0.01f, 0.9f, 0.9f);
            }

            Gizmos.color = new Color(1, 1, 1, 0.1f);
            Gizmos.DrawCube(centerPos, scale);
        }

        private void OnDrawGizmosSelected()
        {
            Vector3 centerPos = transform.position + voxelPosition + (Vector3.one / 2);
            Vector3 scale = Vector3.one;
            if (FaceDirection == FaceDirection.Up)
            {
                centerPos += new Vector3(0, 0.5f, 0);
                scale = new Vector3(0.9f, 0.01f, 0.9f);
            }
            else if (FaceDirection == FaceDirection.Down)
            {
                centerPos += new Vector3(0, -0.5f, 0);
                scale = new Vector3(0.9f, 0.01f, 0.9f);
            }
            else if (FaceDirection == FaceDirection.Forward)
            {
                centerPos += new Vector3(0, 0, 0.5f);
                scale = new Vector3(0.9f, 0.9f, 0.01f);
            }
            else if (FaceDirection == FaceDirection.Backward)
            {
                centerPos += new Vector3(0, 0, -0.5f);
                scale = new Vector3(0.9f, 0.9f, 0.01f);
            }
            else if (FaceDirection == FaceDirection.Right)
            {
                centerPos += new Vector3(0.5f, 0, 0);
                scale = new Vector3(0.01f, 0.9f, 0.9f);
            }
            else if (FaceDirection == FaceDirection.Left)
            {
                centerPos += new Vector3(-0.5f, 0, 0);
                scale = new Vector3(0.01f, 0.9f, 0.9f);
            }

            Gizmos.color = new Color(1, 1, 1, 0.3f);
            Gizmos.DrawCube(centerPos, scale);
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            Gizmos.DrawWireCube(centerPos, scale);
        }

        public Vector3Int GetTargetAir()
        {
            if(FaceDirection == FaceDirection.Up)
            {
                return voxelPosition + new Vector3Int(0, 1, 0);
            }
            else if(FaceDirection == FaceDirection.Down)
            {
                return voxelPosition + new Vector3Int(0, -1, 0);
            }
            else if(FaceDirection == FaceDirection.Forward)
            {
                return voxelPosition + new Vector3Int(0, 0, 1);
            }
            else if(FaceDirection == FaceDirection.Backward)
            {
                return voxelPosition + new Vector3Int(0, 0, -1);
            }
            else if(FaceDirection == FaceDirection.Left)
            {
                return voxelPosition + new Vector3Int(-1, 0, 0);
            }
            else if(FaceDirection == FaceDirection.Right)
            {
                return voxelPosition + new Vector3Int(1, 0, 0);
            }
            return voxelPosition;
        }

        public static SelectableFace Create(Transform parent, int x, int y, int z, FaceDirection direction, Voxel voxel)
        {
            GameObject go = new GameObject("VoxelFace");
            SelectableFace outpt = go.AddComponent<SelectableFace>();
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            outpt.voxelPosition = new Vector3Int(x,y,z);
            outpt.FaceDirection = direction;

            if(voxel.materials.ContainsKey(direction))
                outpt.material = voxel.materials[direction];

            return outpt;
        }
    }
}
