using UnityEngine;

public class circuit : MonoBehaviour
{
    public Animator animator;
    public Door labdoor;
    public AudioSource AudioSource;
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
            if (Physics.Raycast(ray, out hit, 4f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {
                if (hit.transform == transform)
                {

                    animator.SetTrigger("enter");

                   
                   labdoor.locks=false;
                    Debug.Log("ssssssss");

                }

            }
        }
    }
    public void electric()
    {
        AudioSource.Play();
    }
}
