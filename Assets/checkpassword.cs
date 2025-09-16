using UnityEngine;

public class checkpassword : MonoBehaviour
{
    public int[] passsword = { 1, 2, 3, 4, 5, 6 };
    public int[] checkdoor = { 5, 1, 2, 2, 3, 4 };
    public Door unlock;
    public bool checkwrong ;
    private void Start()
    {
        checkwrong = true;
    }
    private void Update()
    {
       if (check() && checkwrong)
        {
            unlock.locks = false;
        }
    }
    public bool check()
    {
        for (int i = 0; i < passsword.Length; i++)
        {
            //Debug.Log(("point : ", passsword[0], passsword[1], passsword[2], passsword[3], passsword[4], passsword[5]));
            //Debug.Log(("point : ", checkdoor[0], checkdoor[1], checkdoor[2], checkdoor[3], checkdoor[4], checkdoor[5]));
            if (passsword[i] != checkdoor[i]) 
            {
              // Debug.Log( (passsword[i], "", checkdoor[i]));
                //Debug.Log("false");
                return false;

            }
        }
        Debug.Log("true");
        return true;
    }
}
