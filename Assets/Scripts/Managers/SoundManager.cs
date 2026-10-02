using System;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audio_mixer;

    void Start()
    {
        // Charge les settings de son dès le lancement
        LoadSetting();
    }


    private void LoadSetting()
    {
        // Regarde dans les PlayerPrefs pour les valeurs stockées; si elles n'existent pas, prend 1 par défaut
        float saved_main_value = PlayerPrefs.GetFloat("main",1);
        float saved_music_value = PlayerPrefs.GetFloat("music",1);
        float saved_sfx_value = PlayerPrefs.GetFloat("sfx",1);

        // Applique les paramètres récupérés
        SetMain(saved_main_value);
        SetMusic(saved_music_value);
        SetSfx(saved_sfx_value);
    }

    public void SetMusic(float value)
    {
        // S'assure que la valeur est entre 0 et 1
        value = Math.Clamp(value,0,1);
        
        // Convertit la value en décibels, 1 = 0 et 0 = -50
        float decibel = (1 - value) * -50;

        // Change le volume du channel correspondant
        audio_mixer.SetFloat("MusicVolume",decibel);

        // Enregistre la valeur du slider dans les PlayerPrefs
        PlayerPrefs.SetFloat("music",value);
        PlayerPrefs.Save();
    }

    public void SetMain(float value)
    {
        value = Math.Clamp(value,0,1);
        float decibel = (1 - value) * -50;
        
        audio_mixer.SetFloat("MainVolume",decibel);

        PlayerPrefs.SetFloat("main",value);
        PlayerPrefs.Save();
    }

    public void SetSfx(float value)
    {
        value = Math.Clamp(value,0,1);
        float decibel = (1 - value) * -50;

        audio_mixer.SetFloat("SfxVolume",decibel);
        
        PlayerPrefs.SetFloat("sfx",value);
        PlayerPrefs.Save();
    }
    
}

