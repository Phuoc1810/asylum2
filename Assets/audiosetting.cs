using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class audiosetting : MonoBehaviour
{
    public AudioMixer audiomixer;

    public Slider MusicSlider;
    public Slider SFXslider;
    
    public void updateMusicVolume(float volume)
    {
        audiomixer.SetFloat("MusicVolume",volume);

    }
    public void UpdateSoundVolume(float volume)
    {
        audiomixer.SetFloat("SFXVolume", volume);
    }
    public void SaveVolume()
    {
        audiomixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume",musicVolume);
        audiomixer.GetFloat("SFXVolume",out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume",sfxVolume);
    }
    public void loadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SFXslider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loadVolume();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
