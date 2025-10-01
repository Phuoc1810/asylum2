using UnityEngine;

public class piccheck : MonoBehaviour
{
    public int[] picchecks = { 1, 2, 3, 4 };
    public int[] password = { 1, 1, 1, 1 };
    public Door door;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(check())
        {
            door.locks = false;
            Debug.Log("open");
        }
    }
    public bool check()
    {
        for (int i = 0; i < picchecks.Length; i++)
        {

            if (picchecks[i] != password[i])
            {

                return false;

            }
        }
        Debug.Log(true);
        return true;
    }
}
