using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableController : MonoBehaviour
{
    [Header("Detection Setting")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float detectionDistance = 3f;
    [SerializeField] private LayerMask interactableLayer = -1;

    [Header("Player Components")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private HeadBobbingController headBobbingController;

    [Header("Pickup System")]
    [SerializeField] private PickupPhysicsManager pickupPhysicsManager;

    [Header("UI Panels")]
    [SerializeField] private GameObject interactionPromptPanel;
    [SerializeField] private GameObject aimPanel;

    private Interactable currentInteractable;
    private bool isHoldingItem = false;

    public static InteractableController instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject.transform.root.gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject.transform.root.gameObject);
    }
    private void Start()
    {
        InitializeController();
    }
    private void Update()
    {
        if (isHoldingItem)
        {
            UpdateHoldingState();
        }
        else
        {
            UpdateNormalState();
        }
        HanldeInteractionInput();
    }
    private void InitializeController()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        if (interactionPromptPanel != null)
        {
            interactionPromptPanel.SetActive(false);
        }
    }
    private void UpdateNormalState()
    {
        DetecInteractableObject();
    }
    private void DetecInteractableObject()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if(Physics.Raycast(ray, out RaycastHit hit, detectionDistance, interactableLayer))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();

            if(interactable != null)
            {
                SetCurrentInteractable(interactable);
                return;
            }
        }
        ClearCurrentInteractable();
    }
    private void SetCurrentInteractable(Interactable interactable)
    {
        if (currentInteractable != interactable)
        {
            currentInteractable = interactable;
            ShowInteractionPrompt(true);
        }
    }
    private void ClearCurrentInteractable()
    {
        if (currentInteractable != null)
        {
            currentInteractable = null;
            ShowInteractionPrompt(false);
        }
    }
    private void UpdateHoldingState()
    {
        if (pickupPhysicsManager != null)
        {
            pickupPhysicsManager.UpdateItemPickup();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ExitInspectMode();
        }
    }
    public void OnInspectionComplete()
    {
        EnablePlayerControls();
        isHoldingItem = false;
    }
    private void HanldeInteractionInput()
    {
        if (Input.GetMouseButtonDown(0) && currentInteractable != null && !isHoldingItem)
        {
            ProcessInteraction(currentInteractable);
        }
    }
    private void ProcessInteraction(Interactable interactable)
    {
        switch (interactable.Type)
        {
            case Interactable.InteracType.Screwdriver:
            case Interactable.InteracType.Fuse:
            case Interactable.InteracType.KeyMaintenance:
            case Interactable.InteracType.BoltCutter:
            case Interactable.InteracType.Crowbar:
            case Interactable.InteracType.DirectorKey:
                PickupItem(interactable.gameObject);
                break;

            case Interactable.InteracType.ElectricBox:
                HandleElectricBoxInteraction(interactable);
                break;

            case Interactable.InteracType.ElectricBoxHandle:
                HanldeElectricBoxHanldeInteraction(interactable);
                break;

            case Interactable.InteracType.BoxDirectorKey:
            case Interactable.InteracType.NoteKnock:
            case Interactable.InteracType.NoteDrawer:
                StartInspectingItem(interactable.gameObject, interactable.Type);
                break;

            case Interactable.InteracType.DoorMaintenance:
            case Interactable.InteracType.DirectorDoor:
                HandleDoorInteraction(interactable);
                break;
            case Interactable.InteracType.DirectorDrawers:
                HandleDrawerInteraction(interactable);
                break;
            case Interactable.InteracType.keymorgue:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.keytools:
                PickupItem(currentInteractable.gameObject);
                break;
            case Interactable.InteracType.blackkey:
                PickupItem(currentInteractable.gameObject);
                break;
        }
    }
    private  void PickupItem(GameObject item)
    {
        var inv = InventoryService.Instance;
        var ws = WorldStateService.Instance;
        if (inv == null)
        {
            Debug.LogError("InventoryService.Instance is NULL in PickupItem!"); 
            return;
        }

        var interactable = item.GetComponent<Interactable>();
        if (interactable == null) return;

        if (!string.IsNullOrEmpty(interactable.ItemId))
        {
            inv.Add(interactable.ItemId, 1);
        }
        if (ws != null && !string.IsNullOrEmpty(interactable.WorldObjectId))
        {
            ws.MarkPicked(interactable.WorldObjectId);
        }
        item.SetActive(false);
    }
    private void StartInspectingItem(GameObject item, Interactable.InteracType itemType)
    {
        if (isHoldingItem) return;

        Interactable interactable = item.GetComponent<Interactable>();
        if (interactable != null && interactable.InspectorSound != null)
        {
            AudioSource.PlayClipAtPoint(interactable.InspectorSound, item.transform.position);
        }

        if (pickupPhysicsManager != null)
        {
            pickupPhysicsManager.StartPickupItem(item, playerCamera, itemType);
            isHoldingItem = true;
            DisablePlayerControls();
        }
    }
    private void ExitInspectMode()
    {
        if (pickupPhysicsManager != null)
        {
            pickupPhysicsManager.StopInspecting();
        }

        EnablePlayerControls();
        isHoldingItem = false;
    }
    private void HandleElectricBoxInteraction(Interactable interactable)
    {
        ElectricBoxController controller = interactable.GetComponent<ElectricBoxController>();

        if(controller != null)
        {
            bool success = controller.InstallFuse();

            if (success)
            {
                interactable.PlaySound(interactable.OpenSound);
            }
            else
            {
                interactable.PlaySound(interactable.CloseSound);
            }
        }
    }
    private void HanldeElectricBoxHanldeInteraction(Interactable interactable)
    {
        ElectricBoxController controller = interactable.GetComponentInParent<ElectricBoxController>();

        if (controller == null)
        {
            controller = FindObjectOfType<ElectricBoxController>();
        }
        if (controller != null)
        {
            controller.ToggleElectricBox();
        }
    }
    private void HandleDoorInteraction(Interactable interactable)
    {
        DoorManager doorManager = interactable.GetComponent<DoorManager>();

        if (doorManager != null)
        {
            doorManager.HandleDoorInteraction();
        }
    }
    private void HandleDrawerInteraction(Interactable interactable)
    {
        DrawerFocusController drawerController = interactable.GetComponent<DrawerFocusController>();

        if (drawerController != null && !drawerController.IsSolved)
        {
            drawerController.StartFocusMode(playerCamera);
            DisablePlayerControls();
        }
    }
    private void DisablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = false;
        if (headBobbingController != null)
            headBobbingController.enabled = false;

        ShowInteractionPrompt(false);

        if (aimPanel != null)
            aimPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    private void EnablePlayerControls()
    {
        if (playerMovement != null)
            playerMovement.enabled = true;
        if (headBobbingController != null)
            headBobbingController.enabled = true;
        if (aimPanel != null)
            aimPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void OnDrawerPuzzleComplete()
    {
        EnablePlayerControls();
    }
    private void ShowInteractionPrompt(bool show)
    {
        if (interactionPromptPanel != null)
        {
            interactionPromptPanel.SetActive(show);
        }
    }
}
