#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EvroDev.BacklotUtilities.Extensions
{
    public static class TextureColorExtensions
    {
        public static Color GetAverageColor(this Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogError("Texture is null.");
                return Color.clear;
            }

            // Retrieve all pixel colors from the texture
            Color[] pixels = texture.GetPixels();
            if (pixels.Length == 0)
                return Color.clear;

            Color sum = Color.clear;
            // Sum all the pixel colors
            foreach (Color pixel in pixels)
            {
                sum += pixel;
            }

            // Divide the sum by the total number of pixels to get the average
            return sum / pixels.Length;
        }

        public static bool InBounds(this Vector3Int pos, int bound)
        {
            if (pos.x < 0 || pos.y < 0 || pos.z < 0) return false;
            if (pos.x >= bound || pos.y >=bound || pos.z >= bound) return false;
            return true;
        }
    }
}
#endif