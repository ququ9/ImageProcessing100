using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(10, "メディアンフィルター")]
public class ImageProcessingPractice_MedianFilter : ImageProcessingPractice
{
    [SerializeField, Range(3, 11)] private int _kernelSize = 3;

    private readonly List<int> _sortedR = new List<int>();
    private readonly List<int> _sortedG = new List<int>();
    private readonly List<int> _sortedB = new List<int>();

    public override void OnProcess()
    {
        var kernel = new ConvolutionKernel(_kernelSize);
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var c = this.CalcMedian(kernel, x, y);
                temp.SetPixel(x, y, c);
            }
            temp.ApplyToTexture(_result);
        }
    }

    private Color32 CalcMedian(ConvolutionKernel kernel, int pixelX, int pixelY)
    {
        _sortedR.Clear();
        _sortedG.Clear();
        _sortedB.Clear();

        foreach (var (x, y, ox, oy) in kernel) {
            var cx = pixelX + ox;
            var cy = pixelY + oy;
            var c = _source.GetPixelSafe(cx, cy);
            _sortedR.Add(c.r);
            _sortedG.Add(c.g);
            _sortedB.Add(c.b);
        }

        _sortedR.Sort();
        _sortedG.Sort();
        _sortedB.Sort();

        var medianIndex = ((_sortedR.Count / 2) + 1);
        if (medianIndex >= _sortedB.Count) {
            medianIndex = (_sortedR.Count / 2);
        }

        var r = _sortedR[medianIndex];
        var g = _sortedG[medianIndex];
        var b = _sortedB[medianIndex];
        return new Color32((byte)r, (byte)g, (byte)b, _source.GetPixel(pixelX, pixelY).a);
    }
}
