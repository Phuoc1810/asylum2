using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSaveData : MonoBehaviour
{
    public GameObject Player;
    public GameObject AutosaveTXT;
    float[] pos = new float[3];
    bool[] puzzles = new bool[11];
    bool[] puzzles_tangham = new bool[5];
    string[] item_name;
    int[] item_num;
    string last_scene_name;
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
            last_scene_name = data.last_scene_name;

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
        else // default data
        {
            //Player.gameObject.transform.position = new Vector3(72f, 0.2f, 100f);
            //Player.gameObject.transform.position = new Vector3(28.7f, 0.47f, 47f);
            //Player.gameObject.transform.position = new Vector3(28f, 0.8f, 47f);
            Player.gameObject.transform.position = new Vector3(14.0600004f, 0.512000024f, 80.1800003f);
            puzzles = new bool[8] { false, false, false , false, false, false, false, false};
            item_name = null;
            item_num = null;
            last_scene_name = "SceneA";
        }
        AutosaveTXT.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Save();
        }
    }

    public void Save()
    {
        pos[0] = Player.gameObject.transform.position.x;
        pos[1] = Player.gameObject.transform.position.y;
        pos[2] = Player.gameObject.transform.position.z;

        Scene currentScene = SceneManager.GetActiveScene();
        last_scene_name = currentScene.name;

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

        PlayerData data = new PlayerData(pos, puzzles, item_name, item_num, last_scene_name);
        SaveSystem.Save(data);

        if (AutosaveTXT != null)
            StartCoroutine(ShowAutosaveTXT());
    }

    private IEnumerator ShowAutosaveTXT()
    {
        AutosaveTXT.SetActive(true);
        yield return new WaitForSeconds(3f);
        AutosaveTXT.SetActive(false);
    }

    // Dung ham nay khi xong puzzle
    public void Autosave(int puzzle_num, bool status)
    {
        puzzles[puzzle_num - 1] = status;
        Save();
    }

    public void AutosaveTangham(int puzzle_num, bool status)
    {
        puzzles_tangham[puzzle_num - 1] = status;
        Save();
    }

    public string GetLastSceneName() => last_scene_name;
}
