using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ImageProcessingPracticeController : MonoBehaviour
{
    [SerializeField] private ImageProcessingPracticeUI _ui = default;
    [SerializeField] private GameObject _imageProcessorRoot = default;
    [SerializeField] private SpriteRenderer _sourceSprite = default;
    [SerializeField] private SpriteRenderer _resultSprite = default;

    private readonly List<(int Priority, string Description, ImageProcessingPractice Effect)> _allPractices = new List<(int, string, ImageProcessingPractice)>();
    private ImageProcessingPractice _currentEffect = null;

    private TextureData _sourceBuffer;

    private void Start()
    {
        var allPractices = new List<(int Priority, string Description, ImageProcessingPractice Effect)>();
        foreach (var practice in _imageProcessorRoot.GetComponentsInChildren<ImageProcessingPractice>()) {
            var className = practice.GetType().Name;
            var attribute = practice.GetType().GetCustomAttribute<ImageProcessingPracticeAttribute>();
            if (attribute != null) {
                allPractices.Add((attribute.Number, $"{attribute.Description} ({className})", practice));
            } else {
                allPractices.Add((999, $"999: (Attribute not set)", practice));
            }
        }

        _allPractices.Clear();
        _allPractices.AddRange(allPractices.OrderBy(t => t.Priority));

        _ui.UpdateImageEffects(_allPractices.Select(p => p.Description));

        _ui.OnSourceChanged += this.OnSourceChanged;
        _ui.OnEffectChanged += this.OnEffectChanged;

        if (_allPractices.Any()) {
            this.OnEffectChanged(0);
        }
    }

    private void OnDestroy()
    {
        this.ClearBuffer();
    }

    private void ClearBuffer()
    {
        _sourceBuffer?.Dispose();
        _sourceBuffer = null;
    }

    private void CreateSourceBuffer()
    {
        if (_ui.CurrentSourceImage == null) {
            return;
        }

        this.ClearBuffer();
        _sourceBuffer = new TextureData(_ui.CurrentSourceImage.texture, false);
    }

    private void OnSourceChanged()
    {
        if (_ui.CurrentSourceImage == null) {
            return;
        }

        this.CreateSourceBuffer();
        this.ApplyEffect();
    }

    private void OnEffectChanged(int index)
    {
        if ((index >= 0) && (index < _allPractices.Count)) {
            _currentEffect = _allPractices[index].Effect;
            this.ApplyEffect();
        } else {
            Debug.LogError($"ImageEffect Index is invalid {index}");
        }
    }

    private void ApplyEffect()
    {
        var sourceImage = _ui.CurrentSourceImage;
        if ((_sourceBuffer == null) && (sourceImage != null)) {
            this.CreateSourceBuffer();
        }

        if ((_sourceBuffer == null) || (sourceImage == null)) {
            Debug.LogError($"SourceSprite is not initialized");
            return;
        }

        if (_currentEffect == null) {
            Debug.LogError($"ImageProcessingPractice is null");
            return;
        }

        _sourceSprite.sprite = sourceImage;

        _currentEffect.CreateResultTexture(sourceImage.texture);

        Debug.Log($"Process {_currentEffect.GetType().Name}");
        _currentEffect.OnProcess(_sourceBuffer);

        var result = _currentEffect.ResultTexture;
        _resultSprite.sprite = Sprite.Create(result, new Rect(0, 0, result.width, result.height), new Vector2(0.5f, 0.5f));
    }
}
