using UnityEngine;

public class XQuang_doc : MonoBehaviour
{
    public GameObject DocTextUI;
    public GameObject DocUI;
    bool isOpen;
    bool isInteracted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DocTextUI.SetActive(false);
        DocUI.SetActive(false);
        isOpen = false;
        isInteracted = false;
    }

    void Update()
    {
        if (isInteracted && Input.GetKeyDown(KeyCode.P))
        {
            if (!isOpen)
            {
                DocUI.SetActive(true);
                DocTextUI.SetActive(false);
                isOpen = true;
            }
            else
            {
                DocUI.SetActive(false);
                DocTextUI.SetActive(true);
                isOpen = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "doc")
        {
            DocTextUI.SetActive(true);
            isInteracted = true;
            Debug.Log("Open doc");
        }
    }

    /*private void OnTriggerStay(Collider other)
    {
        if (other.tag == "doc" && Input.GetKeyDown(KeyCode.E))
        {
            if (!isOpen)
            {
                DocUI.SetActive(true);
                DocTextUI.SetActive(false);
                isOpen = true;
            }
            else
            {
                DocUI.SetActive(false);
                DocTextUI.SetActive(true);
                isOpen = false;
            }
        }
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "doc")
        {
            isInteracted = false;
            DocTextUI.SetActive(false);
        }
    }
}
