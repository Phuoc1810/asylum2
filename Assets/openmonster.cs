using Unity.VisualScripting;
using UnityEngine;

public class openmonster : MonoBehaviour
{
    public GameObject monster;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("aaaa");
        if(other.CompareTag("Player"))
            monster.SetActive(true);
    }
}
