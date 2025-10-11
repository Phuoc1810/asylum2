using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    public GameObject Player;
    float[] pos = new float[3];
    bool[] puzzles = new bool[8];
    string[] item_name;
    int[] item_num;
    GameObject Inventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerData data = SaveSystem.Load<PlayerData>();
        Inventory = GameObject.FindGameObjectWithTag("InventoryService");
        if (data != null)
        {
            pos = data.position;
            puzzles = data.isPuzzleFinished;
            Player.gameObject.transform.position = new Vector3(pos[0], pos[1], pos[2]);

            if (data.item_name != null && data.item_num != null)
            {
                item_name = data.item_name;
                item_num = data.item_num;

                for (int i = 0; i < item_name.Length; i++)
                {
                    Inventory.gameObject.GetComponent<InventoryService>().Add(item_name[i], item_num[i]);
                }
            }
            else
            {
                item_name = null;
                item_num = null;
            }
        }
        else
        {
            //Player.gameObject.transform.position = new Vector3(72f, 0.2f, 100f);
            Player.gameObject.transform.position = new Vector3(-27f, 8.48f, 16f);
            puzzles = new bool[8] { false, false, false , false, false, false, false, false};
            item_name = null;
            item_num = null;
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

        Dictionary<string, int> items = Inventory.gameObject.GetComponent<InventoryService>().GetDict();
        Debug.Log(items.Count);

        if (items.Count != 0)
        {
            item_name = new string[items.Count];
            item_num = new int[items.Count];
            int i = 0;
            foreach (var  item in items)
            {
                item_name[i] = item.Key;
                item_num[i] = item.Value;
                i++;
            }
        }
        else
        {
            item_name = null;
            item_num = null;
        }

        PlayerData data = new PlayerData(pos, puzzles, item_name, item_num);
        SaveSystem.Save(data);
    }

    // Dung ham nay khi xong puzzle
    public void SetBoolPuzzles(int puzzle_num, bool status)
    {
        puzzles[puzzle_num] = status;
    }
}
