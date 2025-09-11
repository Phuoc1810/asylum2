using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ScrollHealth : MonoBehaviour
{
    Sprite h_Status;
    Image m_Image;

    [SerializeField] float scrollSpeed;
    [SerializeField] Sprite[] healthStatus;
    [SerializeField] GameObject Status;

    void Start()
    {
        Status.GetComponent<Image>().sprite = h_Status;
        m_Image = this.GetComponent<Image>();
        m_Image.color = new Color32(0, 255, 0, 255);
        h_Status = healthStatus[0]; 
    }

    void Update()
    {
        Status.GetComponent<Image>().sprite = h_Status;    
        m_Image.material.mainTextureOffset += new Vector2(Time.deltaTime * (-scrollSpeed / 10), 0f);

       
        if (Input.GetKeyDown(KeyCode.G))
        {
            m_Image.color = new Color32(0, 255, 0, 255); 
            h_Status = healthStatus[0];
        }

       
        if (Input.GetKeyDown(KeyCode.Y))
        {
            m_Image.color = new Color32(255, 255, 0, 255); 
            h_Status = healthStatus[1];
        }

      
        if (Input.GetKeyDown(KeyCode.R))
        {
            m_Image.color = new Color32(255, 0, 0, 255); 
            h_Status = healthStatus[2];
        }
    }
}
