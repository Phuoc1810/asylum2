using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class door2 : MonoBehaviour
{
    public bool trig, open;//trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f;//скорость вращения
    public float DoorOpenAngle = -80f;//угол вращения 
    private Vector3 defaulRot;
    private Vector3 openRot;
    public Text txt;//text 
    public bool locks;
    public Transform door;
    public int ID;
    public int number;
    public checkpassword passwork;
    public int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        DoorOpenAngle = -80f;
        locks = true;
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x, defaulRot.y + DoorOpenAngle, defaulRot.z);

    }

    // Update is called once per frame
    void Update()
    {


        if (open)//открыть
        {
            if (count < 1)
            {
                count++;
            }

            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);

            if (ID < 7)
            {
                passwork.checkdoor[number] = ID;


            }
            else if (ID == 10)
            {
                if (count <= 1)
                {
                    passwork.countcheck += 1;
                    count++;

                }

            }
        }
        else//закрыть
        {

            transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaulRot, Time.deltaTime * smooth);

            if (count > +1 && ID == 10)
            {
                passwork.countcheck--;
                count--;

            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, 3f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {

                if (hit.transform == transform)
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
