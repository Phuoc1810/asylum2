using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSceneTransfer : MonoBehaviour
{
    [Header("C?u h?nh Scene")]
    [Tooltip("Tên scene mu?n chuy?n ð?n")]
    public string targetSceneName = "Scene2";

    [Header("C?u h?nh Týõng tác")]
    [Tooltip("Nh?n phím g? ð? chuy?n scene (m?c ð?nh E)")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Tùy ch?n")]
    [Tooltip("Có gi? v?t ph?m này khi chuy?n scene không?")]
    public bool keepItemInNewScene = false;

    private bool playerInRange = false;
    private GameObject player;

    void Start()
    {
        // Ki?m tra có Box Collider không
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("V?t ph?m này c?n có Box Collider!");
        }

        // Ð?m b?o Collider là trigger ð? detect va ch?m
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }

        // N?u mu?n gi? v?t ph?m khi chuy?n scene
        if (keepItemInNewScene)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        // N?u player ðang ? g?n và nh?n phím týõng tác
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            TransferToScene();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Ki?m tra n?u là player va ch?m
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
            Debug.Log("Nh?n " + interactKey + " ð? chuy?n scene!");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;
        }
    }

    void TransferToScene()
    {
        Debug.Log("Ðang chuy?n sang scene: " + targetSceneName);

        // Lýu v? trí c?a player n?u c?n
        if (player != null && keepItemInNewScene)
        {
            PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);
        }

        // Chuy?n scene
        SceneManager.LoadScene(targetSceneName);
    }
}

// ========== SCRIPT PH?: Ð?t vào Player ð? khôi ph?c v? trí ==========
// T?o file m?i tên "PlayerPositionLoader.cs" và paste ðo?n code dý?i

/*
using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    void Start()
    {
        // Khôi ph?c v? trí player n?u có lýu
        if (PlayerPrefs.HasKey("PlayerPosX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerPosX");
            float y = PlayerPrefs.GetFloat("PlayerPosY");
            float z = PlayerPrefs.GetFloat("PlayerPosZ");
            transform.position = new Vector3(x, y, z);
            
            // Xóa d? li?u ð? lýu
            PlayerPrefs.DeleteKey("PlayerPosX");
            PlayerPrefs.DeleteKey("PlayerPosY");
            PlayerPrefs.DeleteKey("PlayerPosZ");
        }
    }
}
*/