using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(22, "ヒストグラム操作")]
public class ImageProcessingPractice_HistogramTransform : ImageProcessingPractice
{
    [SerializeField, Range(0, 255)] private float _targetStandardDeviation = 52;
    [SerializeField, Range(0, 255)] private float _targetAverage = 128;

    public override void OnProcess()
    {
        var average = _source.CalcAverage(0, 0, _source.Width, _source.Height);
        var standardDeviation = _source.CalcStandardDeviation(average);
        var scaleR = _targetStandardDeviation / (float)standardDeviation.r;
        var scaleG = _targetStandardDeviation / (float)standardDeviation.g;
        var scaleB = _targetStandardDeviation / (float)standardDeviation.b;

        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var t = _source.GetPixel(x, y);

                t.r = (byte)Mathf.Clamp((int)(scaleR * (t.r - average.r) + _targetAverage), 0, 255);
                t.g = (byte)Mathf.Clamp((int)(scaleG * (t.g - average.g) + _targetAverage), 0, 255);
                t.b = (byte)Mathf.Clamp((int)(scaleB * (t.b - average.b) + _targetAverage), 0, 255);

                temp.SetPixel(x, y, t);
            }
            temp.ApplyToTexture(_result);
        }
    }
}