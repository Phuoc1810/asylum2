using UnityEngine;

public class pipesscrip : MonoBehaviour
{
    public float[] rotation = { 0, 90, 180, 270 };
    private void onmousedown()
    {
        transform.Rotate(new Vector3(0, 0, 90));
    }
    private void Start()
    {
        int rand = Random.Range(0, rotation.Length);
        transform.Rotate(new Vector3(0, 0, rotation[rand]));
    }
}
