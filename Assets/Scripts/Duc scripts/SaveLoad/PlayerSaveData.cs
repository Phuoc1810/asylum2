using UnityEngine;

public class PlayerSaveData : MonoBehaviour
{
    public GameObject Player;
    float[] pos = new float[3];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerData data = SaveSystem.Load<PlayerData>();
        if (data != null)
        {
            pos = data.position;
            Player.gameObject.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        }
        else
        {
            Player.gameObject.transform.position = new Vector3(72f, 0.2f, 100f);
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

        PlayerData data = new PlayerData(pos);
        SaveSystem.Save(data);
    }
}
