using UnityEngine;

[ImageProcessingPractice(8, "Maxプーリング")]
public class ImageProcessingPractice_MaxPooling : ImageProcessingPractice
{
    [SerializeField, Range(1, 512)] private int _cellSize = 8;

    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            for (var y = 0; y < _source.Height; y += _cellSize) {
                for (var x = 0; x < _source.Width; x += _cellSize) {
                    var max = this.CalcMax(x, y, _cellSize, _cellSize);
                    temp.FillPixels(x, y, _cellSize, _cellSize, max);
                }
            }
            temp.ApplyToTexture(_result);
        }
    }

    private Color32 CalcMax(int startX, int startY, int sizeX, int sizeY)
    {
        var endX = Mathf.Min(_source.Width - 1, startX + sizeX - 1);
        var endY = Mathf.Min(_source.Height - 1, startY + sizeY - 1);

        Color32 max = new Color32(byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue);
        for (var y = startY; y <= endY; ++y) {
            for (var x = startX; x <= endX; ++x) {
                var c = _source.GetPixel(x, y);
                max.r = (byte)Mathf.Max(max.r, c.r);
                max.g = (byte)Mathf.Max(max.g, c.g);
                max.b = (byte)Mathf.Max(max.b, c.b);
                max.a = (byte)Mathf.Max(max.a, c.a);
            }
        }
        return max;
    }
}
