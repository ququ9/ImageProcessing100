using UnityEngine;
using UnityEngine.UI;

[ImageProcessingPractice(30, "回転")]
public class ImageProcessingPractice_Rotation : ImageProcessingPractice
{
    [SerializeField] private int _translationX;
    [SerializeField] private int _translationY;
    [SerializeField] private float _rotation = 30.0f;

    public override void OnProcess()
    {
        var c = Mathf.Cos(Mathf.Deg2Rad * _rotation);
        var s = Mathf.Sin(Mathf.Deg2Rad * _rotation);

        var det = (c * c) - (-s * s);
        if (det == 0.0f) {
            Debug.LogError($"determinant is zero");
            return;
        }

        var mat = new float[2, 2]
        {
            { c / det, -s / det },
            { s / det, c / det },
        };

        var centerX = _source.Width / 2;
        var centerY = _source.Height / 2;

        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var xx = (int)((x - centerX - _translationX) * mat[0, 0] + (y - centerY - _translationY) * mat[0, 1]) + centerX;
                var yy = (int)((x - centerX - _translationX) * mat[1, 0] + (y - centerY - _translationY) * mat[1, 1]) + centerY;

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