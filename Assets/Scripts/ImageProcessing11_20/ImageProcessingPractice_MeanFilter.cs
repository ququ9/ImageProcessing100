using System.Linq;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;

[ImageProcessingPractice(11, "平滑化フィルター")]
public class ImageProcessingPractice_MeanFilter : ImageProcessingPractice
{
    [SerializeField, Range(1, 11)] private int _kernelSize = 3;

    public override void OnProcess()
    {
        var scale = 1.0f / (_kernelSize * _kernelSize);
        var kernelText = string.Join("\n", Enumerable.Range(0, _kernelSize).Select(_ => string.Join(",", Enumerable.Range(0, _kernelSize).Select(__ => "1"))));

        var kernel = new ConvolutionKernel(kernelText, scale);

        var job = ApplyKernelJob.Create(_source, kernel);
        var handle = job.Schedule(job.Result.Length, 8);
        handle.Complete();
        
        using (var temp = new TextureData(_source.Width, _source.Height, job.Result)) {
            temp.ApplyToTexture(_result);
        }
    }
}