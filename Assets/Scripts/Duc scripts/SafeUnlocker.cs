using TMPro;
using UnityEngine;
public class SafeUnlocker : MonoBehaviour
{
    public GameObject SafeTextUI;
    //public GameObject SafeUI;
    public GameObject KeypadUI;
    public GameObject Holder;
    public GameObject[] Lights = new GameObject[3];
    public GameObject keypadLight;
    bool isOpen;
    bool isInteracted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SafeTextUI.SetActive(false);
        KeypadUI.SetActive(false);
        //SafeUI.SetActive(false);
        isOpen = false;
        isInteracted = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteracted && Input.GetKeyDown(KeyCode.P))
        {
            if (!isOpen)
            {
                //SafeUI.SetActive(true);
                SafeTextUI.SetActive(false);
                KeypadUI.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                //Holder.transform.Rotate(15f, 0f, 0f, Space.World);
                Holder.transform.localEulerAngles = new Vector3(15f, 0f, 0f);
                foreach(GameObject light in Lights)
                {
                    light.SetActive(false);
                }
                keypadLight.SetActive(true);
                isOpen = true;
            }
            else
            {
                //SafeUI.SetActive(false);
                SafeTextUI.SetActive(true);
                KeypadUI.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                foreach (GameObject light in Lights)
                {
                    light.SetActive(true);
                }
                keypadLight.SetActive(false);
                isOpen = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "safe")
        {
            SafeTextUI.SetActive(true);
            isInteracted = true;
            Debug.Log("Open safe");
        }
    }

    public void CorrectPassword()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.gameObject.GetComponent<PlayerSaveData>().SetBoolPuzzles(6, true);
        Debug.Log("Safe unlocked");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "safe")
        {
            isInteracted = false;
            SafeTextUI.SetActive(false);
        }
    }
}
