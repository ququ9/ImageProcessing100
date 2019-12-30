using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[ImageProcessingPractice(4, "大津の2値化")]
public class ImageProcessingPractice_ToBinaryOtsu : ImageProcessingPractice
{
    private const int MaxQuantization = 256;

    public override void OnProcess()
    {
        var job = new ImageProcessJob();
        job.Pixels = _source.Pixels;
        job.Width = _source.Width;
        job.Height = _source.Height;
        job.ResultVariance = new NativeArray<float>(MaxQuantization, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

        var handle = job.Schedule(MaxQuantization, 8);
        handle.Complete();

        var result = job.ResultVariance;
        using (var temp = TextureData.CreateTemporalGrayScaleFromRGB(_source)) {
            var maxVariance = float.MinValue;
            var maxVarianceIndex = 0;
            for (var i = 0; i < MaxQuantization; ++i) {
                var threshold = i;
                var variance = result[i];
                if (!(variance > maxVariance)) {
                    continue;
                }

                maxVariance = variance;
                maxVarianceIndex = i;
            }

            var resultThreshold = maxVarianceIndex;

            foreach (var (x, y) in _source) {
                var t = ColorUtility.RGBtoGrayScale(_source.GetPixel(x, y));
                var c = (t.r >= resultThreshold) ? new Color32(255, 255, 255, t.a) : new Color32(0, 0, 0, t.a);
                temp.SetPixel(x, y, c);
            }
            temp.ApplyToTexture(_result);
        }

        job.ResultVariance.Dispose();
    }

    private struct ImageProcessJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Color32> Pixels;

        [WriteOnly] public NativeArray<float> ResultVariance;

        [ReadOnly] public int Width;
        [ReadOnly] public int Height;

        public void Execute(int index)
        {
            var numClass0 = 0;
            var numClass1 = 0;
            var sumClass0 = 0.0f;
            var sumClass1 = 0.0f;
            var threshold = index;

            foreach (var p in this.Pixels) {
                var t = p.r;
                if (t < threshold) {
                    ++numClass0;
                    sumClass0 += t;
                } else {
                    ++numClass1;
                    sumClass1 += t;
                }
            }

            var aveClass0 = (numClass0 > 0) ? (float)(sumClass0 / numClass0) : 0.0f;
            var aveClass1 = (numClass1 > 0) ? (float)(sumClass1 / numClass1) : 0.0f;

            var invPixelCount = 1.0f / (this.Width * this.Height);
            var w0 = numClass0 * invPixelCount;
            var w1 = numClass1 * invPixelCount;

            ResultVariance[index] = w0 * w1 * (aveClass0 - aveClass1) * (aveClass0 - aveClass1);
        }
    }
}
