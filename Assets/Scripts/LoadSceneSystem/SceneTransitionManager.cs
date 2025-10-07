using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("Transition Settings")]
    [SerializeField] private float transitionDelay = 0.5f;

    private string targetSpawnID = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void LoadScene(string sceneName, string spawnPointID)
    {
        targetSpawnID = spawnPointID;

        Invoke(nameof(LoadSceneDelayed), transitionDelay);
    }

    private void LoadSceneDelayed()
    {
        if (!string.IsNullOrEmpty(targetSpawnID))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name == "SceneA" ? "tangham" : "SceneA");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (string.IsNullOrEmpty(targetSpawnID))
        {
            return;
        }

        SpawnPlayer(targetSpawnID);
        targetSpawnID = "";
    }

    private void SpawnPlayer(string spawnID)
    {
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (spawnPoint.SpawnID == spawnID)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                if (player != null)
                {
                    CharacterController cc = player.GetComponent<CharacterController>();
                    if (cc != null)
                    {
                        cc.enabled = false;
                    }

                    player.transform.position = spawnPoint.transform.position;
                    player.transform.rotation = spawnPoint.transform.rotation;

                    if (cc != null)
                    {
                        cc.enabled = true;
                    }

                    Rigidbody rb = player.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                    }
                }

                return;
            }
        }

    }

    public void LoadSceneWithTarget(string sceneName, string spawnID)
    {
        targetSpawnID = spawnID;
        SceneManager.LoadScene(sceneName);
    }
}