using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(18, "Embossフィルター")]
public class ImageProcessingPractice_EmbossFilter : ImageProcessingPractice
{
    public override void OnProcess()
    {
        var kernel = new ConvolutionKernel("-2,-1,0\n-1,1,1\n0,1,2");

        using (var grayScale = TextureData.CreateGrayScaleFromRGB(_source, isTemporal: false)) {
            var job = ApplyKernelJob.Create(grayScale, kernel);
            var handle = job.Schedule(job.Result.Length, 8);
            handle.Complete();
        
            using (var temp = new TextureData(_source.Width, _source.Height, job.Result)) {
                temp.ApplyToTexture(_result);
            }
        }
    }
}