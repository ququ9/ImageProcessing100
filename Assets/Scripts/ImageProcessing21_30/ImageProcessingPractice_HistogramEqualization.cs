using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(23, "ヒストグラム平坦化")]
public class ImageProcessingPractice_HistogramEqualization : ImageProcessingPractice
{
    public override void OnProcess()
    {
        var frequency = new int[3, 256];
        var max = new Color32(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);
        foreach (var (x, y) in _source) {
            var t = _source.GetPixel(x, y);
            frequency[0, t.r]++;
            frequency[1, t.g]++;
            frequency[2, t.b]++;
        }

        double scale = 255 / (double)(_source.Width * _source.Height);

        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var t = _source.GetPixel(x, y);

                var sumR = 0;
                for (var i = 0; i < t.r; ++i) {
                    sumR += frequency[0, i];
                }

                var sumG = 0;
                for (var i = 0; i < t.g; ++i) {
                    sumG += frequency[1, i];
                }

                var sumB = 0;
                for (var i = 0; i < t.b; ++i) {
                    sumB += frequency[2, i];
                }

                t.r = (byte)Mathf.Clamp((int)(scale * sumR), 0, 255);
                t.g = (byte)Mathf.Clamp((int)(scale * sumG), 0, 255);
                t.b = (byte)Mathf.Clamp((int)(scale * sumB), 0, 255);

                temp.SetPixel(x, y, t);
            }
            temp.ApplyToTexture(_result);
        }
    }
}