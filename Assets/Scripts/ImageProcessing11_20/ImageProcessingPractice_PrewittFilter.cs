using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(16, "Prewittフィルター")]
public class ImageProcessingPractice_PrewittFilter : ImageProcessingPractice
{
    private enum Direction
    {
        Horizontal, Vertical
    }

    [SerializeField] private Direction _direction = Direction.Horizontal;

    public override void OnProcess()
    {
        var kernel = this.CreateKernel(_direction);

        using (var grayScale = TextureData.CreateGrayScaleFromRGB(_source, isTemporal: false)) {
            var job = ApplyKernelJob.Create(grayScale, kernel);
            var handle = job.Schedule(job.Result.Length, 8);
            handle.Complete();
        
            using (var temp = new TextureData(_source.Width, _source.Height, job.Result)) {
                temp.ApplyToTexture(_result);
            }
        }
    }

    private ConvolutionKernel CreateKernel(Direction direction)
    {
        if (direction == Direction.Horizontal) {
            return new ConvolutionKernel("-1,-1,-1\n0,0,0\n1,1,1");
        }
        return new ConvolutionKernel("-1,0,1\n-1,0,1\n-1,0,1");
    }
}