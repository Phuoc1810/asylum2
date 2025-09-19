using UnityEngine;

public class Interactable : MonoBehaviour
{

    public enum InteracType { Screwdriver, Fuse, ElectricBox, HanldeElectricBox, Door , art,quiz, KeyMaintance, DoorMaintance, BoltCutter, Crowbar}
    [SerializeField] private InteracType interactype;
    [SerializeField] private Animator anim;
    [SerializeField] private AudioClip open;
    [SerializeField] private AudioClip close;

    public InteracType Type => interactype;
    public Animator Anim => anim;
    public AudioClip Open => open;
    public AudioClip Close => close;
}
