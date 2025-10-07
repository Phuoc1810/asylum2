using UnityEngine;

public class lockmaneger : MonoBehaviour
{
    public GameObject lockpannel;
    public GameObject cameralock;
    public int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
     lockpannel = GameObject.FindGameObjectWithTag("notepic").transform.GetChild(count)?.gameObject;  
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, 3f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {

                if (hit.transform == transform)
                {
                    cameralock.SetActive(true);
                    lockpannel.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }


            }
        }
    }
    public void close()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        lockpannel.SetActive(false);
        cameralock.SetActive(false);
    }
}
