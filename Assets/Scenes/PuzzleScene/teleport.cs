using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSceneTransfer : MonoBehaviour
{
    [Header("Cấu hình Scene")]
    [Tooltip("Tên scene muốn chuyển đến")]
    public string targetSceneName = "Scene2";

    [Header("Cấu hình Tương tác")]
    [Tooltip("Nhấn phím gì để chuyển scene (mặc định E)")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Tùy chọn")]
    [Tooltip("Có giữ vật phẩm này khi chuyển scene không?")]
    public bool keepItemInNewScene = false;

    private bool playerInRange = false;
    private GameObject player;

    void Start()
    {
        // Kiểm tra có Box Collider không
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            Debug.LogError("Vật phẩm này cần có Box Collider!");
        }

        // Đảm bảo Collider là trigger để detect va chạm
        if (boxCollider != null)
        {
            boxCollider.isTrigger = true;
        }

        // Nếu muốn giữ vật phẩm khi chuyển scene
        if (keepItemInNewScene)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
        // Nếu player đang ở gần và nhấn phím tương tác
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            TransferToScene();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu là player va chạm
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.gameObject;
            Debug.Log("Nhấn " + interactKey + " để chuyển scene!");
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
        Debug.Log("Đang chuyển sang scene: " + targetSceneName);

        // Lưu vị trí của player nếu cần
        if (player != null && keepItemInNewScene)
        {
            PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);
        }

        // Chuyển scene
        SceneManager.LoadScene(targetSceneName);
    }
}

