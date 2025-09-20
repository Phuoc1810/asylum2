using UnityEngine;

public class PaperPieces : MonoBehaviour
{
    public int pieceID; // 0 to 3, so you know which part this is
    public bool isCollected;

    public int GetPieceID()
    {
        return pieceID;
    }
    public void AddPiece()
    {
        PaperInventory.instance.AddPiece(pieceID);
        //Destroy(gameObject); // remove piece from world
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PaperInventory.instance.AddPiece(pieceID);
            Destroy(gameObject); // remove piece from world
        }
    }*/
}
