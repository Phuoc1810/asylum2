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
    public int[] passsword = { 4, 3, 2, 1, 4, 2};
    public int[] checkdoor = { 1, 1, 1, 1, 1, 1};
   
    public GameObject lockss;
    public GameObject cameralock;
    public GameObject lockpannel;
    public Animator anim;
    public lockmaneger lockmaneger;
    public int count = 0;
    public int count2 = 0;
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
        lockpannel = GameObject.FindGameObjectWithTag("notepic").transform.GetChild(count2)?.gameObject;
        //Cursor.lockState = CursorLockMode.Confined;
        //Cursor.visible = true;
        if (check())
        {
            anim.SetBool("open", true);
            Debug.Log("true");
            lockmaneger.close();
            lockss.SetActive(false);
        }
        if (count >= 6 && !check())
        {
            count = 0;
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
          
        }

    }
   public void left()
    {
        if (transform.rotation.x != 90)
        {
            checkdoor[count] = 4;
            count++;
            transform.eulerAngles = new Vector3(90, 0f, 0f);
           
        }
     
    }
    public void upp()
    {
        if (transform.rotation.x != 0)
        {
            checkdoor[count] = 1;
            count++;
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            

        }
     
    }
    public void right()
    {
        if (transform.rotation.x != -90f)
        {
            checkdoor[count] = 2;
            count++;
            transform.eulerAngles = new Vector3(-90f, 0f, 0f);
          
        }
      
    }
    public void downn()
    {
        checkdoor[count] = 3;
        if (transform.rotation.x != -90f)
        {
            count++;
            transform.eulerAngles = new Vector3(180f, 0f, 0f);
          
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
    public void close()
    {
        Debug.Log("aaaaaa");
        lockpannel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameralock.SetActive(false);
        count = 0;
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
      
    }

}
