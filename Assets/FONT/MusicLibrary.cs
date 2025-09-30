using UnityEngine;
using UnityEngine.XR;

[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip clip;
}
public class MusicLibrary : MonoBehaviour
{
   public MusicTrack[] tracks; 
  public AudioClip GetClipFromName(string Trackname)
    {
        foreach (var track in tracks)
        {
            if(track.trackName == Trackname)
            {
                return track.clip;
            }
        }

        return null;
    }
}
