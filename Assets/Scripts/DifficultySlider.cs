using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySlider : MonoBehaviour
{
    public Slider difficultySlider;

    /*public void Start()
    {
        //difficultySlider.onValueChanged.AddListener(delegate { SliderChange(); });
        PlayerSettings.difficulty = (int)difficultySlider.value;
    }*/

    void Start()
    {
        difficultySlider.value = PlayerSettings.difficulty;
        difficultySlider.onValueChanged.AddListener(v => PlayerSettings.difficulty = Mathf.RoundToInt(v));
    }


    public void SliderChange()
    {
        PlayerSettings.difficulty = (int) difficultySlider.value;
    }


}
