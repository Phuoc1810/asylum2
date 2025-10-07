using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    public GameObject Player;
    float[] pos = new float[3];
    bool[] puzzles = new bool[8];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerData data = SaveSystem.Load<PlayerData>();
        if (data != null)
        {
            pos = data.position;
            puzzles = data.isPuzzleFinished;
            Player.gameObject.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        }
        else
        {
            Player.gameObject.transform.position = new Vector3(72f, 0.2f, 100f);
            puzzles = new bool[8] { false, false, false , false, false, false, false, false};
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Save();
        }
    }

    public void Save()
    {
        pos[0] = Player.gameObject.transform.position.x;
        pos[1] = Player.gameObject.transform.position.y;
        pos[2] = Player.gameObject.transform.position.z;

        // Code tam de test bool
        puzzles = new bool[8] { true, false, false, false, false, false, false, false };

        PlayerData data = new PlayerData(pos, puzzles);
        SaveSystem.Save(data);
    }

    // Dung ham nay khi xong puzzle
    public void SetBoolPuzzles(int puzzle_num, bool status)
    {
        puzzles[puzzle_num] = status;
    }
}
