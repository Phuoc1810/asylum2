using System.Collections;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [Header("Door Setting")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float smooth = 2f;
    [SerializeField] private float doorOpenAngle = 90f;
    [SerializeField] private bool requiresScrewdriver = false;
    [SerializeField] private bool requiresKeyMaintance = false;
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

    void Start()
    {
        if (isBreak == true)
        {
            isLooked = true;
            isOpen = false;
        }
        defaultRotation = transform.eulerAngles;
        openRotation = new Vector3(defaultRotation.x, defaultRotation.y + doorOpenAngle, defaultRotation.z);
    }
    void Update()
    {
        UpdateDoorRotation();
        HandleInput();
    }
    private void HandleInput()
    {
        if (Input.GetKey(KeyCode.E) && playerInRange)
        {
            TryOpenDoor();
        }
    }
    private void TryOpenDoor()
    {
        bool hasScrewdriver = InventoryManager.instance.HasItem(Interactable.InteracType.Screwdriver);
        bool hasKeyMaintance = InventoryManager.instance.HasItem(Interactable.InteracType.KeyMaintance);
        if (hasScrewdriver && !hasKeyMaintance)
        {
            requiresScrewdriver = true;
            FixDoor();
        }
        else if (hasScrewdriver)
        {
            if (hasKeyMaintance)
            {
                requiresKeyMaintance = true;
                ToggleDoor();
            }
        }
        else
        {
            Debug.Log("Dont has a screwdriver, get it!");
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
    private void FixDoor()
    {
        isBreak = false;
        PlaySound(fixDoorSound);
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