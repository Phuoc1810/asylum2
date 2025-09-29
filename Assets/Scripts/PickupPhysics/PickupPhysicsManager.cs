using TMPro;
using UnityEngine;

public class PickupPhysicsManager : MonoBehaviour
{
    [Header("Hold Positions")]
    [SerializeField] private Transform boxDirectorKeyHoldPosition;
    [SerializeField] private Transform noteKnock;
    [SerializeField] private Transform noteDrawer;

    [Header("Movement Setting")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("UI reference")]
    [SerializeField] private GameObject inspectionInforPanel;
    [SerializeField] private TextMeshProUGUI inspectionInforText;

    [Header("Item Scaling")]
    [SerializeField] private float boxDirectorKeyScale = 0.6f;
    [SerializeField] private float noteKnockScale = 0.5f;
    [SerializeField] private float noteDrawerScale = 0.6f;

    [SerializeField] private Transform targetHoldPosition;

    private GameObject currentItem;
    private Interactable.InteracType currentItemType;
    private Camera playerCamera;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Vector3 originalScale;

    private bool isMovingToHoldPosition = false;
    private float movementProgress = 0f;

    public void StartPickupItem(GameObject item, Camera camera, Interactable.InteracType itemType)
    {
        if (item == null) return;
        if (camera == null) return;

        currentItem = item;
        currentItemType = itemType;
        playerCamera = camera;

        if (targetHoldPosition == null) return;

        SaveOriginalItemState();
        ApplyItemScale();
        DisableItemPhysics();
        BeginItemMovement();
    }
    public void UpdateItemPickup()
    {
        if (currentItem == null) return;

        if (isMovingToHoldPosition)
        {
            UpdateItemMovement();
        }
        else
        {
            HandleItemRotation();
        }
    }
    private void SaveOriginalItemState()
    {
        originalPosition = currentItem.transform.position;
        originalRotation = currentItem.transform.rotation;
        originalParent = currentItem.transform.parent;
        originalScale = currentItem.transform.localScale;
    }
    private void ApplyItemScale()
    {
        switch (currentItemType)
        {
            case Interactable.InteracType.BoxDirectorKey:
                currentItem.transform.localScale = originalScale * boxDirectorKeyScale;
                break;
            case Interactable.InteracType.NoteKnock:
                currentItem.transform.localScale = originalScale * noteKnockScale;
                break;
            case Interactable.InteracType.NoteDrawer:
                currentItem.transform.localScale = originalScale * noteDrawerScale;
                break;
        }
    }
    private void DisableItemPhysics()
    {
        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }
    private void BeginItemMovement()
    {
        isMovingToHoldPosition = true;
        movementProgress = 0f;
    }
    private void UpdateItemMovement()
    {
        if (targetHoldPosition == null || currentItem == null) return;

        movementProgress += Time.deltaTime * moveSpeed;

        Vector3 targetPosition = targetHoldPosition.position;
        Quaternion targetRotation = targetHoldPosition.rotation;

        currentItem.transform.position = Vector3.Lerp(
            originalPosition,
            targetPosition,
            movementProgress
            );

        currentItem.transform.rotation = Quaternion.Slerp(
            originalRotation,
            targetRotation,
            movementProgress
            );

        if (movementProgress >= 1)
        {
            CompleteItemMovement();
        }
    }
    private void CompleteItemMovement()
    {
        if (currentItem == null || playerCamera == null) return;

        isMovingToHoldPosition = false;
        currentItem.transform.SetParent(playerCamera.transform);
        currentItem.transform.localPosition = targetHoldPosition.localPosition;
        currentItem.transform.localRotation = targetHoldPosition.localRotation;

        ShowInspectionPanel(true);

        UpdateFuntion();
    }
    private void HandleItemRotation()
    {
        if (currentItem == null) return;

        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            currentItem.transform.Rotate(-mouseY, mouseX, 0, Space.Self);
        }
    }
    private void ShowInspectionPanel(bool show)
    {
        if (inspectionInforPanel != null)
        {
            inspectionInforPanel.SetActive(show);
        }
    }
    public void StopInspecting()
    {
        if (currentItem == null)
        {
            return;
        }

        ShowInspectionPanel(false);

        currentItem.transform.SetParent(originalParent);
        currentItem.transform.position = originalPosition;
        currentItem.transform.rotation = originalRotation;
        currentItem.transform.localScale = originalScale;

        Rigidbody rb = currentItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        currentItem = null;
        playerCamera = null;

        InteractableController controller = FindObjectOfType<InteractableController>();
        if (controller != null)
        {
            controller.OnInspectionComplete();
        }
    }
    private void UpdateFuntion()
    {
        if (currentItemType == Interactable.InteracType.BoxDirectorKey)
        {
            BoxKnockPuzzle puzzle = currentItem.GetComponent<BoxKnockPuzzle>();
            if (puzzle != null)
            {
                puzzle.StartInspecting();
            }
        }
        if (currentItemType == Interactable.InteracType.NoteKnock)
        {
            if (inspectionInforText != null)
            {
                inspectionInforText.text = "3-3-2-1 ?";
            }
        }
        else
        {
            inspectionInforText.text = "";
        }
    }
}
