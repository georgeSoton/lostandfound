using System;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SceneChanger : NetworkBehaviour
{
    /// <summary>
    /// Enables verbose debug messages in the console
    /// </summary>
    [FormerlySerializedAs("m_ShowDebugMessages")]
    [Tooltip("This will enable verbose debug messages in the Unity Editor console")]
    public bool showDebugMessages;
    
    /// <summary>
    /// The scene to switch to when offline.
    /// <para>Setting this makes the NetworkManager do scene management. This scene will be switched to when a network session is completed - such as a client disconnect, or a server shutdown.</para>
    /// </summary>
    [Scene]
    [FormerlySerializedAs("m_ListScenes")]
    [Tooltip("A List of all Scenes holding Minigames")]
    public string[] sceneList;

    public static SceneChanger singleton { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
    }

    bool InitializeSingleton()
    {
        if (singleton != null && singleton == this) return true;

        // do this early
        LogFilter.Debug = showDebugMessages;
        if (LogFilter.Debug)
        {
            LogFactory.EnableDebugMode();
        }

        if (singleton != null)
        {
            Destroy(gameObject);

            // Return false to not allow collision-destroyed second instance to continue.
            return false;
        }

        singleton = this;
        if (Application.isPlaying) DontDestroyOnLoad(gameObject);

        return true;
    }

    public void NewRandomScene()
    {
        NetworkManager.singleton.ServerChangeScene(sceneList[Random.Range(0, sceneList.Length)]);
    }
}