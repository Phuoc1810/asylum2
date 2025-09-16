using UnityEngine;

public class Interactable : MonoBehaviour
{

    public enum InteracType { Screwdriver, Sparkplug, Door , art,quiz, KeyMaintance}
    [SerializeField] private InteracType interactype;
    [SerializeField] private Animator anim;

    public InteracType Type => interactype;
    public Animator Anim => anim;
}
