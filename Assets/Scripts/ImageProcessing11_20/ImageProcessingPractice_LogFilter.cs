using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(19, "LoGフィルター")]
public class ImageProcessingPractice_LogFilter : ImageProcessingPractice
{
    [SerializeField, Range(1, 10)] private int _kernelSize = 3;
    [SerializeField] private float _sigma = 3.0f;

    public override void OnProcess()
    {
        var kernel = this.CreateKernel(_kernelSize, _sigma);
        Debug.Log($"{string.Join(" ", kernel.Weights)}");

        using (var grayScale = TextureData.CreateGrayScaleFromRGB(_source, isTemporal: false)) {
            using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
                foreach (var (x, y) in temp) {
                    temp.SetPixel(x, y, this.Apply(x, y, grayScale, kernel));
                }
                temp.ApplyToTexture(_result);
            }
        }
    }

    private ConvolutionKernel CreateKernel(int kernelSize, float sigma)
    {
        var kernel = new ConvolutionKernel(kernelSize);
        var sigma2 = sigma * sigma;
        var sigma6 = sigma2 * sigma2 * sigma2;
        var c1 = 1.0f / (2.0f * Mathf.PI * sigma6);
        var c2 = -1.0f / (2.0f * sigma2);
        
        var sum = 0.0f;
        foreach (var (x, y, offsetX, offsetY) in kernel) {
            // (x^2 + y^2 - sigma^2) / (2 * pi * sigma^6) * exp(-(x^2+y^2) / (2*sigma^2))
            var r2 = (float)((offsetX * offsetX) + (offsetY * offsetY));
            var t = (r2 - sigma2) * c1 * Mathf.Exp(c2 * r2);
            kernel.SetWeight(offsetX, offsetY, t);
            sum += Mathf.Abs(t);
        }

        var scale = 1.0f / sum;
         foreach (var (x, y, offsetX, offsetY) in kernel) {
            kernel.SetWeight(offsetX, offsetY, scale * kernel.GetWeight(offsetX, offsetY));
        }
        return kernel;
    }

    private Color32 Apply(int pixelX, int pixelY, TextureData source, ConvolutionKernel kernel)
    {
        var sum = 0.0f;
        foreach (var (x, y, offsetX, offsetY) in kernel) {
            sum += (kernel.GetWeight(offsetX, offsetY) * source.GetPixelSafe(pixelX + offsetX, pixelY + offsetY).r);
        }

        var v = (byte)Mathf.Clamp(sum, 0, 255);
        return new Color(v, v, v, source.GetPixel(pixelX, pixelY).a);
    }
}