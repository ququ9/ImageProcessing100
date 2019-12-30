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

        var job = new ImageProcessJob
        {
          Pixels = _source.Pixels,
          Width = _source.Width,
          Height = _source.Height,
          Kernel = new NativeArray<float>(kernel.Weights.ToArray(), Allocator.TempJob),
          KernelSize = _kernelSize,
          Result = new NativeArray<Color32>((_source.Width * _source.Height), Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
        };

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

    private struct ImageProcessJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Color32> Pixels;

        [ReadOnly, DeallocateOnJobCompletion]
        public NativeArray<float> Kernel;

        [WriteOnly]
        public NativeArray<Color32> Result;

        public int KernelSize;
        public int Width;
        public int Height;

        public void Execute(int index)
        {
            var pixelY = (index / this.Width);
            var pixelX = (index - (pixelY * this.Width));
            var kernelCenter = this.KernelSize / 2;

            float r = 0.0f, g = 0.0f, b = 0.0f;
            for (var iKernel = 0; iKernel < Kernel.Length; ++iKernel) {
                var weight = this.Kernel[iKernel];

                var yKernel = (iKernel / KernelSize);
                var xKernel = (iKernel - (yKernel * KernelSize));

                var x = Mathf.Clamp(pixelX + xKernel - kernelCenter, 0, this.Width - 1);
                var y = Mathf.Clamp(pixelY + yKernel - kernelCenter, 0, this.Height - 1);
                var c = this.Pixels[(y * this.Width) + x];
                r += (weight * c.r);
                g += (weight * c.g);
                b += (weight * c.b);
            }

            var pixelIndex = ((pixelY * this.Width) + pixelX);
            var o = this.Pixels[pixelIndex];
            this.Result[pixelIndex] = new Color32((byte)Mathf.Clamp((byte)r, 0, 255) , (byte)Mathf.Clamp((byte)g, 0, 255), (byte)Mathf.Clamp((byte)b, 0, 255), o.a);
        }
    }
}
