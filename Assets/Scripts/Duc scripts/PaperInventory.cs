using UnityEngine;

public class PaperInventory : MonoBehaviour
{
    public static PaperInventory instance;

    private bool[] collectedPieces = new bool[4];

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddPiece(int id)
    {
        collectedPieces[id] = true;

        if (HasAllPieces())
        {
            Debug.Log("All pieces collected!");
            // Trigger UI assembly
            Puzzle_note.instance.ShowAssembledNote();
        }
    }

    public bool[] GetPieces()
    {
        return collectedPieces;
    }

    public bool HasAllPieces()
    {
        foreach (bool piece in collectedPieces)
        {
            if (!piece) return false;
        }
        return true;
    }
}
