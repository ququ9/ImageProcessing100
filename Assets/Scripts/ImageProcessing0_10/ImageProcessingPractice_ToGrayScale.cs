using UnityEngine;

[ImageProcessingPractice(2, "グレースケール変換")]
public class ImageProcessingPractice_ToGrayScale : ImageProcessingPractice
{
    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporalGrayScaleFromRGB(_source)) { 
            temp.ApplyToTexture(_result);
        }
    }
}
