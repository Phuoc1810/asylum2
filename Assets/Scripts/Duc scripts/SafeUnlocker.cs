using TMPro;
using UnityEngine;
public class SafeUnlocker : MonoBehaviour
{
    public GameObject SafeTextUI;
    public GameObject SafeUI;
    bool isOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SafeTextUI.SetActive(false);
        SafeUI.SetActive(false);
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "safe")
        {
            SafeTextUI.SetActive(true);
            Debug.Log("Open safe");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "safe" && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                SafeUI.SetActive(true);
                SafeTextUI.SetActive(false);
                isOpen = true;
            }
            else
            {
                SafeUI.SetActive(false);
                SafeTextUI.SetActive(true);
                isOpen = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "safe")
        {
            SafeTextUI.SetActive(false);
        }
    }
}
