using System.Collections;
using TMPro;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isLocked = true;
    [SerializeField] private bool isBreak = false;
    [SerializeField] private float smooth = 2f;
    [SerializeField] private float doorOpenAngle = 90f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorClosedSound;
    [SerializeField] private AudioClip doorLockedSound;
    [SerializeField] private AudioClip fixDoorSound;

    [Header("UI Settings")]
    [SerializeField] private TextMeshProUGUI doorText;

    private InteractableController controller;
    private Interactable doorInteractable;
    private Vector3 defaultRotation;
    private Vector3 openRotation;
    private bool playerInDoorRange = false;
    private bool doorInitialized = false;

    void Start()
    {
        Debug.Log("Start - isBreak initial value: " + isBreak);
        controller = FindObjectOfType<InteractableController>();
        doorInteractable = GetComponent<Interactable>();
        InitializeDoor();
        Debug.Log("Start - isBreak after InitializeDoor: " + isBreak);
    }

    void Update()
    {
        UpdateDoorRotation();
        //UpdateDoorText();
    }

    private void InitializeDoor()
    {
        if (!doorInitialized && doorInteractable != null)
        {
            // Chỉ cửa DoorMaintance mới có thể bị hỏng
            if (doorInteractable.Type == Interactable.InteracType.DoorMaintance && isBreak)
            {
                isLocked = true;
                isOpen = false;
            }

            defaultRotation = transform.eulerAngles;
            openRotation = new Vector3(defaultRotation.x, defaultRotation.y + doorOpenAngle, defaultRotation.z);

            if (doorText != null)
            {
                doorText.text = "";
            }

            doorInitialized = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && controller != null)
        {
            playerInDoorRange = true;
            controller.SetDoorPlayerInRange(true, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && controller != null)
        {
            playerInDoorRange = false;
            controller.SetDoorPlayerInRange(false, this);
        }
    }

    // Được gọi từ InteractableController để xử lý tương tác cửa
    public void HandleDoorInteraction()
    {
        Debug.Log("HandleDoorInteraction called. playerInDoorRange: " + playerInDoorRange);
        if (!playerInDoorRange) return;

        Debug.Log("Door Type: " + doorInteractable.Type);
        switch (doorInteractable.Type)
        {
            case Interactable.InteracType.DoorMaintance:
                HandleMaintenanceDoor();
                break;
            case Interactable.InteracType.DitectorDoor:
                HandleDirectorDoor();
                break;
        }
    }

    private void HandleMaintenanceDoor()
    {

        if (isBreak)
        {
            TryFixDoor();
        }
        else
        {
            TryOpenMaintenanceDoor();
        }
    }

    private void HandleDirectorDoor()
    {
        if (InventoryManager.instance != null && InventoryManager.instance.HasItem(Interactable.InteracType.DirectorKey))
        {
            ToggleDoor();
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }

    private void TryOpenMaintenanceDoor()
    {
        if (InventoryManager.instance == null) return;

        //bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        bool hasKeyMaintance = InventoryManager.instance.HasItem(Interactable.InteracType.KeyMaintance);

        if (hasKeyMaintance && isBreak == false)
        {
            ToggleDoor();
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }

    private void TryFixDoor()
    {
        if (InventoryManager.instance == null) return;

        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);

        if (hasScrewdriver)
        {
            StartCoroutine(FixDoorProcess());
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }

    private void ToggleDoor()
    {
        isLocked = false;
        isOpen = !isOpen;

        if (isOpen)
        {
            PlayDoorSound(doorOpenSound);
        }
        else
        {
            PlayDoorSound(doorClosedSound);
        }
    }

    private IEnumerator FixDoorProcess()
    {
        Debug.Log("Cửa đang sửa");
        if (doorText != null)
        {
            doorText.text = "Đang sửa";
        }
        yield return new WaitForSeconds(6f);
        FixDoor();

        if (doorText != null)
        {
            doorText.text = "Xong!";
        }

        yield return new WaitForSeconds(3f);
        UpdateDoorText();
    }

    private void FixDoor()
    {
        isBreak = false;
        PlayDoorSound(fixDoorSound);
    }

    private void UpdateDoorText()
    {
        if (doorText == null || !playerInDoorRange || doorInteractable == null)
        {
            if (doorText != null)
            {
                doorText.text = "";
            }
            return;
        }

        switch (doorInteractable.Type)
        {
            case Interactable.InteracType.DoorMaintance:
                UpdateMaintenanceDoorText();
                break;
            case Interactable.InteracType.DitectorDoor:
                UpdateDirectorDoorText();
                break;
        }
    }

    private void UpdateMaintenanceDoorText()
    {
        if (InventoryManager.instance == null) return;

        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        bool hasKeyMaintance = InventoryManager.instance.HasItem(Interactable.InteracType.KeyMaintance);

        if (isBreak)
        {
            if (!hasScrewdriver)
            {
                doorText.text = "Bị hỏng";
            }
        }
        else
        {
            if (!hasKeyMaintance)
            {
                doorText.text = "Đã bị khóa";
            }
            else
            {
                doorText.text = "";
            }
        }
    }

    private void UpdateDirectorDoorText()
    {
        if (InventoryManager.instance == null) return;

        bool hasDirectorKey = InventoryManager.instance.HasItem(Interactable.InteracType.DirectorKey);

        if (!hasDirectorKey)
        {
            doorText.text = "Cần chìa khóa giám đốc";
        }
        else
        {
            doorText.text = "";
        }
    }

    private void UpdateDoorRotation()
    {
        if (doorInitialized)
        {
            Vector3 targetRotation = isOpen ? openRotation : defaultRotation;
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * smooth);
        }
    }

    private void PlayDoorSound(AudioClip clip)
    {
        if (doorAudioSource != null && clip != null)
        {
            doorAudioSource.PlayOneShot(clip);
        }
    }

    // Getter methods để InteractableController có thể truy cập thông tin cửa
    public bool IsPlayerInRange => playerInDoorRange;
    public bool IsOpen => isOpen;
    public bool IsLocked => isLocked;
    public bool IsBroken => isBreak;
}