using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(17, "Laplacianフィルター")]
public class ImageProcessingPractice_LaplacianFilter : ImageProcessingPractice
{
    public override void OnProcess()
    {
        var kernel = new ConvolutionKernel("0,1,0\n1,-4,1\n0,1,0");

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