using UnityEngine;
using UnityEngine.UI;

[ImageProcessingPractice(29, "拡大縮小")]
public class ImageProcessingPractice_Scaling : ImageProcessingPractice
{
    [SerializeField] private int _translationX;
    [SerializeField] private int _translationY;
    [SerializeField] private float _scaleX = 1.3f;
    [SerializeField] private float _scaleY = 0.7f;

    public override void OnProcess()
    {
        var det = (_scaleX * _scaleY);
        if (det == 0.0f) {
            Debug.LogError($"determinant is zero");
            return;
        }

        var mat = new float[2, 2]
        {
            { _scaleY / det, 0.0f },
            { 0.0f, _scaleX / det },
        };

        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {

                var xx = (int)(x * mat[0, 0] + y * mat[0, 1]) - _translationX;
                var yy = (int)(x * mat[1, 0] + y * mat[1, 1]) - _translationY;
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