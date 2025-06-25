#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EvroDev.BacklotUtilities
{
    public static class BacklotManager
    {
        public static Material DefaultGridMaterial()
        {
            if (_gridMat != null) return _gridMat;

            string matName = "mat_grid_Grey_67_L";
            string[] guids = AssetDatabase.FindAssets(matName);
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                Material gridWall = AssetDatabase.LoadAssetAtPath<Material>(assetPath);

                if (gridWall != null)
                {
                    _gridMat = gridWall;
                    return gridWall;
                }
            }
            return null;
        }

        private static Material _gridMat;


        public static GameObject FindGridWall(Vector2 size)
        {
            if(size.y < size.x)
            {
                (size.y, size.x) = (size.x, size.y);
            }
            // Create both possible name formats: width x height and height x width
            string wallName1 = $"t:prefab plane_{size.x}x{size.y}";

            // Search for the asset in the package directory (first by size.x x size.y)
            string[] guids = AssetDatabase.FindAssets(wallName1);

            // Loop through the found assets and try to load the first match
            foreach (var guid in guids)
            {
                // Get the asset path from the GUID
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                // Try to load the asset as a GameObject
                GameObject gridWall = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                // If a valid grid wall is found, return it
                if (gridWall != null)
                {
                    return gridWall;
                }
            }

            // If no grid wall is found, log a warning and return null
            Debug.LogWarning($"Grid wall {wallName1} not found.");
            return null;
        }
    }
}
#endif