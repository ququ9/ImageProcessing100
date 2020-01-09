using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(24, "ガンマ補正")]
public class ImageProcessingPractice_GammaCorrection : ImageProcessingPractice
{
    [SerializeField, Range(0.01f, 20)] private float _gamma = 2.2f;
    [SerializeField, Range(0.01f, 10)] private float _coefficient = 1.0f;

    public override void OnProcess()
    {
        if (_coefficient <= 0.0f) {
            Debug.LogError($"{nameof(_coefficient)} is negative, set positive value");
            return;
        }

        if (_gamma <= 0.0f) {
            Debug.LogError($"{nameof(_gamma)} is negative, set positive value");
            return;
        }

        var invCoefficient = 1.0f / _coefficient;
        var invGamma = 1.0f / _gamma;
        var inv255 = 1.0f / 255.0f;

        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var t = _source.GetPixel(x, y);

                t.r = (byte)Mathf.Clamp((int)(Mathf.Pow(invCoefficient * t.r * inv255, invGamma) * 255), 0, 255);
                t.g = (byte)Mathf.Clamp((int)(Mathf.Pow(invCoefficient * t.g * inv255, invGamma) * 255), 0, 255);
                t.b = (byte)Mathf.Clamp((int)(Mathf.Pow(invCoefficient * t.b * inv255, invGamma) * 255), 0, 255);

                temp.SetPixel(x, y, t);
            }
            temp.ApplyToTexture(_result);
        }
    }
}