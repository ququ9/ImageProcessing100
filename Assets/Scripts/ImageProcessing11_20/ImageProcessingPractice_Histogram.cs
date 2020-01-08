using System.Linq;
using System.Text;
using UnityEngine;
using Unity.Jobs;

[ImageProcessingPractice(20, "ヒストグラム")]
public class ImageProcessingPractice_Histogram : ImageProcessingPractice
{
    private const int Width = 256;
    private const int Height = 256;

    public override void CreateResultTexture(Texture2D source)
    {
        _result = new Texture2D(Width, Height, TextureFormat.RGBA32, false, false);
    }

    public override void OnProcess()
    {
        var background = new Color32(20, 20, 50, 255);
        var foreground = new Color32(60, 220, 60, 255);

        using (var grayScale = TextureData.CreateGrayScaleFromRGB(_source, isTemporal: true)) {
            var histogram = new int[256];
            foreach (var (x, y) in _source) {
                histogram[grayScale.GetPixel(x, y).r]++;
            }

            var scale = (1.0f / histogram.Max()) * Height;
            for (var i = 0; i < 256; ++i) {
                histogram[i] = (int)(histogram[i] * scale);
            }

            using (var temp = TextureData.CreateTemporal(Width, Height)) {
                for (var y = 0; y < Height; ++y) {
                    for (var x = 0; x < Width; ++x) {
                        temp.SetPixel(x, y, histogram[x] > y ? foreground : background);
                    }
                }
                temp.ApplyToTexture(_result);
            }
        }
    }
}