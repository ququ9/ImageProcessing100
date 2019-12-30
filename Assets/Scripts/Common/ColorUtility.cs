using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class ColorUtility
{
    public struct HSV
    {
        public int h;
        public byte s;
        public byte v;
        public short a;
    }

    public static Color32 RGBtoGrayScale(Color32 c)
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
            var hsv = new HSV { h = 0, s = (byte)(max - min), v = (byte)max, a = c.a };
            
            if (min == max) {
                hsv.h = 0;
            } else if (min == c.b) {
                hsv.h = 60 * (c.g - c.r) / hsv.s + 60;
            } else if (min == c.r) {
                hsv.h = 60 * (c.b - c.g) / hsv.s + 180;
            } else { // if (min == c.g)
                hsv.h = 60 * (c.r - c.b) / hsv.s + 300;
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
            var h = (((hsv.h + 360) % 360) / 60.0f);
            var c = hsv.s;
            var x = (int)(c * (1 - Mathf.Abs((h % 2) - 1)));

            var t = (hsv.v - hsv.s);
            var r = t;
            var g = t;
            var b = t;

            if (h < 1) {
                r += c;
                g += x;
            } else if (h < 2) {
                r += x;
                g += c;
            } else if (h < 3) {
                g += c;
                b += x;
            } else if (h < 4) {
                g += x;
                b += c;
            } else if (h < 5) {
                r += x;
                b += c;
            } else if (h < 6) {
                r += c;
                b += x;
            }

            result[i++] = new Color32((byte)Mathf.Clamp(r, 0, 255), (byte)Mathf.Clamp(g, 0, 255), (byte)Mathf.Clamp(b, 0, 255), (byte)hsv.a);
        }
        return result;
    }
}
