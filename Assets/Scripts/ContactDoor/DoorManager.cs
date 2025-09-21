using System.Collections;
using TMPro;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Setting")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isLocked = true;
    [SerializeField] private bool isBreak = false;
    [SerializeField] private float doorOpenAngle = 90f;
    [SerializeField] private float smooth = 2;

    [Header("Audio Setting")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorClosedSound;
    [SerializeField] private AudioClip doorLockedSound;
    [SerializeField] private AudioClip fixDoorSound;

    [Header("UI Setting")]
    [SerializeField] private TextMeshProUGUI doorText;

    private InteractableController controller;
    private Interactable doorInteractable;
    private Vector3 defaulRotation;
    private Vector3 openRotation;
    private bool playerInDoorRange = false;
    private bool doorInitialized = false;

    void Start()
    {
        controller = FindObjectOfType<InteractableController>();
        doorInteractable = FindObjectOfType<Interactable>();
        InitializeDoor();
    }
    void Update()
    {
        UpdateDoorRotation();
        UpdateDoorText();
    }
    private void InitializeDoor()
    {
        if (!doorInitialized && doorInteractable != null)
        {
            //Chỉ cửa DoorMaintance mới có thể bị hỏng
            if (doorInteractable.Type == Interactable.InteracType.DoorMaintance && isBreak)
            {
                isLocked = true;
                isOpen = false;
            }

            defaulRotation = transform.eulerAngles;
            openRotation = new Vector3(defaulRotation.x, defaulRotation.y + doorOpenAngle, defaulRotation.z);

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

    public void HandleDoorInteractation()
    {
        if (!playerInDoorRange) return;
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
        if (InventoryManager.instance.HasItem(Interactable.InteracType.DirectorKey))
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
        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        bool hasKeyMaintance = InventoryManager.instance.HasItem(Interactable.InteracType.KeyMaintance);

        if (hasScrewdriver && hasKeyMaintance)
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
        if (doorText != null)
        {
            doorText.text = "Đang sửa";
        }

        yield return new WaitForSeconds(2f);

        FixDoor();
        if (doorText != null)
        {
            doorText.text = "Xong!";
        }

        yield return new WaitForSeconds(1f);
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
        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        bool hasKeyMaintance = InventoryManager.instance.HasItem(Interactable.InteracType.KeyMaintance);

        if (isBreak)
        {
            if (!hasScrewdriver)
            {
                doorText.text = "Bị hỏng";
            }
            else
            {
                doorText.text = "Đang sửa";
            }
        }
        else
        {
            if (!hasScrewdriver || !hasKeyMaintance)
            {
                doorText.text = "Bị hỏng";
            }
            else
            {
                doorText.text = "";
            }
        }
    }
    private void UpdateDirectorDoorText()
    {
        bool hasDirectorKey = InventoryManager.instance.HasItem(Interactable.InteracType.DirectorKey);
        if (!hasDirectorKey)
        {
            doorText.text = "Đã bị khóa";
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
            Vector3 targetRotation = isOpen ? openRotation : defaulRotation;
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

    public bool IsPlayerInRange => playerInDoorRange;
    public bool IsOpen => isOpen;
    public bool IsLocked => isLocked;
    public bool isBroken => isBreak;
}