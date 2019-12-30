using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(5, "HSV変換")]
public class ImageProcessingPractice_HSV : ImageProcessingPractice
{
    [SerializeField, Range(0, 360)] private int _rotation = 0;

    private TextureData _source;

    public override void OnProcess(TextureData source)
    {
        _source = source;

        var hsv = TextureData.CreateTemporalHSVFromRGB(source);

        foreach (var (x, y) in source) {
            var idx = (y * source.Width) + x;
            var t = hsv[idx];
            t.h = (_rotation + t.h + 360) % 360;

            hsv[idx] = t;
        }

        using (var temp = TextureData.CreateTemporalRGBFromHSV(hsv, source.Width, source.Height)) {
            temp.ApplyToTexture(_result);
        }
        hsv.Dispose();
    }

    private void OnValidate()
    {
        if (_source != null) {
            this.OnProcess(_source);
        }
    }
}
