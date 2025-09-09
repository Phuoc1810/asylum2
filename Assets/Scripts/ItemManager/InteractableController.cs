using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [SerializeField] private float distanceRay = 3f;
    [SerializeField] private GameObject pointPanel;
    [SerializeField] Camera playerCamera;

    private Interactable currentInteractable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeComponents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNormalState();
    }
    #region Thiết lập ban đầu
    /// <summary>
    /// Khởi tạo các components cần thiết
    /// </summary>
    private void InitializeComponents()
    {
        pointPanel.SetActive(false);
    }
    #endregion
    #region trạng thái bình thường (Normal State)
    /// <summary>
    /// Cập nhật trạng thái bình thường
    /// </summary>
    private void UpdateNormalState()
    {
        PerformRaycast();
    }
    /// <summary>
    /// Thực hiện raycast để phát hiện item
    /// </summary>
    private void PerformRaycast()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distanceRay))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                SetCurrentInteractable(interactable);
            }
            else
            {
                ClearCurrentInteractable();
            }
        }
        else
        {
            ClearCurrentInteractable();
        }
    }
    /// <summary>
    /// Đặt object tương tác và hiện thị UI
    /// </summary>
    private void SetCurrentInteractable(Interactable interactable)
    {
        if (currentInteractable != interactable || !pointPanel.activeSelf)
        {
            currentInteractable = interactable;
            pointPanel.SetActive(true);
        }
    }
    /// <summary>
    /// Xóa object tương tác và ẩn UI
    /// </summary>
    private void ClearCurrentInteractable()
    {
        pointPanel.SetActive(false);
        currentInteractable = null;
    }
    #endregion
}
