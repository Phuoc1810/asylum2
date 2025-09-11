using System.Collections.Generic;
using UnityEngine;

public class gamemaneger2 : MonoBehaviour
{
    private const bool V = false;
    private const bool v = V;
    [SerializeField] private Transform gameTransfrom;
    [SerializeField] private Transform pieacePrefab;
    public List<Transform> pieces;
    private int emptylocation;
    public int size;
    private bool shuffling = false;
    private int count = 0;
    public float piece1;
    public float piece2;
    public float piece3;
    public float piece4;
    public float piece5;
    public float piece6;
    public float piece7;
    public float piece8;
    public Door unlock;
  
    private void creatGamePice(float gapThickness)
    {
        float width = 1 / (float)size;
        for (int row = 0; row < size; row++)
        {
            for (int col = 0; col < size; col++)
            {
                Transform piece = Instantiate(pieacePrefab, gameTransfrom);
                pieces.Add(piece);
                piece.localPosition = new Vector3(-1 + (2 * width * col) + width,
                                                  +1 - (2 * width * row) - width
                                                  , 0);
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";
                if ((row == size ) && (col == size ))
                {
                    emptylocation = (size * size);
                   // piece.gameObject.SetActive(false);
                }
                else
                {
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];
                    //uv coord order : (0,1) ;(1,1);(0,0);(1,0);
                    uv[0] = new Vector2((width * col) + gap, 1 - ((width * (row + 1)) - gap));
                    uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap, 1 - ((width * row) + gap));
                    uv[3] = new Vector2((width * (col + 1)) - gap, 1 - (width * row) + gap);

                    mesh.uv = uv;
                }
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        pieces = new List<Transform>();

        creatGamePice(0.01f);
       


    }

    // Update is called once per frame
    void Update()
    {
        piece1 = pieces[1].transform.eulerAngles.z;
        piece2 = pieces[2].transform.eulerAngles.z;
        piece3 = pieces[3].transform.eulerAngles.z;
        piece4 = pieces[4].transform.eulerAngles.z;
        piece5 = pieces[5].transform.eulerAngles.z;
        piece6 = pieces[6].transform.eulerAngles.z;
        piece7 = pieces[7].transform.eulerAngles.z;
        piece8 = pieces[8].transform.eulerAngles.z;
        if (checkcompletion())
        {
            unlock.locks = v;
                Debug.Log("complet");
            
        }
        
        if (Input.GetKeyDown(KeyCode.Mouse0)&&!checkcompletion())
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            if (Physics.Raycast(ray, out hit, 3f))

            // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
            // if (hit)
            {
                if (hit.collider.tag == "art")
                {
                    Debug.Log("hitttttttt");
                }
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        pieces[i].Rotate(new Vector3(0, 0, 90));
                        if (pieces[i].transform.eulerAngles.z== 9.659347e-06f)
                        { pieces[i].Rotate(new Vector3(0, 0, -9.659347e-06f)); }


                    }
                }
            }
        }
    }
    private bool checkcompletion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
           if (pieces[i].transform.eulerAngles.z != 0)
         {
            return false;
            }

        }
        //if(pieces[1].transform.eulerAngles.z==0 && pieces[2].transform.eulerAngles.z == 0 && pieces[3].transform.eulerAngles.z == 9.659347e-06 && pieces[4].transform.eulerAngles.z == 0 &&
        //  pieces[5].transform.eulerAngles.z == 0 && pieces[6].transform.eulerAngles.z == 9.659347e-06 && pieces[7].transform.eulerAngles.z == 0 && pieces[8].transform.eulerAngles.z == 9.659347e-06 )
        return true;
       // return false;
    }

}
