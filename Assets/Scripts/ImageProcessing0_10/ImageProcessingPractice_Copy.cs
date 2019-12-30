using UnityEngine;

[ImageProcessingPractice(0, "元画像の単純なコピー")]
public class ImageProcessingPractice_Copy : ImageProcessingPractice
{
    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                temp.SetPixel(x, y, _source.GetPixel(x, y));
            }
            temp.ApplyToTexture(_result);
        }
    }
}
