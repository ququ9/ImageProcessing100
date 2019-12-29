using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(3, "2値化")]
public class ImageProcessingPractice_ToBinary : ImageProcessingPractice
{
    [SerializeField, Range(0, 255)] private int _threshold = 128;

    private TextureData _source;

    public override void OnProcess(TextureData source)
    {
        _source = source;

        var black = new Color32(0, 0, 0, 255);
        var white = new Color32(255, 255, 255, 255);
        using (var temp = TextureData.CreateTemporal(source.Width, source.Height)) {
            foreach (var (x, y) in source) {
                var t = ColorUtility.RGBtoGrayScale(source.GetPixel(x, y));
                var c = (t.r >= _threshold) ? new Color32(255, 255, 255, t.a) : new Color32(0, 0, 0, t.a);
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
