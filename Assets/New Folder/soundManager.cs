using UnityEngine;

public class soundManager : MonoBehaviour
{
    public static soundManager Instance;
    [SerializeField]
    private soundlibrary sfxlibary;
    [SerializeField]
    private AudioSource audioSource;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    } 
    public void Playsound3D(AudioClip clip,Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos); 
        }
    }
    public void Playsound3D(string soundName,Vector3 pos)
    {
        Playsound3D(sfxlibary.getClipFromName(soundName), pos);
    }
  public void PlaySound2D(string soundName)
    {
        audioSource.PlayOneShot(sfxlibary.getClipFromName(soundName));
    }
}
