using System.Collections.Generic;
using UnityEngine;

public class WorldStateService : MonoBehaviour
{
    public static WorldStateService Instance { get; private set; }
    [SerializeField] private bool dontDestroyOnLoad = true;
    private readonly HashSet<string> _pickedIds = new HashSet<string>();

    private readonly HashSet<string> _flags = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public void MarkPicked(string worldObjectID)
    {
        if(!string.IsNullOrEmpty(worldObjectID)) _pickedIds.Add(worldObjectID);
    }
    public bool IsPicked(string worldObjectID)
    {
        return !string.IsNullOrEmpty(worldObjectID) && _pickedIds.Contains(worldObjectID);
    }

    public void SetFlag(string key, bool value)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (value) _flags.Add(key);
        else _flags.Remove(key);
    }
    public bool HasFlag(string key) => !string.IsNullOrEmpty(key) && _flags.Contains(key);

    public void Clear()
    {
        _pickedIds.Clear();
        _flags.Clear();
    }
}

