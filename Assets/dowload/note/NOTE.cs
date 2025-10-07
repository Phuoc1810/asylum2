using UnityEngine;

public class NOTE : MonoBehaviour
{
    public GameObject NOTEPIC;
    public int count;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        NOTEPIC = GameObject.FindGameObjectWithTag("notepic").transform.GetChild(count)?.gameObject;
        if (Input.GetKeyDown(KeyCode.E) )
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, 4f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {
                if (hit.transform == transform)
                {
                    NOTEPIC.SetActive(true);
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
               
            }
        }
    }

    public void close()
    {
        NOTEPIC.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
