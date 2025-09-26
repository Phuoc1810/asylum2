using UnityEngine;

public class lock4 : MonoBehaviour
{
    public bool trig, open;//trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f;//скорость вращения
    public float DoorOpenAngle;//угол вращения 
    public Vector3 defaulRot;
    private Vector3 openRot;
    
    public bool locks;
    public checklock2 passwork;
    public int count = 1;
    public int ID;
    public int pass =1 ;

    // Start is called before the first frame update
    void Start()
    {
        count = 1;
        locks = true;
        defaulRot = transform.eulerAngles;
        openRot = transform.eulerAngles;

    }

    // Update is called once per frame
    void Update()
    {

            

        
    }
    public void locknumber()
    {
        openRot.z += DoorOpenAngle;

        count++;

        if (count == 1)
        {
            openRot.z = 0f;
            pass = 0;
        }
        else if (count > 5)
        {
            count = 0;
            pass = 1;

        }
        transform.eulerAngles = new Vector3(0f, 0f, openRot.z);
        passwork.checkdoor[ID] = pass;
    }
}
