using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class ConvolutionKernel : IEnumerable<(int X, int Y, int OffsetX, int OffsetY)>
{
    private readonly int _kernelSize;
    private readonly int _kernelCenter;
    private readonly float[] _kernel;

    public IEnumerable<float> Weights => _kernel;

    public ConvolutionKernel(string kernel, float scale = 1.0f)
    {
        var rows = kernel.Split('\n');
        var columns = rows.Select(row => row.Split(',')).ToArray();

        var columnSize = columns.Select(c => c.Length).Max();
        _kernelSize = rows.Length * columnSize;
        _kernelCenter = _kernelSize / 2;
        _kernel = new float[_kernelSize * _kernelSize];

        if ((_kernelSize & 0x01) == 0) {
            Debug.LogWarning($"Kernel Size is even");
        }

        for (var r = 0; r < rows.Length; ++r) {
            var column = columns[r];
            for (var c = 0; c < columnSize; ++c) {
                if (c < column.Length) {
                    _kernel[(columnSize * r) + c] = float.Parse(column[c].Trim()) * scale;
                } else {
                    // そもそも不正な形式ではあるが一応0で埋めておく
                    _kernel[(columnSize * r) + c] = 0.0f;
                }
            }
        }
    }

    public ConvolutionKernel(int kernelSize)
    {
        _kernelSize = kernelSize;
        _kernelCenter = _kernelSize / 2;
        _kernel = new float[kernelSize * kernelSize];
    }

    public Color32 Apply(int pixelX, int pixelY, TextureData source)
    {
        float r = 0.0f, g = 0.0f, b = 0.0f;
        foreach (var (x, y, kx, ky) in this) {
            var cx = pixelX + kx;
            var cy = pixelY + ky;
            var c = source.GetPixelSafe(cx, cy);
            var weight = _kernel[(y * _kernelSize) + x];
            r += (weight * c.r);
            g += (weight * c.g);
            b += (weight * c.b);
        }
        var o = source.GetPixel(pixelX, pixelY);
        return new Color32((byte)Mathf.Clamp((byte)r, 0, 255) , (byte)Mathf.Clamp((byte)g, 0, 255), (byte)Mathf.Clamp((byte)b, 0, 255), o.a);
    }

    public void SetWeight(int kernelCoordinateX, int kernelCoordinateY, float weight)
    {
        var x = kernelCoordinateX + _kernelCenter;
        var y = kernelCoordinateY + _kernelCenter;
        var index = ((y * _kernelSize) + x);
        _kernel[index] = weight;
    }

    public float GetWeight(int kernelCoordinateX, int kernelCoordinateY)
    {
        var x = kernelCoordinateX + _kernelCenter;
        var y = kernelCoordinateY + _kernelCenter;
        var index = ((y * _kernelSize) + x);
        return _kernel[index];
    }

    public IEnumerator<(int X, int Y, int OffsetX, int OffsetY)> GetEnumerator()
    {
        return new ConvolutionKernelEnumerator(_kernelSize);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
