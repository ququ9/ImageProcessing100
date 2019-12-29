using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(1, "チャンネル入れ替え")]
public class ImageProcessingPractice_SwapChannel : ImageProcessingPractice
{
    [SerializeField] private bool _swapRG = true;
    [SerializeField] private bool _swapRB = false;
    [SerializeField] private bool _swapGB = false;

    private TextureData _source;

    public override void OnProcess(TextureData source)
    {
        _source = source;

        using (var temp = TextureData.CreateTemporal(source.Width, source.Height)) {
            foreach (var (x, y) in source) {
                var c = source.GetPixel(x, y);
                if (_swapRG) {
                    var t = c.g;
                    c.g = c.r;
                    c.r = t;
                }

                if (_swapRB) {
                    var t = c.b;
                    c.b = c.r;
                    c.r = t;
                }

                if (_swapGB) {
                    var t = c.g;
                    c.g = c.b;
                    c.b = t;
                }

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
