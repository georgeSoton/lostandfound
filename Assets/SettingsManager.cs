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
    
    [SyncVar]
    public SyncDictionary<string, bool> levelSelectMap = new SyncDictionary<string, bool>();

    public static SettingsManager singleton { get; private set; }

    public override void OnStartClient()
    {
        base.OnStartClient();
        InitializeSingleton();
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        InitializeSingleton();
    }

    bool InitializeSingleton()
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
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);

        Debug.Log("Finish initialisation");
        return true;
    }

    private void Awake()
    {
        levelSelectMap = new SyncDictionary<string, bool>();
    }

}
