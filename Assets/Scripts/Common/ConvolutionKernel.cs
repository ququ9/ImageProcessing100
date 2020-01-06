using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class ConvolutionKernel : IEnumerable<(int X, int Y, int OffsetX, int OffsetY)>
{
    private readonly int _kernelWidth;
    private readonly int _kernelHeight;
    private readonly int _kernelCenterX;
    private readonly int _kernelCenterY;
    private readonly float[] _kernel;

    public IEnumerable<float> Weights => _kernel;

    public ConvolutionKernel(string kernel, float scale = 1.0f)
    {
        var rows = kernel.Split('\n');
        _kernelHeight = rows.Length;
        _kernelCenterY = _kernelHeight / 2;

        var columns = rows.Select(row => row.Split(',')).ToArray();
        _kernelWidth = columns.Select(c => c.Length).Max();
        _kernelCenterX = _kernelWidth / 2;

        _kernel = new float[_kernelWidth * _kernelHeight];

        for (var r = 0; r < _kernelHeight; ++r) {
            var column = columns[r];
            for (var c = 0; c < _kernelWidth; ++c) {
                if (c < column.Length) {
                    _kernel[(_kernelWidth * r) + c] = float.Parse(column[c].Trim()) * scale;
                } else {
                    // そもそも不正な形式ではあるが一応0で埋めておく
                    _kernel[(_kernelWidth * r) + c] = 0.0f;
                }
            }
        }
    }

    public ConvolutionKernel(int kernelSize)
    {
        _kernelWidth = _kernelHeight = kernelSize;
        _kernelCenterX = _kernelCenterY = kernelSize / 2;
        _kernel = new float[kernelSize * kernelSize];
    }

    public Color32 Apply(int pixelX, int pixelY, TextureData source)
    {
        float r = 0.0f, g = 0.0f, b = 0.0f;
        foreach (var (x, y, kx, ky) in this) {
            var cx = pixelX + kx;
            var cy = pixelY + ky;
            var c = source.GetPixelSafe(cx, cy);
            var weight = _kernel[(y * _kernelWidth) + x];
            r += (weight * c.r);
            g += (weight * c.g);
            b += (weight * c.b);
        }
        var o = source.GetPixel(pixelX, pixelY);
        return new Color32((byte)Mathf.Clamp((int)r, 0, 255) , (byte)Mathf.Clamp((int)g, 0, 255), (byte)Mathf.Clamp((int)b, 0, 255), o.a);
    }

    public void SetWeight(int kernelCoordinateX, int kernelCoordinateY, float weight)
    {
        var x = kernelCoordinateX + _kernelCenterX;
        var y = kernelCoordinateY + _kernelCenterY;
        var index = ((y * _kernelWidth) + x);
        _kernel[index] = weight;
    }

    public float GetWeight(int kernelCoordinateX, int kernelCoordinateY)
    {
        var x = kernelCoordinateX + _kernelCenterX;
        var y = kernelCoordinateY + _kernelCenterY;
        var index = ((y * _kernelWidth) + x);
        return _kernel[index];
    }

    public IEnumerator<(int X, int Y, int OffsetX, int OffsetY)> GetEnumerator()
    {
        return new ConvolutionKernelEnumerator(_kernelWidth, _kernelHeight);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
