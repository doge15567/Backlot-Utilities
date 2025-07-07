#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using SLZ.MarrowEditor;
using SLZ.Marrow.Warehouse;

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

            TextField surfaceData = new TextField("Surface Data");
            surfaceData.value = serializedObject.FindProperty("surfaceData").stringValue;
            surfaceData.RegisterValueChangedCallback(evt =>
            {
                UpdateVoxelSurface(new DataCardReference<SurfaceDataCard>(evt.newValue));
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
                    face.surfaceData = newSurface.Barcode.ID;
                    face.chunk.GetVoxel(face.voxelPosition).SetSurface(face.FaceDirection, newSurface);
                    face.chunk.isDirty = true;
                    EditorUtility.SetDirty(face);
                }
            }
        }
    }
}
#endif