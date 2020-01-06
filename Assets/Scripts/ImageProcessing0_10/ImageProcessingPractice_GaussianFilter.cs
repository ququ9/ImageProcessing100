using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[ImageProcessingPractice(9, "ガウシアンフィルター")]
public class ImageProcessingPractice_GaussianFilter : ImageProcessingPractice
{
    [SerializeField, Range(1, 10)] private int _kernelSize = 3;
    [SerializeField] private float _standardDeviation = 1.3f;

    public override void OnProcess()
    {
        var variance = (_standardDeviation * _standardDeviation);
        var kernel = this.CreateGaussianKernel(_kernelSize, variance);

        var job = ApplyKernelJob.Create(_source, kernel);
        var handle = job.Schedule(job.Result.Length, 8);
        handle.Complete();

        using (var temp = new TextureData(_source.Width, _source.Height, job.Result)) {
            temp.ApplyToTexture(_result);
        }
    }

    private ConvolutionKernel CreateGaussianKernel(int cellSize, float variance)
    {
        const float invPi = 1.0f / Mathf.PI;
        var invVariance = 1.0f / variance;

        var kernel = new ConvolutionKernel(cellSize);
        var sum = 0.0f;
        foreach (var (x, y, offsetX, offsetY) in kernel) {
            var weight = (0.5f * invPi * invVariance) * Mathf.Exp(-0.5f * invVariance * ((offsetX * offsetX) + (offsetY * offsetY)));
            kernel.SetWeight(offsetX, offsetY, weight);
            sum += weight;

        }

        var scale = 1.0f / sum;
        foreach (var (x, y, offsetX, offsetY) in kernel) {
            kernel.SetWeight(offsetX, offsetY, scale * kernel.GetWeight(offsetX, offsetY));
        }

        return kernel;
    }
}
