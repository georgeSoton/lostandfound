using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Serialization;

//TODO
public class SettingsManager : NetworkBehaviour
{
    [FormerlySerializedAs("m_ShowDebugMessages")]
    [Tooltip("This will enable verbose debug messages in the Unity Editor console")]
    public bool showDebugMessages;

    public SyncDictionary<string, bool> levelSelectMap = new SyncDictionary<string, bool>();

    public static SettingsManager singleton { get; private set; }
    void Awake()
    {
        InitializeSingleton();
    }

    public bool InitializeSingleton()
    {
        Debug.Log("Initialising Settings manager");
        if (singleton != null && singleton == this) return true;

        LogFilter.Debug = showDebugMessages;
        if (LogFilter.Debug)
        {
            LogFactory.EnableDebugMode();
        }

        if (singleton != null)
        {
            Destroy(gameObject);
            return false;
        }

        singleton = this;
        //levelSelectMap.Callback += OnLevelSelectChange;
        DontDestroyOnLoad(this);
        Debug.Log("Finish initialisation");
        return true;
    }

    void OnLevelSelectChange(SyncDictionary<string, bool>.Operation op, string key, bool toggle)
    {
        Debug.Log("Change in LevelSelectMap: " + key + ", " + toggle);
    }

}
