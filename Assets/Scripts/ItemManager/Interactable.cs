using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteracType
    {
        // === ITEMS (Có thể nhặt) ===
        Screwdriver,
        Fuse,
        KeyMaintenance,
        BoltCutter,
        Crowbar,
        DirectorKey,
        keytools,
        blackkey,
        locks,
        keymorgue,
        keyinterrogationroom,
        electricroomkey,
        BroadingKey,
        Flashlight,
        Barrtery,
        FileKey,
        XquangKey,
        WCKey,
        KeyStrorage,
        Keyending,

        //=== DOOR ===//
        Door,
        DoorMaintenance,
        DirectorDoor,
        BroadingDoor,
        FileDoor,
        XquangDoor,
        WCDoor,
        DoorStrorage,
        Doorending,

        // === INTERACTIVE OBJECTS ===
        ElectricBox,
        ElectricBoxHandle,
        DirectorDrawers,

        // === INSPECTABLE ITEMS ===
        BoxDirectorKey,
        NoteKnock,
        NoteDrawer,

        ArtPiece,
        Quiz,
        Keypad,
        Newspaper,
        Note1,
        Note2,
        Note3,
        Note4
    }
    [Header("Interaction Setting")]
    [SerializeField] private InteracType type;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Inventory/World Linking")]
    [SerializeField] private string itemId;
    [SerializeField] private string worldObjectId;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip inspectorSound;

    public InteracType Type => type;
    public Animator Animator => animator;
    public AudioClip OpenSound => openSound;
    public AudioClip CloseSound => closeSound;
    public AudioClip InspectorSound => inspectorSound;
    public string ItemId => itemId;
    public string WorldObjectId => worldObjectId;

    /// <summary>
    /// Kiểm tra xem có phải là item có thể nhặt không
    /// </summary>
    public bool IsPickupableItem()
    {
        return type == InteracType.Screwdriver ||
               type == InteracType.Fuse ||
               type == InteracType.KeyMaintenance ||
               type == InteracType.BoltCutter ||
               type == InteracType.Crowbar ||
               type == InteracType.DirectorKey;
    }

    /// <summary>
    /// Kiểm tra có phải là cửa không
    /// </summary>
    public bool IsDoor()
    {
        return type == InteracType.DoorMaintenance ||
               type == InteracType.DirectorDoor ||
               type == InteracType.BroadingDoor;
    }
    /// <summary>
    /// Kiểm tra có phải là inspectable item không
    /// </summary>
    public bool IsInspectable()
    {   
        return type == InteracType.BoxDirectorKey ||
               type == InteracType.NoteDrawer ||
               type == InteracType.NoteKnock;
    }
    /// <summary>
    /// Play audio clip tại vị trí object
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
    }
}
