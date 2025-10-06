using System;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerController : MonoBehaviour
{
    public event EventHandler OnSelectedColorChange;




    [SerializeField] private Slider sliderR;
    [SerializeField] private Slider sliderG;
    [SerializeField] private Slider sliderB;


    

    public void SetSlidersToColor(Color color)
    {
        sliderR.value = color.r;
        sliderG.value = color.g;
        sliderB.value = color.b;
    }

    public Color GetCurrentlySelectedColor()
    {
        return new Color(sliderR.value, sliderG.value, sliderB.value);
    }

    public void SliderValueChange()
    {
        OnSelectedColorChange?.Invoke(this, EventArgs.Empty);
    }
}
