using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class SceneChanger : MonoBehaviour
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

    private List<String> _scenes;

    public static SceneChanger singleton { get; private set; }

    private void Awake()
    {
        InitializeSingleton();
        _scenes = new List<string>(sceneList);
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
        var newScene = _scenes[Random.Range(0, _scenes.Count)];
        _scenes.Remove(newScene);
        if (_scenes.Count<=0)
        {
            _scenes.AddRange(sceneList);
            _scenes.Remove(newScene); //prevent selecting the same scene twice
        }
        NetworkManager.singleton.ServerChangeScene(newScene);
    }
}