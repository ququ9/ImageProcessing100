using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ImageProcessingPracticeUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _sourceImages = default;
    [SerializeField] private Dropdown _sourceDropdown = default;
    [SerializeField] private Dropdown _imageEffects = default;
    [SerializeField] private Button _saveButton = default;

    public Sprite CurrentSourceImage => _sourceImages[_sourceDropdown.value];
    public Action OnSourceChanged { get; set; }
    public Action<int> OnEffectChanged { get; set; }
    public Action OnSave { get; set; }

    public void UpdateImageEffects(IEnumerable<string> imageEffects)
    {
        _imageEffects.options = imageEffects.Select(e => new Dropdown.OptionData(e)).ToList();
    }

    private void Awake()
    {
        _sourceDropdown.options = _sourceImages.Select(i => new Dropdown.OptionData(i.name, i)).ToList();
        _sourceDropdown.onValueChanged.AddListener((index) => {
            this.OnSourceChanged?.Invoke();
        });

        _imageEffects.onValueChanged.AddListener((index) => {
            this.OnEffectChanged?.Invoke(index);
        });

        _saveButton.onClick.AddListener(() => {
            this.OnSave?.Invoke();
        });
    }
}
