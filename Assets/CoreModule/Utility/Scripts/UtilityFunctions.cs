#pragma warning disable IDE0130

namespace Texell.Utility
{

    using System;
    using System.Collections;
    using UnityEngine;

    public static class UtilityFunctions
    {
        public static string FormatLargeNumber(long number)
        {
            string numberFormat;

            if (number < 1000)
            {
                numberFormat = number.ToString();
            }
            else if (number >= 1000 && number < 1000000)
            {
                numberFormat = string.Format("{0:0,.0K}", number);
            }
            else if (number >= 1000000 && number < 1000000000)
            {
                numberFormat = string.Format("{0:0,.0M}", number / 1000.0);
            }
            else
            {
                numberFormat = string.Format("{0:0,.0B}", number / 1000000.0);
            }

            return numberFormat;
        }

        public static Color32 HexStringToColor(string hex)
        {
            int hexColor = Convert.ToInt32(hex.Replace("#", ""), 16);
            int red = (hexColor >> 16) & 0xFF;
            int green = (hexColor >> 8) & 0xFF;
            int blue = hexColor & 0xFF;

            Color32 color = new((byte)red, (byte)green, (byte)blue, 255);
            return color;
        }

        public static IEnumerator LoadResourcesAsync<T>(string path, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
        {
            var request = Resources.LoadAsync<T>(path);
            yield return request;
            callback?.Invoke(request.asset);
        }

        public static byte[] HexStringToBytes(string hex)
        {
            if (hex.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length.");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }

}