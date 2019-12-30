using UnityEngine;

[ImageProcessingPractice(7, "平均プーリング")]
public class ImageProcessingPractice_AveragePooling : ImageProcessingPractice
{
    [SerializeField, Range(1, 512)] private int _cellSize = 8;

    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            for (var y = 0; y < _source.Height; y += _cellSize) {
                for (var x = 0; x < _source.Width; x += _cellSize) {
                    var average = _source.CalcAverage(x, y, _cellSize, _cellSize);
                    temp.FillPixels(x, y, _cellSize, _cellSize, average);
                }
            }
            temp.ApplyToTexture(_result);
        }
    }
}
