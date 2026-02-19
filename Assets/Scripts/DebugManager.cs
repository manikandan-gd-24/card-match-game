using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static DebugManager _instance;

    public static DebugManager Instance
    {
        get
        {
            // If instance doesn't exist, create it
            if (_instance == null)
            {
                // Try to find existing one in scene
                _instance = FindObjectOfType<DebugManager>();

                if (_instance == null)
                {
                    // Create new GameObject
                    GameObject go = new GameObject("DebugManager");
                    _instance = go.AddComponent<DebugManager>();

                    DontDestroyOnLoad(go);

                    Debug.Log("DebugManager auto-created.");
                }
            }

            return _instance;
        }
    }

    [SerializeField]
    private bool enableDebug = true;

    private void Awake()
    {
        // Make sure only one exists
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Debug Method (Takes One String)
    public void Log(string message)
    {
        if (!enableDebug) return;

        Debug.Log("[DebugManager] " + message);
    }

}
