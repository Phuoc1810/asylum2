using UnityEngine;

public class ElectricBoxController : MonoBehaviour
{
    [Header("Electric Box Components")]
    [SerializeField] private GameObject fuseObject;
    [SerializeField] private GameObject[] lights;
    [SerializeField] private Interactable electricBoxHanlde;
    [SerializeField] private DoorManager storageDoor2;

    [Header("State")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private bool hasFuseInstalled = false;

    public bool IsActive => isActive;
    public bool HasFuseInstalled => hasFuseInstalled;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeElectricBox();
    }
    private void InitializeElectricBox()
    {
        if (fuseObject != null)
        {
            fuseObject.SetActive(false);
        }

        TurnOffAllLights();
        if (storageDoor2 == null)
        {
            storageDoor2 = FindStorageDoor2();
        }
    }
    public bool InstallFuse()
    {
        if (InventoryService.Instance == null) { return false; }
        if (!InventoryService.Instance.Contains("fuse")) { return false; }

        if (hasFuseInstalled)
        {
            return false;
        }
        if (fuseObject != null)
        {
            fuseObject.SetActive(true);
        }

        hasFuseInstalled = true;
        InventoryService.Instance.Remove("fuse", 1);

        return true;
    }
    public void ToggleElectricBox()
    {
        if (!hasFuseInstalled)
        {
            if (electricBoxHanlde != null && electricBoxHanlde.Animator != null)
            {
                electricBoxHanlde.Animator.SetTrigger("Close");
            }
            PlaySound(false);
            return;
        }
        isActive = !isActive;

        if (electricBoxHanlde != null && electricBoxHanlde.Animator != null)
        {
            electricBoxHanlde.Animator.SetTrigger("Open");
        }
        if (isActive)
        {
            TurnOnAllLights();
            PlaySound(true);

            OpenStorageDoor2();
        }
        else
        {
            TurnOffAllLights();
            PlaySound(false);
        }
    }
    private void OpenStorageDoor2()
    {
        if (storageDoor2 == null)
        {
            return;
        }

        storageDoor2.ForceOpenDoor();

    }
    private DoorManager FindStorageDoor2()
    {
        DoorManager[] allDoors = FindObjectsOfType<DoorManager>();

        foreach (DoorManager door in allDoors)
        {
            Interactable interactable = door.GetComponent<Interactable>();
            if (interactable != null && interactable.Type == Interactable.InteracType.stroragedoor2)
            {
                Debug.Log("Found Storage Door 2!");
                return door;
            }
        }
        return null;
    }
    private void TurnOnAllLights()
    {
        if (lights == null || lights.Length == 0) return;

        foreach(GameObject lightObj in lights)
        {
            if (lightObj != null)
            {
                lightObj.SetActive(true);
            }
        }
    }
    private void TurnOffAllLights()
    {
        if (lights == null || lights.Length == 0) return;
        foreach(GameObject lightObj in lights)
        {
            if (lightObj != null)
            {
                lightObj.SetActive(false);
            }
        }
    }
    private void PlaySound(bool isOpening)
    {
        if (electricBoxHanlde == null) return;
        AudioClip clip = isOpening ? electricBoxHanlde.OpenSound : electricBoxHanlde.CloseSound;
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
    private void OnValidate()
    {
        if (electricBoxHanlde == null)
        {
            electricBoxHanlde = GetComponent<Interactable>();
        }
    }
}
