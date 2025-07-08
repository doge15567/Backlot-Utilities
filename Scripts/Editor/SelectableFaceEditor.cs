#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SLZ.MarrowEditor;
using SLZ.Marrow.Warehouse;
using System;
using Object = UnityEngine.Object;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace EvroDev.BacklotUtilities.Voxels
{
    [CustomEditor(typeof(SelectableFace))]
    [CanEditMultipleObjects]
    public class SelectableFaceEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            // Create property container element.
            VisualElement container = new VisualElement();

            var Title = new Label("Voxel Data");
            Title.style.fontSize = 20;
            Title.style.unityTextAlign = TextAnchor.MiddleCenter;
            Title.style.letterSpacing = new StyleLength(12.2f);
            Title.style.unityTextOutlineWidth = 0.65f;


            var spacer = new Label();
            spacer.style.fontSize = 20;
            spacer.style.unityTextAlign = TextAnchor.MiddleCenter;
            spacer.style.letterSpacing = new StyleLength(15.2f);
            spacer.style.unityTextOutlineWidth = 1.3f;
            spacer.style.unityTextOutlineColor = new Color(0.3f, 1f, 0.3f, 1);

            ObjectField voxelMaterial = new ObjectField();
            voxelMaterial.objectType = typeof(Material);
            voxelMaterial.allowSceneObjects = false;
            voxelMaterial.value = (Material)serializedObject.FindProperty("material").objectReferenceValue;
            voxelMaterial.RegisterValueChangedCallback(evt =>
            {
                UpdateVoxelMaterials((Material)evt.newValue);
            });


            var surfaceData = new PropertyField(serializedObject.FindProperty("surfaceData"));
            surfaceData.RegisterValueChangeCallback(evt =>
            {
                //Debug.Log("H", evt.changedProperty.serializedObject.targetObject);
                serializedObject.ApplyModifiedProperties(); // After changing the property, this callback gets called every ~.25 seconds the face stays selected.
                UpdateVoxelSurface(((SelectableFace)evt.changedProperty.serializedObject.targetObject).surfaceData);
<<<<<<< Updated upstream
=======
                //serializedObject.ApplyModifiedProperties();
                //Selection.objects = Array.Empty<Object>();
>>>>>>> Stashed changes
            });

            Toggle removeButton = new Toggle("Is Empty");
            removeButton.value = serializedObject.FindProperty("IsEmpty").boolValue;
            removeButton.RegisterValueChangedCallback(evt =>
            {
                UpdateVoxelEnabled(evt.newValue);
            });

            // Add fields to the container.
            container.Add(Title);
            container.Add(spacer);
            container.Add(voxelMaterial);
            container.Add(surfaceData);
            container.Add(removeButton);

            return container;
        }

        private static void UpdateVoxelEnabled(bool value)
        {
            GameObject[] voxels = Selection.gameObjects;

            foreach (GameObject voxel in voxels)
            {
                SelectableFace face;
                if (voxel.TryGetComponent<SelectableFace>(out face))
                {
                    face.IsEmpty = value;
                    face.chunk.GetVoxel(face.voxelPosition).SetOverrideFace(face.FaceDirection, value);
                    face.chunk.isDirty = true;
                    EditorUtility.SetDirty(face);
                }
            }
        }


        private static void UpdateVoxelMaterials(Material newMaterial)
        {
            GameObject[] voxels = Selection.gameObjects;

            foreach (GameObject voxel in voxels)
            {
                SelectableFace face;
                if (voxel.TryGetComponent<SelectableFace>(out face))
                {
                    face.material = newMaterial;
                    face.chunk.GetVoxel(face.voxelPosition).SetMaterial(face.FaceDirection, newMaterial);
                    face.chunk.isDirty = true;
                    EditorUtility.SetDirty(face);
                }
            }
        }

        private static void UpdateVoxelSurface(DataCardReference<SurfaceDataCard> newSurface)
        {
            GameObject[] voxels = Selection.gameObjects;
            foreach (GameObject voxel in voxels)
            {
                SelectableFace face;
                if (voxel.TryGetComponent<SelectableFace>(out face))
                {
                    face.surfaceData = newSurface;
                    //foreach (var item in face.chunk.manager.surfaceDataCache)
                    //{
                    //    Debug.Log($"Iterate list -1: {item}");
                    //}
                    face.chunk.GetVoxel(face.voxelPosition).SetSurface(face.FaceDirection, newSurface);
                    face.chunk.isDirty = true;
                    EditorUtility.SetDirty(face);
                }
            }
        }
    }
}
#endif