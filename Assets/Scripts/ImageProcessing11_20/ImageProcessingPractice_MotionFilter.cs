using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(12, "モーションフィルター")]
public class ImageProcessingPractice_MotionFilter : ImageProcessingPractice
{
    [SerializeField, Range(1, 11)] private int _kernelSize = 3;

    public override void OnProcess()
    {
        var scale = 1.0f / (_kernelSize);
        var kernelText = string.Join("\n", Enumerable.Range(0, _kernelSize).Select(i => string.Join(",", Enumerable.Range(0, _kernelSize).Select(j => (i == j) ? "1" : "0"))));

        var kernel = new ConvolutionKernel(kernelText, scale);

        var job = ApplyKernelJob.Create(_source, kernel);
        var handle = job.Schedule(job.Result.Length, 8);
        handle.Complete();
        
        using (var temp = new TextureData(_source.Width, _source.Height, job.Result)) {
            temp.ApplyToTexture(_result);
        }
    }
}