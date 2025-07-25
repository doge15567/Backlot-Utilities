#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor.EditorTools;
using System.Linq;

namespace EvroDev.BacklotUtilities.Voxels
{
    [EditorTool("Voxel Extrusion Tool", typeof(SelectableFace))]
    public class VoxelExtrudeTool : EditorTool
    {
        GUIContent m_Icon;

        void OnEnable()
        {
            m_Icon = new GUIContent()
            {
                image = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Icons/MoveToolIcon.png"),
                text = "Voxel Extrusion Tool",
                tooltip = "Extrude Backlot Voxels"
            };
        }

        public override GUIContent toolbarIcon => m_Icon;

        public override void OnToolGUI(EditorWindow window)
        {
            if (targets == null || targets.Count() == 0)
                return;

            SelectableFace targetComponent = (SelectableFace)targets.First();
            Transform targetTransform = targetComponent.transform;

            Handles.BeginGUI();

            Vector3 worldUiPosition = targetTransform.position + Vector3.up;
            Vector2 guiPosition = HandleUtility.WorldToGUIPoint(worldUiPosition);

            GUILayout.BeginArea(new Rect(guiPosition.x - 60, guiPosition.y - 30, 120, 60), GUI.skin.box);
            GUILayout.Label("Custom Tools");

            if (GUILayout.Button("Flood Fill"))
            {
                foreach (UnityEngine.Object obj in targets)
                {
                    ((SelectableFace)obj).chunk.FloodFillSelect(new List<SelectableFace>() { (SelectableFace)obj });
                    // line above is killing m
                }
            }

            GUILayout.EndArea();
            Handles.EndGUI();

            EditorGUI.BeginChangeCheck();

            Vector3 movementAxis = targetComponent.GetAxis();
            Vector3 currentPosition = targetTransform.position;

            Handles.color = new Color(0, 1, 1, 1);

            Vector3 handlePosition = Handles.Slider(
                currentPosition,
                movementAxis,
                HandleUtility.GetHandleSize(currentPosition) * 1.0f,
                Handles.ArrowHandleCap,
                0.1f
            );

            if (EditorGUI.EndChangeCheck())
            {
                float gridSize = 1.0f;

                Vector3 offset = handlePosition - currentPosition;
                float distanceAlongAxis = Vector3.Dot(offset, movementAxis);

                float snappedDistance = Mathf.Round(distanceAlongAxis / gridSize) * gridSize;

                if (snappedDistance >= 1f)
                {
                    List<SelectableFace> facesToMove = GetFacesToMove();
                    facesToMove[0].chunk.ExtrudeFaceGizmos(facesToMove);
                }
                else if (snappedDistance <= -1)
                {
                    List<SelectableFace> facesToMove = GetFacesToMove();
                    facesToMove[0].chunk.IntrudeFaceGizmos(facesToMove);
                }

                List<SelectableFace> GetFacesToMove()
                {
                    List<SelectableFace> facesToMove = new List<SelectableFace>();
                    foreach (UnityEngine.Object obj in targets)
                    {
                        SelectableFace face = (SelectableFace)obj;
                        if (face.GetAxis() == movementAxis)
                        {
                            facesToMove.Add(face);
                        }
                    }
                    return facesToMove;
                }
            }
        }
    }
}
#endif