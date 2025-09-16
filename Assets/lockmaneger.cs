using UnityEngine;

public class lockmaneger : MonoBehaviour
{
    public GameObject lockpannel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
