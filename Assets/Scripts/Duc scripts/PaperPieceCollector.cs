using UnityEngine;

public class PaperPieceCollector : MonoBehaviour
{
    public GameObject PuzzleTextUI;
    public GameObject InspectItem;
    public GameObject[] PuzzlePieces;

    bool isInteracted;
    bool isCollecting;
    Collider puzzleObj;

    public GameObject Holder;
    public GameObject[] Lights = new GameObject[3];
    public GameObject keypadLight;

    private void Start()
    {
        PuzzleTextUI.SetActive(false);
        isInteracted = false;
        isCollecting = false;
    }

    private void Update()
    {
        if (isInteracted && Input.GetKeyDown(KeyCode.P))
        {
            if (!isCollecting && puzzleObj.gameObject.GetComponent<PaperPieces>().isCollected == false)
            {
                PuzzleTextUI.SetActive(false);
                int id = puzzleObj.gameObject.GetComponent<PaperPieces>().GetPieceID();
                InspectItem.gameObject.GetComponent<ItemInspect>().InspectItem(PuzzlePieces[id]);
                puzzleObj.gameObject.GetComponent<PaperPieces>().AddPiece();
                Debug.Log("Add piece");

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Holder.transform.localEulerAngles = new Vector3(15f, 0f, 0f);
                foreach (GameObject light in Lights)
                {
                    light.SetActive(false);
                }
                keypadLight.SetActive(true);

                isCollecting = true;
            }
            else
            {
                Debug.Log("Exit");
                puzzleObj.gameObject.GetComponent<PaperPieces>().isCollected = true;
                InspectItem.gameObject.GetComponent<ItemInspect>().ExitInspection();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                foreach (GameObject light in Lights)
                {
                    light.SetActive(true);
                }
                PuzzleTextUI.SetActive(true);
                isCollecting = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "puzzle")
        {
            PuzzleTextUI.SetActive(true);
            isInteracted = true;
            puzzleObj = other;
            Debug.Log("collect puzzle");
        }
    }

    /*private void OnTriggerStay(Collider other)
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
    }*/

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "puzzle")
        {
            PuzzleTextUI.SetActive(false);
            isInteracted = false;
        }
    }
}
