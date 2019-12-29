using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(2, "グレースケール変換")]
public class ImageProcessingPractice_ToGrayScale : ImageProcessingPractice
{
    public override void OnProcess(TextureData source)
    {
        using (var temp = TextureData.CreateTemporalGrayScaleFromRGB(source)) { 
            temp.ApplyToTexture(_result);
        }
    }
}
