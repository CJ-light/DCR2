using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueText : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    public float modificador = 1;
    // Start is called before the first frame update
    void Start()
    {
        if (modificador < 0)
        {
            _slider.onValueChanged.AddListener((v) => {
                float valor = modificador*v;
                _sliderText.text = valor.ToString("0");
            });
        }
        else if (modificador < 0.001)
        {
            _slider.onValueChanged.AddListener((v) => {
                float valor = modificador*v;
                _sliderText.text = valor.ToString("0.####");
            });
        }
        else if (modificador < 0.01)
        {
            _slider.onValueChanged.AddListener((v) => {
                float valor = modificador*v;
                _sliderText.text = valor.ToString("0.###");
            });
        }
        else if (modificador < 0.1)
        {
            _slider.onValueChanged.AddListener((v) => {
                float valor = modificador*v;
                _sliderText.text = valor.ToString("0.##");
            });
        }
        else if (modificador < 1)
        {
            _slider.onValueChanged.AddListener((v) => {
                float valor = modificador*v;
                _sliderText.text = valor.ToString("0.#");
            });
        }
        else
        {
            _slider.onValueChanged.AddListener((v) => {
                float valor = modificador*v;
                _sliderText.text = valor.ToString("0");
            });
        }
    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
