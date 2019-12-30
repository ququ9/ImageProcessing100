using UnityEngine;

[ImageProcessingPractice(6, "減色")]
public class ImageProcessingPractice_ReduceColor : ImageProcessingPractice
{
    [SerializeField, Range(1, 255)] private int _quantizeSize = 1;

    public override void OnProcess()
    {
        var size = Mathf.Clamp(_quantizeSize, 1, 255);
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var c = _source.GetPixel(x, y);
                c.r = (byte)(size * (c.r / size));
                c.g = (byte)(size * (c.g / size));
                c.b = (byte)(size * (c.b / size));
                temp.SetPixel(x, y, c);
            }
            temp.ApplyToTexture(_result);
        }
    }
}
