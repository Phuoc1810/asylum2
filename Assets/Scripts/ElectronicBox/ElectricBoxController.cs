//using UnityEngine;

//public class ElectricBoxController : MonoBehaviour
//{
//    [Header("Electric Box Components")]
//    [SerializeField] private GameObject fuseObject;
//    [SerializeField] private GameObject[] lights;
//    [SerializeField] private Interactable electricBoxHanlde;

//    [Header("State")]
//    [SerializeField] private bool isActive = false;
//    [SerializeField] private bool hasFuseInstalled = false;

//    public bool IsActive => isActive = false;
//    public bool HasFuseInstalled => hasFuseInstalled = false;
//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {
//        InitializeElectricBox();
//    }
//    private void InitializeElectricBox()
//    {
//        if (fuseObject != null)
//        {
//            fuseObject.SetActive(false);
//        }

//        TurnOffAllLights();
//    }
//    public bool InstallFuse()
//    {
//        if (InventoryManager.instance == null)
//        {
//            return false;
//        }
//        if (!InventoryManager.instance.HasItem(Interactable.InteracType.Fuse))
//        {
//            return false;
//        }
//        if (hasFuseInstalled)
//        {
//            return false;
//        }
//        if (fuseObject != null)
//        {
//            fuseObject.SetActive(true);
//        }

//        hasFuseInstalled = true;
//        InventoryManager.instance.RemoveItem(Interactable.InteracType.Fuse);

//        return true;
//    }
//    public void ToggleElectricBox()
//    {
//        if (!hasFuseInstalled)
//        {
//            if (electricBoxHanlde != null && electricBoxHanlde.Animator != null)
//            {
//                electricBoxHanlde.Animator.SetTrigger("Close");
//            }
//            PlaySound(false);
//            return;
//        }
//        isActive = !isActive;

//        if (electricBoxHanlde != null && electricBoxHanlde.Animator != null)
//        {
//            electricBoxHanlde.Animator.SetTrigger("Open");
//        }
//        if (isActive)
//        {
//            TurnOnAllLights();
//            PlaySound(true);
//        }
//        else
//        {
//            TurnOffAllLights();
//            PlaySound(false);
//        }
//    }
//    private void TurnOnAllLights()
//    {
//        if (lights == null || lights.Length == 0) return;

//        foreach(GameObject lightObj in lights)
//        {
//            if (lightObj != null)
//            {
//                lightObj.SetActive(true);
//            }
//        }
//    }
//    private void TurnOffAllLights()
//    {
//        if (lights == null || lights.Length == 0) return;
//        foreach(GameObject lightObj in lights)
//        {
//            if (lightObj != null)
//            {
//                lightObj.SetActive(false);
//            }
//        }
//    }
//    private void PlaySound(bool isOpening)
//    {
//        if (electricBoxHanlde == null) return;
//        AudioClip clip = isOpening ? electricBoxHanlde.OpenSound : electricBoxHanlde.CloseSound;
//        if (clip != null)
//        {
//            AudioSource.PlayClipAtPoint(clip, transform.position);
//        }
//    }
//    private void OnValidate()
//    {
//        if (electricBoxHanlde == null)
//        {
//            electricBoxHanlde = GetComponent<Interactable>();
//        }
//    }
//}
