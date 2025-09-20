using UnityEngine;

public class PuzzleUI : MonoBehaviour
{
    public GameObject PaperInventory;
    public GameObject[] Pieces;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < Pieces.Length; i++)
        {
            Pieces[i].SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            bool[] pieces = PaperInventory.gameObject.GetComponent<PaperInventory>().GetPieces();
            for (int i = 0; i < Pieces.Length; i++)
            {
                Pieces[i].SetActive(pieces[i]);
            }
        }
    }
}
