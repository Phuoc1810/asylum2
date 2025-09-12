using UnityEngine;
using UnityEngine.UI;
public class door2 : MonoBehaviour
{
    public bool trig, open;//trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f;//скорость вращения
    public float DoorOpenAngle = -90.0f;//угол вращения 
    private Vector3 defaulRot;
    private Vector3 openRot;
    public Text txt;//text 
    public bool locks;
    public GameObject door;
    public int ID;
    public int number;
    public checkpassword passwork;
    // Start is called before the first frame update
    void Start()
    {
        locks = true;
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);
    }

    // Update is called once per frame
    void Update()
    {

        defaulRot.x = 0;   
        if (open)//открыть
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
            passwork.checkdoor[number] = ID;
        }
        else//закрыть
        {
            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);
            passwork.checkdoor[number] = 10;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, 3f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {
               
                if(hit.transform == transform)
                    open = !open;
                
            }
        }
        if (trig)
        {
            if (open)
            {
                //txt.text = "Close E";
            }
            else
            {
                // txt.text = "Open E";
            }
        }
    }
    private void OnTriggerEnter(Collider coll)//вход и выход в\из  триггера 
    {
        if (coll.CompareTag("Player"))
        {

            if (!open)
            {
                //txt.text = "Close E ";
            }
            else
            {
                //txt.text = "Open E";
            }
            trig = true;
        }
    }
    private void OnTriggerExit(Collider coll)//вход и выход в\из  триггера 
    {
        if (coll.CompareTag("Player"))
        {
            txt.text = " ";
            trig = false;
        }
    }
}
