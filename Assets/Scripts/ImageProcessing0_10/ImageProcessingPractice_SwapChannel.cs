using UnityEngine;

[ImageProcessingPractice(1, "チャンネル入れ替え")]
public class ImageProcessingPractice_SwapChannel : ImageProcessingPractice
{
    [SerializeField] private bool _swapRG = true;
    [SerializeField] private bool _swapRB = false;
    [SerializeField] private bool _swapGB = false;

    public override void OnProcess()
    {
        using (var temp = TextureData.CreateTemporal(_source.Width, _source.Height)) {
            foreach (var (x, y) in _source) {
                var c = _source.GetPixel(x, y);
                if (_swapRG) {
                    var t = c.g;
                    c.g = c.r;
                    c.r = t;
                }

                if (_swapRB) {
                    var t = c.b;
                    c.b = c.r;
                    c.r = t;
                }

                if (_swapGB) {
                    var t = c.g;
                    c.g = c.b;
                    c.b = t;
                }

                temp.SetPixel(x, y, c);
            }
            temp.ApplyToTexture(_result);
        }
    }
}
