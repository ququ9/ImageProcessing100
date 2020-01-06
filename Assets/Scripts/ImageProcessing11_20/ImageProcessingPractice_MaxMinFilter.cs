using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(13, "MAX-MINフィルター")]
public class ImageProcessingPractice_MaxMinFilter : ImageProcessingPractice
{
    [SerializeField, Range(1, 11)] private int _kernelSize = 3;

    public override void OnProcess()
    {
        using (var grayScale = TextureData.CreateGrayScaleFromRGB(_source, isTemporal: false)) {
            var job = ImageProcessingJob.Create(grayScale, _kernelSize);
            var handle = job.Schedule(job.Result.Length, 8);
            handle.Complete();

            using (var temp = new TextureData(_source.Width, _source.Height, job.Result)) {
                temp.ApplyToTexture(_result);
            }
        }
    }

    public struct ImageProcessingJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Color32> Pixels;

        [WriteOnly] public NativeArray<Color32> Result;

        [ReadOnly] private int KernelSize;
        [ReadOnly] private int KernelLength;
        [ReadOnly] private int KernelCenter;
        [ReadOnly] private int Width;
        [ReadOnly] private int Height;

        public static ImageProcessingJob Create(TextureData source, int kernelSize)
        {
            var job = new ImageProcessingJob
            {
                Pixels = source.Pixels,
                Width = source.Width,
                Height = source.Height,
                KernelSize = kernelSize,
                KernelLength = kernelSize * kernelSize,
                KernelCenter = kernelSize / 2,
                Result = new NativeArray<Color32>((source.Width * source.Height), Allocator.TempJob, NativeArrayOptions.UninitializedMemory)
            };
            return job;
        }

        public void Execute(int index)
        {
            var pixelY = (index / this.Width);
            var pixelX = (index - (pixelY * this.Width));

            int min = char.MaxValue;
            int max = char.MinValue;
            for (var iKernel = 0; iKernel < this.KernelLength; ++iKernel) {
                var yKernel = (iKernel / this.KernelSize);
                var xKernel = (iKernel - (yKernel * this.KernelSize));

                var x = Mathf.Clamp(pixelX + xKernel - this.KernelCenter, 0, this.Width - 1);
                var y = Mathf.Clamp(pixelY + yKernel - this.KernelCenter, 0, this.Height - 1);
                var c = this.Pixels[(y * this.Width) + x];
                min = Mathf.Min(min, c.r);
                max = Mathf.Max(max, c.r);
            }

            var o = this.Pixels[index];
            var v = (byte)(max - min);
            this.Result[index] = new Color32(v, v, v, o.a);
        }
    }
}