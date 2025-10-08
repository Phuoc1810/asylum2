using UnityEngine;

public class quiz : MonoBehaviour
{


    public float speed = 5f; // tốc độ di chuyển
    public Transform thiss;
    public float ppp;
    void Update()
    {
        ppp = thiss.position.z;
        // Di chuyển vật thể theo trục Z (phía trước)
        if (transform.position.x >= -24.5)
            transform.Translate(Vector3.forward * -speed * Time.deltaTime);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

 
}
