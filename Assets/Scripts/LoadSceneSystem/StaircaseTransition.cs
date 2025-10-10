using UnityEngine;

public class StaircaseTransition : MonoBehaviour
{
    [Header("Trigger Setting")]
    [SerializeField] private string targetSceneName = "tangham";
    [SerializeField] private string targetSpawnID = "stair1";

    [Header("Trigger Setting")]
    [SerializeField] private bool usePrompt = false;
    [SerializeField] private GameObject promptUI;

    private bool playerInRange = false;
    private bool isTransitioning = false;

    private void Start()
    {
        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }
    }
    private void Update()
    {
        if (usePrompt && playerInRange && !isTransitioning)
        {
            TriggerTransition();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (usePrompt)
            {
                ShowPrompt(true);
            }
            else
            {
                TriggerTransition();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
        if (usePrompt)
        {
            ShowPrompt(false);
        }
    }
    private void TriggerTransition()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (promptUI != null)
        {
            promptUI.SetActive(false);
        }

        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.LoadSceneWithTarget(targetSceneName, targetSpawnID);
        }
    }
    private void ShowPrompt(bool show)
    {
        if (promptUI != null)
        {
            promptUI.SetActive(show);
        }
    }
    private void OnDrawGizmos()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider != null && boxCollider.isTrigger)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
        }

#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position+Vector3.up*2f,
            $"{targetSceneName}\nSpawn: {targetSpawnID}");
#endif
    }
}
