using UnityEngine;

public class lightmaneger : MonoBehaviour
{
    public Light Light;
    public float count=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       count+=Time.deltaTime;
        if (count > 1)
        {
            Light.enabled=!Light.enabled;
            count=0;

        }
    }
}
