using UnityEngine;
using UnityEngine.UI;

[ImageProcessingPractice(28, "平行移動")]
public class ImageProcessingPractice_Translation : ImageProcessingPractice
{
    [SerializeField] private int _translationX;
    [SerializeField] private int _translationY;

    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {

                var xx = x - _translationX;
                var yy = y - _translationY;
                if (_source.IsValidCoordinate(xx, yy)) {
                    temp.SetPixel(x, y, _source.GetPixel(xx, yy));
                } else {
                    temp.SetPixel(x, y, new Color32(0, 0, 0, 255));
                }
            }
            temp.ApplyToTexture(_result);
        }
    }
}