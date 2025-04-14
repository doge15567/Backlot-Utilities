using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using EvroDev.BacklotUtilities.Extensions;
using UnityEditor;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EvroDev.BacklotUtilities.Voxels
{
    public enum FaceDirection
    {
        Forward = 4,
        Backward = 5,
        Up = 1,
        Down = 0,
        Left = 2,
        Right = 3
    }

    public class SelectableFace : MonoBehaviour
    {
        public BacklotVoxelChunk chunk;
        public Vector3Int voxelPosition;
        public FaceDirection FaceDirection;
        public Material material;

        void OnDrawGizmos()
        {
            if(chunk.manager.visualizationMode != VisualizationMode.Gizmos) return;

            Vector3 centerPos = transform.parent.position + voxelPosition + (Vector3.one / 2);
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

            transform.position = centerPos;

            Gizmos.color = new Color(1, 1, 1, 0.1f);
            if (material)
            {
                try
                {
                    Gizmos.color = TextureColorExtensions.GetAverageColor(material.mainTexture as Texture2D) * material.color;
                }
                catch
                {
                    Gizmos.color = material.color * new Color(1, 1, 1, 0.1f);
                }
            }
            Gizmos.DrawCube(centerPos, scale);
        }

        private void OnDrawGizmosSelected()
        {
            if(chunk.manager.visualizationMode != VisualizationMode.Gizmos) return;

            Vector3 centerPos = transform.parent.position + voxelPosition + (Vector3.one / 2);
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
            if (material)
            {
                try
                {
                    Gizmos.color = TextureColorExtensions.GetAverageColor(material.mainTexture as Texture2D) * material.color;
                }
                catch
                {
                    Gizmos.color = material.color * new Color(1, 1, 1, 0.3f);
                }
            }
            Gizmos.DrawCube(centerPos, scale);
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            Gizmos.DrawWireCube(centerPos, scale);
        }

        public Vector3Int GetTargetAir()
        {
            if (FaceDirection == FaceDirection.Up)
            {
                return voxelPosition + new Vector3Int(0, 1, 0);
            }
            else if (FaceDirection == FaceDirection.Down)
            {
                return voxelPosition + new Vector3Int(0, -1, 0);
            }
            else if (FaceDirection == FaceDirection.Forward)
            {
                return voxelPosition + new Vector3Int(0, 0, 1);
            }
            else if (FaceDirection == FaceDirection.Backward)
            {
                return voxelPosition + new Vector3Int(0, 0, -1);
            }
            else if (FaceDirection == FaceDirection.Left)
            {
                return voxelPosition + new Vector3Int(-1, 0, 0);
            }
            else if (FaceDirection == FaceDirection.Right)
            {
                return voxelPosition + new Vector3Int(1, 0, 0);
            }
            return voxelPosition;
        }

        public Vector3Int GetBackwardPos()
        {
            if (FaceDirection == FaceDirection.Up)
            {
                return voxelPosition + new Vector3Int(0, -1, 0);
            }
            else if (FaceDirection == FaceDirection.Down)
            {
                return voxelPosition + new Vector3Int(0, 1, 0);
            }
            else if (FaceDirection == FaceDirection.Forward)
            {
                return voxelPosition + new Vector3Int(0, 0, -1);
            }
            else if (FaceDirection == FaceDirection.Backward)
            {
                return voxelPosition + new Vector3Int(0, 0, 1);
            }
            else if (FaceDirection == FaceDirection.Left)
            {
                return voxelPosition + new Vector3Int(1, 0, 0);
            }
            else if (FaceDirection == FaceDirection.Right)
            {
                return voxelPosition + new Vector3Int(-1, 0, 0);
            }
            return voxelPosition;
        }

        public void Extrude(bool add)
        {
            if(!add)
            {
                chunk.ExtrudeFaceGizmo(this);
            }
            else
            {
                chunk.IntrudeFaceGizmo(this);
            }
        }

        public static SelectableFace Create(Transform parent, int x, int y, int z, FaceDirection direction, Voxel voxel, BacklotVoxelChunk chunk)
        {
            GameObject go = new GameObject("VoxelFace");
            SelectableFace outpt = go.AddComponent<SelectableFace>();
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;

            outpt.voxelPosition = new Vector3Int(x, y, z);
            outpt.FaceDirection = direction;

            if (voxel.GetMaterial(direction) != null)
                outpt.material = voxel.GetMaterial(direction);

            outpt.chunk = chunk;

            return outpt;
        }
    }

    [InitializeOnLoad]
    public class PlaneGizmoToolGUI
    {
        static PlaneGizmoToolGUI()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        static void OnSceneGUI(SceneView sceneView)
        {
            // Filter selected objects to only those with PlaneGizmo
            var selectedPlanes = Selection.gameObjects
                .Select(go => go.GetComponent<SelectableFace>())
                .Where(p => p != null)
                .ToArray();

            if (selectedPlanes.Length == 0)
                return;

            // Calculate center position
            Vector3 center = Vector3.zero;
            foreach (var plane in selectedPlanes)
                center += plane.transform.position;
            center /= selectedPlanes.Length;

            // Convert world position to GUI position
            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(center);

            //Handles.BeginGUI();

            //GUILayout.BeginArea(new Rect(guiPosition.x - 50, guiPosition.y - 30, 100, 60), GUI.skin.box);

            Handles.color = Color.cyan;
            float size = HandleUtility.GetHandleSize(center + Vector3.up * 0.5f) * 0.2f;

            if (Handles.Button(center + Vector3.up * 0.5f, Quaternion.identity, size, size, Handles.SphereHandleCap))
            {
                ExtrudeSelectedPlanes(selectedPlanes, true);
            }

            Handles.color = Color.red;
            float size2 = HandleUtility.GetHandleSize(center - Vector3.up * 0.5f) * 0.2f;

            if (Handles.Button(center - Vector3.up * 0.5f, Quaternion.identity, size2, size2, Handles.SphereHandleCap))
            {
                ExtrudeSelectedPlanes(selectedPlanes, false);
            }

            //GUILayout.EndArea();
            //Handles.EndGUI();
        }

        static void ExtrudeSelectedPlanes(SelectableFace[] planes, bool add)
        {
            foreach (var plane in planes)
            {
                plane.Extrude(add); // Assume you have a method like this
            }

            SceneView.RepaintAll();
        }
    }
}
