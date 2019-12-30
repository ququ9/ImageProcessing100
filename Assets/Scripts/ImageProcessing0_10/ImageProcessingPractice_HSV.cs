using UnityEngine;

[ImageProcessingPractice(5, "HSV変換")]
public class ImageProcessingPractice_HSV : ImageProcessingPractice
{
    [SerializeField, Range(0, 360)] private int _rotation = 0;

    public override void OnProcess()
    {
        var hsv = TextureData.CreateTemporalHSVFromRGB(_source);

        foreach (var (x, y) in _source) {
            var idx = (y * _source.Width) + x;
            var t = hsv[idx];
            t.h = (_rotation + t.h + 360) % 360;

            hsv[idx] = t;
        }

        using (var temp = TextureData.CreateTemporalRGBFromHSV(hsv, _source.Width, _source.Height)) {
            temp.ApplyToTexture(_result);
        }
        hsv.Dispose();
    }
}
