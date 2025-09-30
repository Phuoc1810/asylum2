using UnityEngine;
using UnityEngine.UI;
public class DOOR3 : MonoBehaviour
{
    public bool trig, open;//trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f;//скорость вращения
    public float DoorOpenAngle = 35f;//угол вращения 
    public Vector3 defaulRot;
    private Vector3 openRot;
    public Text txt;//text 
    public bool locks;

    public int ID;
    public checklock passwork;
    public int count = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        count = 1;
        DoorOpenAngle = -36.5f;
        locks = true;
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(0f, 0f, 0f);

    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
    public void locknumber()
    {
        openRot.x += DoorOpenAngle;

        count++;

        if (count == 1)
        {
            openRot.x = 0f;
        }
        else if (count > 9)
        {
            count = 0;

        }
        transform.eulerAngles = new Vector3(openRot.x, 0f, 0f);
        passwork.checkdoor[ID]= count;
    }
}
   

