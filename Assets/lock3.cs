using UnityEngine;

public class lock3 : MonoBehaviour
{
    public bool trig, open;//trig-проверка входа выхода в триггер(игрок должен быть с тегом Player) open-закрыть и открыть дверь
    public float smooth = 2.0f;//скорость вращения
    public float DoorOpenAngle = -90f;//угол вращения 
    private Vector3 defaulRot;
    private Vector3 openRot;
   
    public bool locks;
    public Transform door;
    public int ID;
    public int number;
    public int[] passsword = { 2, 4, 3, 1, 2, 3};
    public int[] checkdoor = { 1, 1, 1, 1, 1, 1};
    public GameObject picup;
    public GameObject picdown;
    public GameObject picleft;
    public GameObject picright;
    public Animator anim;
    public lockmaneger lockmaneger;
    public int count = 0;
    // Start is called before the first frame update
    void Start()
    {
        
        locks = true;
        defaulRot = transform.eulerAngles;
        openRot = new Vector3(defaulRot.x + DoorOpenAngle, defaulRot.y , defaulRot.z);

    }

    // Update is called once per frame
    void Update()
    {

        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
        if (check())
        {
            anim.SetBool("open", true);
            Debug.Log("true");
            lockmaneger.close();
        }
        if (count >= 6 && !check())
        {
            count = 0;
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            picdown.SetActive(false);
            picleft.SetActive(false);
            picright.SetActive(false);
            picup.SetActive(true);
        }

    }
   public void left()
    {
        if (transform.rotation.x != 90)
        {
            checkdoor[count] = 4;
            count++;
            transform.eulerAngles = new Vector3(90, 0f, 0f);
            picdown.SetActive(false);
            picup.SetActive(false); 
            picleft.SetActive(true);
            picright.SetActive(false);
        }
     
    }
    public void upp()
    {
        if (transform.rotation.x != 0)
        {
            checkdoor[count] = 1;
            count++;
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            picdown.SetActive(false);
            picleft.SetActive(false);
            picright.SetActive(false);
            picup.SetActive(true);

        }
     
    }
    public void right()
    {
        if (transform.rotation.x != -90f)
        {
            checkdoor[count] = 2;
            count++;
            transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            picdown.SetActive(false);
            picleft.SetActive(false);
            picright.SetActive(true);
            picup.SetActive(false);
        }
      
    }
    public void downn()
    {
        checkdoor[count] = 3;
        if (transform.rotation.x != -90f)
        {
            count++;
            transform.eulerAngles = new Vector3(180f, 0f, 0f);
            picdown.SetActive(true);
            picleft.SetActive(false);
            picright.SetActive(false);
            picup.SetActive(false);
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
       Debug.Log(true);
        return true;
    }

}
