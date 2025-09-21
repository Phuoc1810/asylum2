using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [Header("Raycast Setting")]
    [SerializeField] private float distanceRay = 3f;

    [SerializeField] private GameObject pointPanel;
    [SerializeField] Camera playerCamera;

    private Animator targetAnimation;
    private Interactable currentInteractable;
    private DoorManager currentDoorManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeComponents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNormalState();
        HandleInput();
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

    #region ElectricBox System
    /// <summary>
    /// Các biến liên quan
    /// </summary>
    [Header("Electric Box Setting")]
    [SerializeField] private GameObject fuseObject;
    [SerializeField] private GameObject[] light;

    /// <summary>
    /// Cập nhật các trạng thái của Electric Box
    /// </summary>
    private void HandleElectricBoxInteraction()
    {
        if (currentInteractable.Type == Interactable.InteracType.HanldeElectricBox)
        {
            if(InventoryManager.instance!=null && InventoryManager.instance.HasItem(Interactable.InteracType.Fuse))
            {
                ActivateElectricSystem();
            }
            else
            {
                DeactivateElectricSystem();
            }
        }
        else if (currentInteractable.Type == Interactable.InteracType.ElectricBox)
        {
            InstallFuse();
        }
    }
    private void InstallFuse()
    {
        if (InventoryManager.instance != null && InventoryManager.instance.HasItem(Interactable.InteracType.Fuse))
        {
            if (fuseObject != null)
            {
                fuseObject.SetActive(true);
            }
        }
    }
    private void ActivateElectricSystem()
    {
        if (currentInteractable.Anim != null)
        {
            currentInteractable.Anim.SetTrigger("Open");
        }
        InventoryManager.instance.RemoveItem(Interactable.InteracType.Fuse);
        PlayOpenSound(currentInteractable);
        TurnOnLights();

    }
    private void DeactivateElectricSystem()
    {
        if (currentInteractable.Anim != null)
        {
            currentInteractable.Anim.SetTrigger("Close");
        }
        PlayCloseSound(currentInteractable);
    }
    private void TurnOnLights()
    {
        if (light != null)
        {
            foreach(GameObject light in light)
            {
                if (light != null)
                {
                    light.SetActive(true);
                    Light pointLight = light.GetComponent<Light>();
                    if (pointLight != null)
                    {
                        pointLight.enabled = true;
                    }
                }
            }
        }
    }
    #endregion

    #region Tương tác cửa
    ///<summary>
    ///
    ///</summary>
    private void HandleDoorInteraction()
    {
        if (IsDoorType(currentInteractable.Type))
        {
            DoorManager doorManager = currentInteractable.GetComponent<DoorManager>();
            if (doorManager != null&& doorManager.IsPlayerInRange)
            {
                doorManager.HandleDoorInteractation();
            }
        }
    }

    private bool IsDoorType(Interactable.InteracType type)
    {
        return type == Interactable.InteracType.DoorMaintance ||
               type == Interactable.InteracType.DirectorKey;
    }
    public void SetDoorPlayerInRange(bool inRange, DoorManager doorManager)
    {
        if (inRange)
        {
            currentDoorManager = doorManager;
        }
        else
        {
            if (currentDoorManager == doorManager)
            {
                currentDoorManager = null;
            }
        }
    }
    #endregion
    #region Pickup item
    ///<summary>
    ///Xu ly input tu nguoi choi
    ///</summary>
    private void HandleInput()
    {
        if(Input.GetMouseButtonDown(0) && currentInteractable != null)
        {
            HandleInteraction();
        }
    }
    
    ///<summary>
    ///Xu ly tuong tac dua tren loai item
    ///</summary>
    private void HandleInteraction()
    {
        switch (currentInteractable.Type)
        {
            case Interactable.InteracType.Screwdriver:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.KeyMaintance:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.Fuse:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.ElectricBox:
                HandleElectricBoxInteraction();
                break;
            case Interactable.InteracType.HanldeElectricBox:
                HandleElectricBoxInteraction();
                break;
            case Interactable.InteracType.BoltCutter:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.Crowbar:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.DirectorKey:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.DoorMaintance:
            case Interactable.InteracType.DitectorDoor:
                HandleDoorInteraction();
                break;
        }
    }
    ///<summary>
    ///Nhat item va them vao inventory
    ///</summary>
    private void PickupItem(GameObject item)
    {
        if (InventoryManager.instance == null)
        {
            return;
        }
        InventoryManager.instance.AddItems(item);
        item.SetActive(false);
    }
    #endregion

    #region Audio System
    private void PlayOpenSound(Interactable interactable) 
    {
        if (interactable != null)
        {
            AudioSource.PlayClipAtPoint(interactable.Open, interactable.transform.position);
        }
    }
    private void PlayCloseSound(Interactable interactable)
    {
        AudioSource.PlayClipAtPoint(interactable.Close, interactable.transform.position);
    }
    #endregion
}
