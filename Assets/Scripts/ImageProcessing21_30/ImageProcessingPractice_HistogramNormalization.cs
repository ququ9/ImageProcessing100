using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(21, "ヒストグラム正規化")]
public class ImageProcessingPractice_HistogramNormalization : ImageProcessingPractice
{
    [SerializeField, Range(0, 254)] private int _min = 0;
    [SerializeField, Range(1, 255)] private int _max = 255;

    public override void OnProcess()
    {
        var min = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 255);
        var max = new Color32(byte.MinValue, byte.MinValue, byte.MinValue, 255);
        foreach (var (x, y) in _source) {
            var t = _source.GetPixel(x, y);
            min = ColorUtility.MinRGB(ref t, ref min);
            max = ColorUtility.MaxRGB(ref t, ref max);
        }

        var scaleR = (_max - _min) / (float)(max.r - min.r);
        var scaleG = (_max - _min) / (float)(max.g - min.g);
        var scaleB = (_max - _min) / (float)(max.b - min.b);
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var t = _source.GetPixel(x, y);

                if (t.r < min.r) {
                    t.r = (byte)_min;
                } else if (t.r > max.r) {
                    t.r = (byte)_max;
                } else {
                    t.r = (byte)Mathf.Clamp(scaleR * (t.r - min.r) + _min, 0, 255);
                }

                if (t.g < min.g) {
                    t.g = (byte)_min;
                } else if (t.g > max.g) {
                    t.g = (byte)_max;
                } else {
                    t.g = (byte)Mathf.Clamp(scaleG * (t.g - min.g) + _min, 0, 255);
                }

                if (t.b < min.b) {
                    t.b = (byte)_min;
                } else if (t.b > max.b) {
                    t.b = (byte)_max;
                } else {
                    t.b = (byte)Mathf.Clamp(scaleB * (t.b - min.b) + _min, 0, 255);
                }

                temp.SetPixel(x, y, t);
            }
            temp.ApplyToTexture(_result);
        }
    }
}