using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(0, "元画像の単純なコピー")]
public class ImageProcessingPractice_Copy : ImageProcessingPractice
{
    public override void OnProcess(TextureData source)
    {
        using (var temp = TextureData.CreateTemporal(source.Width, source.Height)) {
            foreach (var (x, y) in source) {
                temp.SetPixel(x, y, source.GetPixel(x, y));
            }
            temp.ApplyToTexture(_result);
        }
    }
}
