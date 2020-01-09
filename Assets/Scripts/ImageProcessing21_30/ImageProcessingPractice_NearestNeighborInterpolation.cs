using UnityEngine;

[ImageProcessingPractice(25, "最近傍補間")]
public class ImageProcessingPractice_NearestNeighborInterpolation : ImageProcessingPractice
{
    private float _scale;
    private float _invScale;
    private int _width;
    private int _height;

    public override void CreateResultTexture(Texture2D source)
    {
        _scale = 1.5f;
        _invScale = 1.0f / _scale;
        _width = (int)(source.width * _scale);
        _height = (int)(source.height * _scale);

        _result = new Texture2D(_width, _height, TextureFormat.RGBA32, false, false);
    }

    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporal(_width, _height)) {
            for (var y = 0; y < _height; ++y) {
                var v = Mathf.Clamp((int)Mathf.Round(y * _invScale), 0, _source.Height - 1);
                for (var x = 0; x < _width; ++x) {
                    var u = Mathf.Clamp((int)Mathf.Round(x * _invScale), 0, _source.Width - 1);
                    temp.SetPixel(x, y, _source.GetPixel(u, v));
                }
            }
            temp.ApplyToTexture(_result);
        }
    }
}