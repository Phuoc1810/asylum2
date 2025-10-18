using UnityEngine;

public class checkquiz : MonoBehaviour
{
    public bool checkquiz1;
    public bool checkquiz2;
    public bool checkquiz3;
    public bool checkquiz4;
    public bool checkquiz5;
    public checkpassword checklock1;
    
    public piccheck checklock2;
    public lock3 checklock3;
    public checklock checklock4;
    public checklock2 checklock5;

    private void Update()
    {
        if (checklock1.check()==true && checklock1.checkwrong==true)
        { checkquiz1 = true; }
        else
        {
            checkquiz1 = false;
        }
        checkquiz2 = checklock2.check();
        checkquiz3 = checklock3.check();
        checkquiz4 = checklock4.check();
        checkquiz5 = checklock5.check();
    }
}
