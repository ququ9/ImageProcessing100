using System.Linq;
using UnityEngine;

[ImageProcessingPractice(11, "平滑化フィルター")]
public class ImageProcessingPractice_MeanFilter : ImageProcessingPractice
{
    [SerializeField, Range(1, 11)] private int _kernelSize = 3;

    public override void OnProcess()
    {
        var scale = 1.0f / (_kernelSize * _kernelSize);
        var kernelText = string.Join("\n", Enumerable.Range(0, _kernelSize).Select(_ => string.Join(",", Enumerable.Range(0, _kernelSize).Select(__ => "1"))));

        var kernel = new ConvolutionKernel(kernelText, scale);
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                temp.SetPixel(x, y, kernel.Apply(x, y, _source));
            }
            temp.ApplyToTexture(_result);
        }
    }
}
