using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{

    public bool trig, open;
    public float smooth = 1.0f;
    public float DoorOpenAngle = 90.0f;
    private Vector3 defaulRot;
    private Vector3 openRot;
    public Text txt;
    public bool locks;
    // Start is called before the first frame update
    void Start()
    {
        locks = true;
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        }
        else
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
        }
        if (Input.GetKeyDown(KeyCode.E) && trig && !locks)
        {
            open = !open;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, 2f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {

                if (hit.transform == transform)
                    open = !open;

            }
        }
        if (trig)
        {
            if (open)
            {
                //txt.text = "Close E";
            }
            else
            {
               // txt.text = "Open E";
            }
        }
    }
    //private void OnTriggerEnter(Collider coll)
    //{
    //    if (coll.CompareTag("Player"))
    //    {
         
    //        if (!open)
    //        {
    //            //txt.text = "Close E ";
    //        }
    //        else
    //        {
    //            //txt.text = "Open E";
    //        }
    //        trig = true;
    //    }
    //}
    //private void OnTriggerExit(Collider coll)
    //{
    //    if (coll.CompareTag("Player"))
    //    {
    //        txt.text = " ";
    //        trig = false;
    //    }
    //}
}
