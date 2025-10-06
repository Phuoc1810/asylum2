using System;

[Serializable]
public class PlayerData
{
    public float[] position;

    public PlayerData(float[] pos)
    {
        position = new float[3];
        position[0] = pos[0];
        position[1] = pos[1];
        position[2] = pos[2];
    }
}
