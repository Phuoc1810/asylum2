using UnityEngine;

public class audioclick : MonoBehaviour
{
    public AudioSource audioSource; // Gắn AudioSource từ Inspector

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 0 = chuột trái
        {
            if (!audioSource.isPlaying)   // Tránh bị chồng âm
            {
                audioSource.Play();
            }
        }
    }
}
