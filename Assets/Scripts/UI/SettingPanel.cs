using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [SerializeField] private Slider main_slider;
    [SerializeField] private Slider music_slider;
    [SerializeField] private Slider sfx_slider;

    [SerializeField] private TextMeshProUGUI main_value_text;
    [SerializeField] private TextMeshProUGUI music_value_text;
    [SerializeField] private TextMeshProUGUI sfx_value_text;


    // Appelé à chaque fois que le script ou le gameobject devient actif
    void OnEnable()
    {
        SetSliders();
    }

    // Récupère les valeurs de son stockées et met les sliders aux bonnes valeurs
    public void SetSliders()
    {
        float saved_main_value = PlayerPrefs.GetFloat("main",1);
        float saved_music_value = PlayerPrefs.GetFloat("music",1);
        float saved_sfx_value = PlayerPrefs.GetFloat("sfx",1);

        main_slider.value = saved_main_value;
        music_slider.value = saved_music_value;
        sfx_slider.value = saved_sfx_value;
    }

    public void OnMusicSliderChange(float value)
    {
        // Met à jour le texte correspondant
        music_value_text.text = Math.Floor(value * 100).ToString();
        // Appelle le sound manager pour changer le volume
        GameManager.instance.sound_manager.SetMusic(value);
    }

    public void OnMainSliderChange(float value)
    {
        main_value_text.text = Math.Floor(value * 100).ToString();
        GameManager.instance.sound_manager.SetMain(value);
    }

    public void OnSfxSliderChange(float value)
    {
        sfx_value_text.text = Math.Floor(value * 100).ToString();
        GameManager.instance.sound_manager.SetSfx(value);
    }



}
