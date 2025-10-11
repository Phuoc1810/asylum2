using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Door : MonoBehaviour
{

    public bool trig, open;
    public float smooth = 2.0f;
    public float DoorOpenAngle = 90.0f;
    private Vector3 defaulRot;
    private Vector3 openRot;
    private Vector3 lockRot;
    public TextMeshProUGUI txt;
    public bool locks;
    public Animator animator;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip openDoorSound;
    [SerializeField] private AudioClip closeDoorSound;
    [SerializeField] private AudioClip lockedDoorSound;

    private bool lastOpenState;
    // Start is called before the first frame update
    void Start()
    {
        locks=true;
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);
        lockRot = new Vector3(defaulRot.x, defaulRot.y + 4f, defaulRot.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!locks)
            animator.enabled = false;

        if (open != lastOpenState)
        {
            if (open)
            {
                audioSource.PlayOneShot(openDoorSound);
            }
            else
            {
                audioSource.PlayOneShot(closeDoorSound);
            }
            lastOpenState = open;
        }

        // Xoay cửa
        if (open)
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
        else
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);

        //if (Input.GetKeyDown(KeyCode.E) && trig && !locks)
        //{
        //    open = !open;
        //}
        RaycastHit hits;
        Ray rays = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(rays, out hits, 4f))
        {
            if (hits.collider.tag == "door")
            {
                if (open)
                {
                    txt.text = "PRESS E TO Close ";
                    


                }
                else if (!open && transform.eulerAngles.y==0) 
                {
                    txt.text = "PRESS E TO OPEN";
                    Debug.Log("open");

                }
            }
            else if (hits.collider.tag != "door")
            {
                txt.text = " ";

            }

        }
        if (Input.GetKeyDown(KeyCode.E) )
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, 2f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {

                if (hit.transform == transform && !locks)
                {
                  
                    open = !open;
                }
                else if (hit.transform == transform && locks)
                {
                    animator.SetTrigger("lock");
                    audioSource.PlayOneShot(lockedDoorSound);

                }
                  


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
