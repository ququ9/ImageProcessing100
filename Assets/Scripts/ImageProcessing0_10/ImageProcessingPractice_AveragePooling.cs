using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageProcessingPractice(7, "平均プーリング")]
public class ImageProcessingPractice_AveragePooling : ImageProcessingPractice
{
    [SerializeField, Range(1, 512)] private int _cellSize = 8;

    private TextureData _source;

    public override void OnProcess(TextureData source)
    {
        _source = source;

        using (var temp = TextureData.CreateTemporal(source.Width, source.Height)) {
            for (var y = 0; y < source.Height; y += _cellSize) {
                for (var x = 0; x < source.Width; x += _cellSize) {
                    var average = source.CalcAverage(x, y, _cellSize, _cellSize);
                    temp.FillPixels(x, y, _cellSize, _cellSize, average);
                }
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
