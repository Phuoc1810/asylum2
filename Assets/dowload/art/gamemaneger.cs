using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gamemaneger : MonoBehaviour
{
    [SerializeField] private Transform gameTransfrom;
    [SerializeField] private Transform pieacePrefab;
    private List<Transform> pieces;
    private int emptylocation;
    public int size;
    private bool shuffling = false;
    private int count = 0;
    public Door unlock;
    private void creatGamePice(float gapThickness)
    {
        float width = 1/(float)size;
        for (int row =0; row < size; row++)
        {
            for (int col =0; col < size; col++)
            {
                Transform piece = Instantiate(pieacePrefab, gameTransfrom);
                pieces.Add(piece);
                piece.localPosition = new Vector3(-1+(2*width*col)+width,
                                                  +1-(2*width*row)-width
                                                  ,0);
                piece.localScale = ((2 * width) - gapThickness) * Vector3.one;
                piece.name = $"{(row * size) + col}";
                if ((row == size - 1) && (col == size - 1))
                {
                    emptylocation = (size * size) - 1;
                    piece.gameObject.SetActive(false);
                }
                else
                {
                    float gap = gapThickness / 2;
                    Mesh mesh = piece.GetComponent<MeshFilter>().mesh;
                    Vector2[] uv = new Vector2[4];
                    //uv coord order : (0,1) ;(1,1);(0,0);(1,0);
                    uv[0] = new Vector2((width*col)+gap,1-((width*(row+1))-gap));
                    uv[1] = new Vector2((width * (col + 1)) - gap, 1 - ((width * (row + 1)) - gap));
                    uv[2] = new Vector2((width * col) + gap,1-((width*row)+gap));
                    uv[3] = new Vector2((width*(col+1)) - gap,1-(width*row)+gap);

                    mesh.uv = uv;
                }
            }
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pieces = new List<Transform> ();
        
        creatGamePice(0.01f);
    }

    // Update is called once per frame
    void Update()
    {
        if(!shuffling && checkcompletion())
        {
            if (count == 0)
            {
                shuffling = true;
                count = 1;
                StartCoroutine(WaitShuffle(0.5f));

            }
            else
            {
                unlock.locks = false;
                Debug.Log("complet");
            }
        }
        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            Ray ray = new Ray(Camera.main.transform.position,Camera.main.transform.forward);
            if(Physics.Raycast(ray, out hit,3f))

           // RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, Camera.main.transform.forward);
           // if (hit)
            {
                if(hit.collider.tag == "art")
                {
                    Debug.Log("hitttttttt");
                }
                for (int i = 0; i < pieces.Count; i++)
                {
                    if (pieces[i] == hit.transform)
                    {
                        if (Swapifvalid(i, -size, size)) { break; }
                        if (Swapifvalid(i, +size,size)) { break; }
                        if (Swapifvalid(i, -1, 0)){ break; }
                        if (Swapifvalid(i, +1, size - 1)) { break; }

                    }
                }
            }
        }
    }


    private bool checkcompletion()
    {
        for (int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].name != $"{i}")
            {
                return false;
            }

        }
        return true;
    }
    private IEnumerator WaitShuffle (float duration)
    {
        yield return new WaitForSeconds(duration);
        shuffle();
        shuffling = false;
    }
    private void shuffle()
    {
        int count = 0;
        int last = 0;
        while(count <(size*size*size))
        {
            int rnd = Random.Range(0, size*size);
            if(rnd == last) { continue; }
            last = emptylocation;
            if (Swapifvalid(rnd, -size, size)) { count++; }
            else if (Swapifvalid(rnd, +size, size)) { count++; }
            else if (Swapifvalid(rnd, -1, 0)) {  count++; }
            else if (Swapifvalid(rnd, +1, size - 1)) {  count++; }
        }
    }
    private bool Swapifvalid(int i, int offset,int colcheck)
    {
        if (((i % size) != colcheck) && ((i + offset) == emptylocation))
        {
            (pieces[i], pieces[i + offset]) = (pieces[i + offset], pieces[i]);
            (pieces[i].localPosition, pieces[i + offset].localPosition) = ((pieces[i + offset].localPosition, pieces[i].localPosition));
            emptylocation = i;
            return true;
        }
        return false;
        
    }
}
