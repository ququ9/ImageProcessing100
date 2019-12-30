using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(6, "減色")]
public class ImageProcessingPractice_ReduceColor : ImageProcessingPractice
{
    [SerializeField, Range(1, 255)] private int _quantizeSize = 1;

    private TextureData _source;

    public override void OnProcess(TextureData source)
    {
        _source = source;

        var size = Mathf.Clamp(_quantizeSize, 1, 255);
        using (var temp = TextureData.CreateTemporal(source.Width, source.Height)) {
            foreach (var (x, y) in source) {
                var c = source.GetPixel(x, y);
                c.r = (byte)(size * (c.r / size));
                c.g = (byte)(size * (c.g / size));
                c.b = (byte)(size * (c.b / size));
                temp.SetPixel(x, y, c);
            }
            temp.ApplyToTexture(_result);
        }
    }

    private void OnValidate()
    {
        if (_source != null) {
            this.OnProcess(_source);
        }
    }
}
