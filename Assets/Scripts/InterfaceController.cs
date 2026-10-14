using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InterfaceController : MonoBehaviour
{
    [SerializeField] Scrollbar _scrollbar;
    [SerializeField] Slider _slider;
    [SerializeField] private List<Light> _lights;
    [SerializeField] private Dictionary<Light, float> _lightsIntensity;

    private void Start()
    {
        _lightsIntensity = new Dictionary<Light, float>();
        foreach (Light light in _lights ) 
        {
            _lightsIntensity.Add( light, light.intensity);            
        }

        _slider.onValueChanged.AddListener(ChangeBrightness);
        _scrollbar.onValueChanged.AddListener(ChangeColor);

        ChangeBrightness(_slider.value);
        ChangeColor(_scrollbar.value);
    }

    private void ChangeColor(float value)
    {
        Color color = Color.HSVToRGB(value, 1f, 1f);

        foreach (Light light in _lights)
        {
            if (light != null)
                light.color = color;
        }
    }
    private void ChangeBrightness(float value)
    {
        foreach (Light light in _lights)
        {
            if (light != null)
                light.intensity = _lightsIntensity[light] * value;
        }
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(ChangeBrightness);
        _scrollbar.onValueChanged.RemoveListener(ChangeColor);
    }
}
