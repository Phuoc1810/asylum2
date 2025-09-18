using System.Collections;
using TMPro;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Setting")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float smooth = 2f;
    [SerializeField] private float doorOpenAngle = 90f;
    [SerializeField] private bool isLooked = true;
    [SerializeField] private bool isBreak = true;

    [Header("Audio Setting")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fixDoorSound;
    [SerializeField] private AudioClip doorOpenSound;
    [SerializeField] private AudioClip lockedSound;
    [SerializeField] private AudioClip closedSound;

    private Vector3 defaultRotation;
    private Vector3 openRotation;
    private bool playerInRange = false;
    [SerializeField] private TextMeshProUGUI text;

    void Start()
    {
        if (isBreak == true)
        {
            isLooked = true;
            isOpen = false;
        }
        defaultRotation = transform.eulerAngles;
        openRotation = new Vector3(defaultRotation.x, defaultRotation.y + doorOpenAngle, defaultRotation.z);
        text.text = "";
    }
    void Update()
    {
        UpdateDoorRotation();
        HandleInput();
        UpdateText();
    }
    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0) && playerInRange && !isBreak)
        {
            TryOpenDoor();
        }

        else if (Input.GetMouseButtonDown(0) && playerInRange && isBreak)
        {
            TryFIxDoor();
        }
    }
    private void TryOpenDoor()
    {
        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        bool hasKeyMaintance = InventoryManager.instance.HasItem(Interactable.InteracType.KeyMaintance);
        
         if (hasScrewdriver)
        { 
            if (hasKeyMaintance)
            {
                ToggleDoor();
            }
            else
            {
                PlaySound(lockedSound);
            }
        }
        else
        {
            PlaySound(lockedSound);
            Debug.Log("Dont has a screwdriver, get it!");
        }
    }
    private void TryFIxDoor()
    {
        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        
        if (hasScrewdriver )
        {
            StartCoroutine(FixDoorProcess());
        }
        else
        {
            PlaySound(lockedSound);
        }
    }
    private void ToggleDoor()
    {
        isLooked = false;
        isOpen = !isOpen;
        if (isOpen == false)
        {
            PlaySound(closedSound);
        }
        else
        {
            PlaySound(doorOpenSound);
        }
    }
    private IEnumerator FixDoorProcess()
    {
        text.text = "Đang sửa";
        yield return new WaitForSeconds(2f);

        FixDoor();
        text.text = "Xong!";

        yield return new WaitForSeconds(1f);

        UpdateText();
    }
    private void FixDoor()
    {
        isBreak = false;
        PlaySound(fixDoorSound);
    }
    private void UpdateText()
    {
        if (!playerInRange)
        {
            text.text = "";
            return;
        }
        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        bool hasKeyMaintance = InventoryManager.instance.HasItem(Interactable.InteracType.KeyMaintance);

        if (isBreak)
        {
            if (!hasScrewdriver)
            {
                text.text = "Cửa bị hỏng";
            }
        }
        else
        {
            if (!hasKeyMaintance)
            {
                text.text = "Đã bị khóa";
            }
        }

    }
    private void UpdateDoorRotation()
    {
        Vector3 targetRotation = isOpen ? openRotation : defaultRotation;
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * smooth);
    }
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

}