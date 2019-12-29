using UnityEngine;

public abstract class ImageProcessingPractice : MonoBehaviour
{
    protected Texture2D _result;

    public Texture2D ResultTexture => _result;

    public virtual void CreateResultTexture(Texture2D source)
    {
        _result = new Texture2D(source.width, source.height, source.format, false, false);
    }

    public abstract void OnProcess(TextureData source);
}
