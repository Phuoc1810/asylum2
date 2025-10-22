using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    [Header("Kéo thả scene đích vào đây")]
#if UNITY_EDITOR
    [SerializeField] private UnityEditor.SceneAsset targetScene; // ô kéo-thả như Transform
#endif

    [SerializeField] private float delay = 0f; // delay realtime trước khi chuyển

    // tên scene dùng lúc runtime (tự điền từ asset ở Editor)
    [SerializeField, HideInInspector] private string sceneNameRuntime;

    public void LoadTargetScene()
    {
        if (string.IsNullOrEmpty(sceneNameRuntime))
        {
            Debug.LogError("Chưa gán scene đích trong SceneTransition (kéo file .unity vào ô).");
            return;
        }

        if (delay > 0f) StartCoroutine(LoadAfterDelayRealtime());
        else SceneManager.LoadScene(sceneNameRuntime);
    }

    private IEnumerator LoadAfterDelayRealtime()
    {
        yield return new WaitForSecondsRealtime(delay);
        SceneManager.LoadScene(sceneNameRuntime);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // khi bạn kéo-thả scene, tự lưu tên để dùng khi build
        sceneNameRuntime = targetScene != null ? targetScene.name : string.Empty;
    }
#endif
}
