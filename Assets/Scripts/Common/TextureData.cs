using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class TextureData : IEnumerable<(int x, int y)>, IDisposable
{
    private static readonly Color32 kColorBlack = new Color32(0, 0, 0, 0);

    private NativeArray<Color32> _pixels;
    private readonly int _width;
    private readonly int _height;

    public int Width => _width;
    public int Height => _height;

    public NativeArray<Color32> Pixels => _pixels;

    public TextureData(Texture2D source, bool isTemporal)
    {
        _width = source.width;
        _height = source.height;

        _pixels = new NativeArray<Color32>(source.GetRawTextureData<Color32>(), isTemporal ? Allocator.Temp : Allocator.Persistent);
    }

    private TextureData(int width, int height, NativeArray<Color32> tempBuffer)
    {
        _pixels = tempBuffer;
        _width = width;
        _height = height;
    }

    public void Dispose()
    {
        _pixels.Dispose();
    }

    public static TextureData CreateTemporal(int width, int height)
    {
        var tempBuffer = new NativeArray<Color32>(width * height, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
        return new TextureData(width, height, tempBuffer);
    }

    public static TextureData CreateTemporalGrayScaleFromRGB(TextureData rgb)
    {
        var temp = TextureData.CreateTemporal(rgb.Width, rgb.Height);
        foreach (var (x, y) in rgb) {
            temp.SetPixel(x, y, ColorUtility.RGBtoGrayScale(rgb.GetPixel(x, y)));
        }
        return temp;
    }

    public static NativeArray<ColorUtility.HSV> CreateTemporalHSVFromRGB(TextureData rgb)
    {
        return ColorUtility.RGBtoHSV(rgb._pixels, true);
    }

    public static TextureData CreateTemporalRGBFromHSV(NativeArray<ColorUtility.HSV> hsv, int width, int height)
    {
        var result = ColorUtility.HSVtoRGB(hsv, true);
        return new TextureData(width, height, result);
    }

    public IEnumerator<(int x, int y)> GetEnumerator()
    {
        return new TextureDataEnumerator(_width, _height);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public void ApplyToTexture(Texture2D destination)
    {
        destination.LoadRawTextureData(_pixels);
        destination.Apply(false);
    }

    public Color32 GetPixel(int x, int y)
    {
        var index = (y * _width) + x;
        return _pixels[index];
    }

    public Color32 GetPixelSafe(int x, int y)
    {
        if (!this.IsValidCoordinate(x, y)) {
            return kColorBlack;
        }

        var index = (y * _width) + x;
        return _pixels[index];
    }

    public void SetPixel(int x, int y, Color32 color)
    {
        var index = (y * _width) + x;
        _pixels[index] = color;
    }

    public void SetPixelSafe(int x, int y, Color color)
    {
        if (this.IsValidCoordinate(x, y)) {
            this.SetPixel(x, y, color);
        }
    }

    public void FillPixels(int startX, int startY, int fillWidth, int fillHeight, Color32 fillColor)
    {
        var endX = Mathf.Min(_width - 1, startX + fillWidth - 1);
        var endY = Mathf.Min(_height - 1, startY + fillHeight - 1);
        for (var y = startY; y <= endY; ++y) {
            for (var x = startX; x <= endX; ++x) {
                this.SetPixel(x, y, fillColor);
            }
        }
    }

    public bool IsValidCoordinate(int x, int y)
    {
        if ((x < 0) || (x >= _width)) {
            return false;
        }
        if ((y < 0) || (y >= _height)) {
            return false;
        }
        return true;
    }

    public Color32 CalcAverage()
    {
        return this.CalcAverage(0, 0, _width, _height);
    }

    public Color32 CalcAverage(int startX, int startY, int sizeX, int sizeY)
    {
        var endX = Mathf.Min(_width - 1, startX + sizeX - 1);
        var endY = Mathf.Min(_height - 1, startY + sizeY - 1);

        double sumR = 0.0f;
        double sumG = 0.0f;
        double sumB = 0.0f;
        double sumA = 0.0f;
        var count = 0;
        for (var y = startY; y <= endY; ++y) {
            for (var x = startX; x <= endX; ++x) {
                var c = this.GetPixel(x, y);
                sumR += c.r;
                sumG += c.g;
                sumB += c.b;
                sumA += c.a;
                ++count;
            }
        }

        double scale = 1.0 / count;
        var r = (byte)Mathf.Clamp((int)(scale * sumR), 0, 255);
        var g = (byte)Mathf.Clamp((int)(scale * sumG), 0, 255);
        var b = (byte)Mathf.Clamp((int)(scale * sumB), 0, 255);
        var a = (byte)Mathf.Clamp((int)(scale * sumA), 0, 255);
        return new Color32(r, g, b, a);
    }

    public Color32 CalcVariance(int startX, int startY, int sizeX, int sizeY)
    {
        var average = this.CalcAverage(startX, startY, sizeX, sizeY);
        return this.CalcVariance(startX, startY, sizeX, sizeY, average);
    }

    public Color32 CalcVariance(int startX, int startY, int sizeX, int sizeY, Color32 average)
    {
        var endX = Mathf.Min(_width - 1, startX + sizeX - 1);
        var endY = Mathf.Min(_height - 1, startY + sizeY - 1);

        double sumR = 0.0f;
        double sumG = 0.0f;
        double sumB = 0.0f;
        double sumA = 0.0f;
        var count = 0;
        for (var y = startY; y <= endY; ++y) {
            for (var x = startX; x <= endX; ++x) {
                var c = this.GetPixel(x, y);
                sumR += ((c.r - average.r) * (c.r - average.r));
                sumG += ((c.g - average.g) * (c.g - average.g));
                sumB += ((c.b - average.b) * (c.b - average.b));
                sumA += ((c.a - average.a) * (c.a - average.a));
                ++count;
            }
        }

        double scale = 1.0 / count;
        var r = (byte)Mathf.Clamp((int)(scale * sumR), 0, 255);
        var g = (byte)Mathf.Clamp((int)(scale * sumG), 0, 255);
        var b = (byte)Mathf.Clamp((int)(scale * sumB), 0, 255);
        var a = (byte)Mathf.Clamp((int)(scale * sumA), 0, 255);
        return new Color32(r, g, b, a);
    }

    public Color32 CalcVariance()
    {
        return this.CalcVariance(0, 0, _width, _height, this.CalcAverage());
    }

    public Color32 CalcVariance(Color32 average)
    {
        return this.CalcVariance(0, 0, _width, _height, average);
    }

    public Color32 CalcStandardDeviation(int startX, int startY, int sizeX, int sizeY)
    {
        var average = this.CalcAverage(startX, startY, sizeX, sizeY);
        return this.CalcStandardDeviation(startX, startY, sizeX, sizeY, average);
    }

    public Color32 CalcStandardDeviation(int startX, int startY, int sizeX, int sizeY, Color32 average)
    {
        var v = this.CalcVariance(startX, startY, sizeX, sizeY, average);
        var r = (byte)Mathf.Clamp((int)Mathf.Sqrt(v.r), 0, 255);
        var g = (byte)Mathf.Clamp((int)Mathf.Sqrt(v.g), 0, 255);
        var b = (byte)Mathf.Clamp((int)Mathf.Sqrt(v.b), 0, 255);
        var a = (byte)Mathf.Clamp((int)Mathf.Sqrt(v.a), 0, 255);
        return new Color32(r, g, b, a);
    }

    public Color32 CalcStandardDeviation()
    {
        return this.CalcStandardDeviation(0, 0, _width, _height);
    }

    public Color32 CalcStandardDeviation(Color32 average)
    {
        return this.CalcStandardDeviation(0, 0, _width, _height, average);
    }
}
