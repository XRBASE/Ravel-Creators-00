using UnityEngine;
using UnityEngine.UI;

public class ScrollbarSlider : MonoBehaviour
{
    //small value check against to see if floats match
    private const float LABDA = 0.0001f;
    
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private Slider _slider;
    [SerializeField] private bool _inverse = true;

    private void Awake()
    {
        _scrollbar.onValueChanged.AddListener(OnScrollbarChanged);
        _slider.onValueChanged.AddListener(OnSliderChanged);
        OnScrollbarChanged(_scrollbar.value);
    }

    private void OnSliderChanged(float value)
    {
        //normalize value to range 0 - 1
        value = (value - _slider.minValue) / (_slider.maxValue - _slider.minValue);
        if (_inverse) {
            value = 1f - value;
        }
        if (Mathf.Abs(_scrollbar.value - value) > LABDA) {
            _scrollbar.value = value;
        }
    }

    private void OnScrollbarChanged(float value)
    {
        if (_inverse) {
            value = 1f - value;
        }
        
        value = (value * (_slider.maxValue - _slider.minValue) + _slider.minValue);
        if (Mathf.Abs(_slider.value - value) > LABDA) {
            _slider.value = value;
        }
    }
}
