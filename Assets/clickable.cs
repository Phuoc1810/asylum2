using UnityEngine;
using UnityEngine.UI;
public class clickable : MonoBehaviour
{
    public float alphathreshold = 0.1f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold = alphathreshold;
        Debug.Log("aa0");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
