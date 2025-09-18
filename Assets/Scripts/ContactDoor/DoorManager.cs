using System.Collections;
using TMPro;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    private InteractableController controller;

    void Start()
    {
        controller = FindObjectOfType<InteractableController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && controller != null)
        {
            controller.SetDoorPlayerInRange(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && controller != null)
        {
            controller.SetDoorPlayerInRange(false);
        }
    }
}