using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door State")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private bool isLocked = true;
    [SerializeField] private bool isBroken = false;

    [Header("Door Animation Setting")]
    [SerializeField] private float animationSmooth = 2f;
    [SerializeField] private float doorOpenAngle = 90f;

    [Header("Audio Setting")]
    [SerializeField] private AudioSource doorAudioSource;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip doorClosedSound;
    [SerializeField] private AudioClip doorLockedSound;
    [SerializeField] private AudioClip fixDoorSound;
    [SerializeField] private AudioClip doorSlamSound;
    [SerializeField] private AudioClip doorKnockSound;
    [SerializeField] private AudioClip screamSound;

    [Header("UI Setting")]
    [SerializeField] private TextMeshProUGUI doorStatusText;

    [Header("Fix Setting")]
    [SerializeField] private float fixDuration = 6f;
    [SerializeField] private float textDisplayDuration = 3f;

    [Header("Door Pivot (optional)")]
    [SerializeField] private Transform doorPivot;

    [Header("Jumpscare Setting")]
    [SerializeField] private GameObject scaryFace;

    private Interactable doorInteractable;
    private Vector3 defaultRotation;
    private Vector3 openRotation;
    private bool playerInRange = false;
    [SerializeField] private bool isInitialized = false;
    private bool isFixing = false;
    private bool isFaceActive = false;

    public bool IsPlayerInRange => playerInRange;
    public bool IsOpen => isOpen;
    public bool IsLocked => isLocked;
    public bool IsBroken => isBroken;

    private void Start()
    {
        InitializeDoor();
    }
    private void Update()
    {
        UpdateDoorRotation();
        UpdateDoorText();
    }
    private void InitializeDoor()
    {
        if (isInitialized) return;

        doorInteractable = GetComponent<Interactable>();
        if (doorInteractable == null)
        {
            return;
        }

        var t = doorPivot != null ? doorPivot : transform;

        defaultRotation = t.eulerAngles;
        openRotation = new Vector3(
            defaultRotation.x,
            defaultRotation.y + doorOpenAngle,
            defaultRotation.z
            );
        if (doorInteractable.Type == Interactable.InteracType.DoorMaintenance && isBroken)
        {
            isLocked = true;
            isOpen = false;
        }
        if (doorStatusText != null)
        {
            doorStatusText.text = "";
        }

        isInitialized = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (isFaceActive && doorInteractable != null && 
                doorInteractable.Type == Interactable.InteracType.DirectorDoor)
            {
                TriggerJumpscare();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (doorStatusText != null)
            {
                doorStatusText.text = "";
            }
        }
    }
    public void HandleDoorInteraction()
    {
        if (!playerInRange || isFixing) return;

        switch (doorInteractable.Type)
        {
            case Interactable.InteracType.DoorMaintenance:
                HandleMaintenanceDoor();
                break;
            case Interactable.InteracType.DirectorDoor:
                HandleDirectorDoor();
                break;
            case Interactable.InteracType.BroadingDoor:
                HandleBroadingDoor();
                break;
            case Interactable.InteracType.FileDoor:
                HanldeFileDoor();
                break;
            case Interactable.InteracType.XquangDoor:
                HandleXQUangDoor();
                break;
            case Interactable.InteracType.WCDoor:
                HandleWCDoor();
                break;
            case Interactable.InteracType.DoorStrorage:
                HandleStrorageDoor();
                break;
        }
    }
    private void HandleMaintenanceDoor()
    {
        if (isBroken)
        {
            TryFixDoor();
            return;
        }

        TryOpenMaintenanceDoor();
    }
    private void TryOpenMaintenanceDoor()
    {
        bool hasKey = InventoryService.Instance != null && InventoryService.Instance.Contains("key_maintenance");

        if (hasKey)
        {
            ToggleDoor();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.gameObject.GetComponent<PlayerSaveData>().Autosave(6, true);
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }
    private void TryFixDoor()
    {
        bool hasScrewdriver = InventoryService.Instance != null && InventoryService.Instance.Contains("screwdriver");

        if (hasScrewdriver)
        {
            StartCoroutine(FixDoorProcess());
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }
    private void HandleDirectorDoor()
    {
        bool hasDirectorKey = InventoryService.Instance != null && InventoryService.Instance.Contains("director_key");

        if (hasDirectorKey)
        {
            ToggleDoor();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.gameObject.GetComponent<PlayerSaveData>().Autosave(10, true);
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }
    private void HandleBroadingDoor()
    {
        bool hasBroadingKey = InventoryService.Instance != null && InventoryService.Instance.Contains("broading_key");
        if (hasBroadingKey)
        {
            ToggleDoor();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.gameObject.GetComponent<PlayerSaveData>().Autosave(1, true);
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }
    private void HanldeFileDoor()
    {
        bool hasFileKey = InventoryService.Instance != null && InventoryService.Instance.Contains("file_key");
        if (hasFileKey)
        {
            ToggleDoor();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.gameObject.GetComponent<PlayerSaveData>().Autosave(5, true);
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }
    private void HandleXQUangDoor()
    {
        bool hasXQKey = InventoryService.Instance != null && InventoryService.Instance.Contains("xq_key");
        if (hasXQKey)
        {
            ToggleDoor();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.gameObject.GetComponent<PlayerSaveData>().Autosave(4, true);
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }
    private void HandleWCDoor()
    {
        bool hasWCKey = InventoryService.Instance != null && InventoryService.Instance.Contains("wc_key");
        if (hasWCKey)
        {
            ToggleDoor();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.gameObject.GetComponent<PlayerSaveData>().Autosave(7, true);
        }
        else
        {
            PlayDoorSound(doorLockedSound);
        }
    }
    private void HandleStrorageDoor()
    {
        bool hasStorageKey = InventoryService.Instance != null && InventoryService.Instance.Contains("strorage_key");
        if (hasStorageKey)
        {
            ToggleDoor();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.gameObject.GetComponent<PlayerSaveData>().Autosave(11, true);
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

        PlayDoorSound(isOpen ? doorOpenSound : doorClosedSound);
    }
    public void OnDrawerPuzzleSolved()
    {
        if (doorInteractable == null || doorInteractable.Type != Interactable.InteracType.DirectorDoor)
            return;

        StartCoroutine(TryScare());
    }
    private IEnumerator TryScare()
    {
        yield return new WaitForSeconds(2f);
        if (isOpen)
        {
            isOpen = false;
            PlayDoorSound(doorSlamSound);
            yield return new WaitForSeconds(2f);

            PlayDoorSound(doorKnockSound);

            if (scaryFace != null)
            {
                scaryFace.SetActive(true);
                isFaceActive = true;
            }
        }
        else
        {
            PlayDoorSound(doorKnockSound);

            if (scaryFace != null)
            {
                scaryFace.SetActive(true);
                isFaceActive = true;
            }
        }
    }
    private void TriggerJumpscare()
    {
        if (scaryFace != null)
        {
            scaryFace.SetActive(false);
        }

        PlayDoorSound(screamSound);

        isFaceActive = false;
    }
    private IEnumerator FixDoorProcess()
    {
        isFixing = true;
        if (doorStatusText != null)
        {
            doorStatusText.text = "Đang sửa ...";
        }

        yield return new WaitForSeconds(fixDuration);

        CompleteDoorFix();

        if (doorStatusText != null)
        {
            doorStatusText.text = "Xong";
        }

        yield return new WaitForSeconds(textDisplayDuration);

        if(doorStatusText != null)
        {
            doorStatusText.text = "";
        }
        isFixing = false;
    }
    private void CompleteDoorFix()
    {
        isBroken = false;
        PlayDoorSound(fixDoorSound);
    }
    private void UpdateDoorRotation()
    {
        if (!isInitialized) return;

        var t = doorPivot != null ? doorPivot : transform;

        Vector3 targetEuler = isOpen ? openRotation : defaultRotation;
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        // Slerp với speed đủ nhanh
        t.rotation = Quaternion.Slerp(
            t.rotation,
            targetRotation,
            Time.deltaTime * animationSmooth
        );
    }
    private void UpdateDoorText()
    {
        if (doorStatusText == null || !playerInRange || isFixing)
        {
            return;
        }

        if (doorInteractable == null)
        {
            doorStatusText.text = "";
            return;
        }

        switch (doorInteractable.Type)
        {
            case Interactable.InteracType.DoorMaintenance:
                UpdateMaintenceDoorText();
                break;
            case Interactable.InteracType.DirectorDoor:
                UpdateDirectorDoorText();
                break;
        }
    }
    private void UpdateMaintenceDoorText()
    {
        if(InventoryManager.instance == null)
        {
            doorStatusText.text = "";
            return;
        }

        bool hasScrewdriver = InventoryService.Instance.Contains("screwdriver");
        bool hasKey = InventoryService.Instance.Contains("key_maintenance");

        if (isBroken)
        {
            doorStatusText.text = hasScrewdriver ? "" : "Bị hỏng";
            return;
        }
        if (isLocked || !hasKey)
        {
            doorStatusText.text = hasKey ? "" : "Bị khóa";
        }
        doorStatusText.text = "";
    }
    private void UpdateDirectorDoorText()
    {
        if (InventoryManager.instance == null)
        {
            doorStatusText.text = "";
            return;
        }
        bool hasDirectorKey = InventoryService.Instance.Contains("director_key");

        doorStatusText.text = hasDirectorKey ? "" : "Bị khóa";
    }
    private void PlayDoorSound(AudioClip clip)
    {
        if (doorAudioSource == null || clip == null) return;

        doorAudioSource.PlayOneShot(clip);
    }
    private void OnValidate()
    {
        if (doorAudioSource == null)
        {
            doorAudioSource = GetComponent<AudioSource>();
        }

        if (doorInteractable == null)
        {
            doorInteractable = GetComponent<Interactable>();
        }
    }
}