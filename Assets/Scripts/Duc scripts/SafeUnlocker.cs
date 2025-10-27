using TMPro;
using UnityEngine;
public class SafeUnlocker : MonoBehaviour
{
    //public GameObject SafeUI;
    public GameObject KeypadUI;
    public GameObject Holder;
    public GameObject[] Lights = new GameObject[3];
    public GameObject keypadLight;
    bool isOpen;
    bool isInteracted;

    [SerializeField] private Collider colliderSafeBox;
    GameObject drawer;
    bool isUnlocked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drawer = GameObject.FindGameObjectWithTag("safe_unlock");
        KeypadUI.SetActive(false);
        //SafeUI.SetActive(false);
        isOpen = false;
        isInteracted = false;
        isUnlocked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isInteracted && Input.GetKeyDown(KeyCode.P))
        {
            if (!isOpen)
            {
                //SafeUI.SetActive(true);
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

        if (isUnlocked)
        {
            if (drawer.gameObject.transform.localPosition.x <= 0.7f)
            {
                drawer.gameObject.transform.Translate(Vector3.right * 0.5f * Time.deltaTime, Space.Self);
            }
            colliderSafeBox.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "safe")
        {
            isInteracted = true;
            Debug.Log("Open safe");
        }
    }

    public void CorrectPassword()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.gameObject.GetComponent<PlayerSaveData>().Autosave(5, true);
        isUnlocked = true;
        Debug.Log("Safe unlocked");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "safe")
        {
            isInteracted = false;
        }
    }
}
