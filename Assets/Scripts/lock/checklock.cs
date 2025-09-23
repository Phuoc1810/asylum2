using UnityEngine;

public class checklock : MonoBehaviour
{
    public int[] passsword = { 1, 8, 1, 0, 0};
    public int[] checkdoor = { 1,1,1,1,1 };
    public Animator anim;
    public lockmaneger lockmaneger;
    public GameObject locks;
 

    private void Start()
    {
   
    }
    private void Update()
    {
     
        if (check() )
        {
           anim.SetBool("open",true);
            
            Debug.Log("true");
            lockmaneger.close();
            gameObject.SetActive(false);
        }
    }
    public bool check()
    {
        for (int i = 0; i < passsword.Length; i++)
        {
            
            if (passsword[i] != checkdoor[i])
            {
               
                return false;

            }
        }

        return true;
    }
}
