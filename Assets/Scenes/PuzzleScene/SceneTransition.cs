using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("Tên Scene Đích")]
    [SerializeField] private string targetSceneName;

    [Header("Độ trễ trước khi chuyển (giây)")]
    [SerializeField] private float delay = 0f;

    // Gọi hàm này để chuyển scene
    public void LoadTargetScene()
    {
        if (delay > 0f)
            Invoke(nameof(LoadSceneNow), delay);
        else
            LoadSceneNow();
    }

    private void LoadSceneNow()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogError(" Chưa gán tên Scene đích trong SceneTransition!");
        }
    }
}
