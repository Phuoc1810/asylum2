using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    [SerializeField]
    private MusicLibrary musicLibrary;
    [SerializeField]
    private AudioSource musicSource;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);

        }
        else
        {
            instance = this;    
            DontDestroyOnLoad(gameObject);
        }
    }
    public void Playmusic(string TrackName,float fadeDuration = 0.5f)
    {
        StartCoroutine(animateMusicCrossfade(musicLibrary.GetClipFromName(TrackName), fadeDuration));
    }
IEnumerator animateMusicCrossfade(AudioClip nextTrack,float fadeDuration =0.5f)
    {
        float percent = 0;
        while (percent<1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(1f,0,percent);
            yield return null;
        }
        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while(percent<1)
        {
            percent += Time.deltaTime * 1 / fadeDuration;
            musicSource.volume = Mathf.Lerp(0, 1f, percent);
            yield return null;
        }
    }
}
