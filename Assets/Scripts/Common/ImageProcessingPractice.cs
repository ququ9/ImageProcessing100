using System;
using UnityEngine;

public abstract class ImageProcessingPractice : MonoBehaviour
{
    protected Texture2D _result;
    protected TextureData _source;

    public Texture2D ResultTexture => _result;

    public virtual void CreateResultTexture(Texture2D source)
    {
        _result = new Texture2D(source.width, source.height, source.format, false, false);
    }

    public void Initialize(TextureData source)
    {
        _source = source;
    }

    public abstract void OnProcess();

    private void OnValidate()
    {
        if (_source != null) {
            this.OnProcess();
        }
    }
}
