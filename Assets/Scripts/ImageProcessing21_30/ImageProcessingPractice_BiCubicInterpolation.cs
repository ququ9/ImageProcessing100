using UnityEngine;
using UnityEngine.UI;

[ImageProcessingPractice(27, "バイキュービック補間")]
public class ImageProcessingPractice_BiCubicInterpolation : ImageProcessingPractice
{
    private const float kWeightFactor = -1.0f;

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
        var wxy = new float[2, 4];

        using (var temp = TextureData.CreateTemporal(_width, _height)) {
            for (var y = 0; y < _height; ++y) {
                var fv = y * _invScale;;
                var v = Mathf.Clamp((int)Mathf.Floor(fv), 0, _source.Height - 1);
                wxy[1, 0] = this.CalcWeight(fv - (v - 1));
                wxy[1, 1] = this.CalcWeight(fv - (v));
                wxy[1, 2] = this.CalcWeight(fv - (v + 1));
                wxy[1, 3] = this.CalcWeight(fv - (v + 2));

                for (var x = 0; x < _width; ++x) {
                    var fu = x * _invScale;
                    var u = Mathf.Clamp((int)Mathf.Floor(fu), 0, _source.Width - 1);
                    wxy[0, 0] = this.CalcWeight(fu - (u - 1));
                    wxy[0, 1] = this.CalcWeight(fu - (u));
                    wxy[0, 2] = this.CalcWeight(fu - (u + 1));
                    wxy[0, 3] = this.CalcWeight(fu - (u + 2));

                    float r = 0, g = 0, b = 0, a = 0;
                    float sum = 0.0f;
                    for (var j = 0; j < 4; ++j) {
                        for (var i = 0; i < 4; ++i) {
                            var c = _source.GetPixelSafe(u + i - 2, v + j - 2);
                            var w = wxy[0, i] * wxy[1, j];

                            r += (c.r * w);
                            g += (c.g * w);
                            b += (c.b * w);
                            a += (c.a * w);

                            sum += w;
                        }
                    }

                    var scale = 1.0f / sum;
                    int ir = Mathf.Clamp((int)(scale * r), 0, 255);
                    int ig = Mathf.Clamp((int)(scale * g), 0, 255);
                    int ib = Mathf.Clamp((int)(scale * b), 0, 255);
                    int ia = Mathf.Clamp((int)(scale * a), 0, 255);

                    temp.SetPixel(x, y, new Color32((byte)ir, (byte)ig, (byte)ib, (byte)ia));
                }
            }
            temp.ApplyToTexture(_result);
        }
    }

    private float CalcWeight(float t)
    {
        var at = Mathf.Abs(t);
        if (at <= 1.0f) {
            return ((kWeightFactor + 2.0f) * (at * at * at)) - ((kWeightFactor + 3.0f) * at * at) + 1.0f;
        }
        
        if (at <= 2.0f) {
            return (kWeightFactor * (at * at * at)) - (5.0f * kWeightFactor * at * at) + (8.0f * kWeightFactor * at) - (4.0f * kWeightFactor);
        }
        return 0.0f;
    }
}