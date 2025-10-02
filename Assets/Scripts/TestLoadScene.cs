using UnityEngine;

public class TestLoadScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
