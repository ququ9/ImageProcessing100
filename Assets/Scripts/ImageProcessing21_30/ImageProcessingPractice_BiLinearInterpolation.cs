using UnityEngine;

[ImageProcessingPractice(26, "バイリニア補間")]
public class ImageProcessingPractice_BiLinearInterpolation : ImageProcessingPractice
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
                var fv = y * _invScale;;
                var v = Mathf.Clamp((int)Mathf.Floor(fv), 0, _source.Height - 1);
                var dy = fv - v;
                for (var x = 0; x < _width; ++x) {
                    var fu = x * _invScale;
                    var u = Mathf.Clamp((int)Mathf.Floor(fu), 0, _source.Width - 1);
                    var dx = fu - u;

                    var v00 = _source.GetPixel(u, v);
                    var v10 = _source.GetPixelSafe(u + 1, v);
                    var v01 = _source.GetPixelSafe(u, v + 1);
                    var v11 = _source.GetPixelSafe(u + 1, v + 1);

                    var w00 = (1.0f - dx) * (1.0f - dy);
                    var w10 = dx * (1.0f - dy);
                    var w01 = (1.0f - dx) * dy;
                    var w11 = dx * dy;

                    var r = Mathf.Clamp((int)(w00 * v00.r + w10 * v10.r + w01 * v01.r + w11 * v11.r), 0, 255);
                    var g = Mathf.Clamp((int)(w00 * v00.g + w10 * v10.g + w01 * v01.g + w11 * v11.g), 0, 255);
                    var b = Mathf.Clamp((int)(w00 * v00.b + w10 * v10.b + w01 * v01.b + w11 * v11.b), 0, 255);
                    var a = Mathf.Clamp((int)(w00 * v00.a + w10 * v10.a + w01 * v01.a + w11 * v11.a), 0, 255);

                    temp.SetPixel(x, y, new Color32((byte)r, (byte)g, (byte)b, (byte)a));
                }
            }
            temp.ApplyToTexture(_result);
        }
    }
}