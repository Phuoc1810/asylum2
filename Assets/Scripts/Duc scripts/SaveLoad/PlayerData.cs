using System;

[Serializable]
public class PlayerData
{
    public float[] position;
    public bool[] isPuzzleFinished;

    public PlayerData(float[] pos, bool[] puzzles)
    {
        position = new float[3];
        position[0] = pos[0];
        position[1] = pos[1];
        position[2] = pos[2];

        isPuzzleFinished = new bool[3];
        isPuzzleFinished = puzzles;
    }
}
