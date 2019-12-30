using UnityEngine;

[ImageProcessingPractice(3, "2値化")]
public class ImageProcessingPractice_ToBinary : ImageProcessingPractice
{
    [SerializeField, Range(0, 255)] private int _threshold = 128;

    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var t = ColorUtility.RGBtoGrayScale(_source.GetPixel(x, y));
                var c = (t.r >= _threshold) ? new Color32(255, 255, 255, t.a) : new Color32(0, 0, 0, t.a);
                temp.SetPixel(x, y, c);
            }
            temp.ApplyToTexture(_result);
        }
    }
}
