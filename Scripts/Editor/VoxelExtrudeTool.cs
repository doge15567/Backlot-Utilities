using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic

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
        if (targets == null || targets.Length == 0)
            return;

        SelectableFace targetComponent = (SelectableFace)targets[0];
        Transform targetTransform = targetComponent.transform;

        EditorGUI.BeginChangeCheck();

        Vector3 movementAxis = targetComponent.GetTargetAir();
        Vector3 currentPosition = targetTransform.position;

        Handles.color = new Color(0, 1, 1, 1);

        Vector3 handlePosition = Handles.Slider(
            currentPosition,
            movementAxis
            HandleUtility.GetHandleSize(currentPosition) * 1.0f,
            Handles.ArrowHandleCap,
            0.1f
        );
        
        if (EditorGUI.EndChangeCheck())
        {
            float gridSize = 1.0f;

            Vector3 offset = handlePosition - currentPosition;
            float distanceAlongAxis = Vector3.Dot(offset, axis);

            float snappedDistance = Mathf.Round(distanceAlongAxis / gridSize) * gridSize;

            if(snappedDistance >= 1f)
            {
                foreach(Object obj in targets)
                {
                    SelectableFace face = (SelectableFace)obj;
                    if(face.GetTargetAir() == movementAxis)
                    {
                        face.Extrude(false);
                    }
                }
            }
            else if (snappedDistance <= -1)
            {
                foreach(Object obj in targets)
                {
                    SelectableFace face = (SelectableFace)obj;
                    if(face.GetTargetAir() == movementAxis)
                    {
                        face.Extrude(true);
                    }
                }
            }
        }
    }
}
