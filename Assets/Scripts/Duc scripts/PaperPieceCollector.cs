using UnityEngine;

public class PaperPieceCollector : MonoBehaviour
{
    public GameObject PuzzleTextUI;
    public GameObject InspectItem;
    public GameObject[] PuzzlePieces;

    bool isCollected;
    private void Start()
    {
        PuzzleTextUI.SetActive(false);
    }

    private void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "puzzle")
        {
            PuzzleTextUI.SetActive(true);
            Debug.Log("collect puzzle");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "puzzle" && Input.GetKeyDown(KeyCode.E) && other.gameObject.GetComponent<PaperPieces>().isCollected == false)
        {
            PuzzleTextUI.SetActive(false);
            int id = other.gameObject.GetComponent<PaperPieces>().GetPieceID();
            InspectItem.gameObject.GetComponent<ItemInspect>().InspectItem(PuzzlePieces[id]);
            other.gameObject.GetComponent<PaperPieces>().AddPiece();
            Debug.Log("Add piece");
            other.gameObject.GetComponent<PaperPieces>().isCollected = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "puzzle")
        {
            PuzzleTextUI.SetActive(false);
        }
    }
}
