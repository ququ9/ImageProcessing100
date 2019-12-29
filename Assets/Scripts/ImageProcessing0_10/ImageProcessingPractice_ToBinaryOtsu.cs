using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(4, "大津の2値化")]
public class ImageProcessingPractice_ToBinaryOtsu : ImageProcessingPractice
{
    public override void OnProcess(TextureData source)
    {
        using (var temp = TextureData.CreateTemporalGrayScaleFromRGB(source)) {
            var maxVariance = float.MinValue;
            var maxVarianceIndex = 0;
            for (var i = 0; i < 255; ++i) {
                var threshold = i;
                var variance = this.CalcVariance(temp, threshold);
                if (!(variance > maxVariance)) {
                    continue;
                }

                maxVariance = variance;
                maxVarianceIndex = i;
            }

            var resultThreshold = maxVarianceIndex;

            var black = new Color32(0, 0, 0, 255);
            var white = new Color32(255, 255, 255, 255);
            foreach (var (x, y) in source) {
                var t = ColorUtility.RGBtoGrayScale(source.GetPixel(x, y));
                var c = (t.r >= resultThreshold) ? new Color32(255, 255, 255, t.a) : new Color32(0, 0, 0, t.a);
                temp.SetPixel(x, y, c);
            }
            temp.ApplyToTexture(_result);
        }
    }

    private float CalcVariance(TextureData grayscaleImage, int threshold)
    {
        var numClass0 = 0;
        var numClass1 = 0;
        var sumClass0 = 0.0f;
        var sumClass1 = 0.0f;

        foreach (var (x, y) in grayscaleImage) {
            var t = (int)grayscaleImage.GetPixel(x, y).r;
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

        var invPixelCount = 1.0f / (grayscaleImage.Width * grayscaleImage.Height);
        var w0 = numClass0 * invPixelCount;
        var w1 = numClass1 * invPixelCount;

        return w0 * w1 * (aveClass0 - aveClass1) * (aveClass0 - aveClass1);
    }
}
