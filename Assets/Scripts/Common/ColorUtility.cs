using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class ColorUtility
{
    public struct HSV
    {
        public float h;
        public byte s;
        public byte v;
        public short a;
    }

    public static Color32 ToGrayScale(ref Color32 c)
    {
        var t = (byte)Mathf.Clamp((int)(0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b), 0, 255);
        return new Color32(t, t, t, c.a);
    }

    public static NativeArray<HSV> RGBtoHSV(NativeArray<Color32> source, bool isTemporal = true)
    {
        var result = new NativeArray<HSV>(source.Length, isTemporal ? Allocator.Temp : Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        var i = 0;
        foreach (var c in source) {
            var max = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
            var min = Mathf.Min(Mathf.Min(c.r, c.g), c.b);

            var hsv = new HSV { h = 0.0f, s = (byte)(max - min), v = (byte)max, a = c.a };

            if (min == max) {
                hsv.h = 0;
            } else if (min == c.b) {
                hsv.h = 60.0f * (c.g - c.r) / hsv.s + 60.0f;
            } else if (min == c.r) {
                hsv.h = 60.0f * (c.b - c.g) / hsv.s + 180.0f;
            } else { // if (min == c.g)
                hsv.h = 60.0f * (c.r - c.b) / hsv.s + 300.0f;
            }

            result[i++] = hsv;
        }
        return result;
    }

    public static NativeArray<Color32> HSVtoRGB(NativeArray<HSV> source, bool isTemporal = true)
    {
        var result = new NativeArray<Color32>(source.Length, isTemporal ? Allocator.Temp : Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        var i = 0;
        foreach (var hsv in source) {
            var h = ((int)hsv.h / 60);
            var c = hsv.s;
            var x = (byte)(c * (1 - Mathf.Abs((h % 2) - 1)));

            var t = (byte)(hsv.v - hsv.s);
            var rgb = new Color32(t, t, t, (byte)hsv.a);
            if (h > 0) {
                if (h < 1) {
                    rgb.r += c;
                    rgb.g += x;
                } else if (h < 2) {
                    rgb.r += x;
                    rgb.g += c;
                } else if (h < 3) {
                    rgb.g += c;
                    rgb.b += x;
                } else if (h < 4) {
                    rgb.g += x;
                    rgb.b += c;
                } else if (h < 5) {
                    rgb.r += x;
                    rgb.b += c;
                } else if (h < 6) {
                    rgb.r += c;
                    rgb.b += x;
                }
            }
            result[i++] = rgb;
        }
        return result;
    }
}
