using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData
{
    public float[] position;
    public bool[] isPuzzleFinished;

    public string[] item_name;
    public int[] item_num;

    public string last_scene_name;

    public PlayerData(float[] pos, bool[] puzzles, string[] item_name, int[] item_num, string last_scene_name)
    {
        position = pos;
        isPuzzleFinished = puzzles;

        this.item_num = item_num;
        this.item_name = item_name;

        this.last_scene_name = last_scene_name;
    }
}
