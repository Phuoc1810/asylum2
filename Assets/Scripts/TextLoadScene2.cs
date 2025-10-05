using UnityEngine;

public class TextLoadScene2 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }
}
