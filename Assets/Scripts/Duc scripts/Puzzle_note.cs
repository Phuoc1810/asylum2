using UnityEngine;

public class Puzzle_note : MonoBehaviour
{
    public static Puzzle_note instance;

    public GameObject assembledNoteUI; // Drag your UI Panel or 3D Note Object
    public Transform note3D;           // The 3D note object

    private void Awake()
    {
        instance = this;
        assembledNoteUI.SetActive(false);
    }

    public void ShowAssembledNote()
    {
        assembledNoteUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // pause game if needed
    }

    public void HideAssembledNote()
    {
        assembledNoteUI.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (assembledNoteUI.activeSelf)
        {
            float rotX = Input.GetAxis("Mouse X") * 5f;
            float rotY = -Input.GetAxis("Mouse Y") * 5f;
            note3D.Rotate(rotY, rotX, 0, Space.World);
        }
    }
}
