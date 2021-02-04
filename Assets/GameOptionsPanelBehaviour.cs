using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mirror;

public class GameOptionsPanelBehaviour : MonoBehaviour
{
    [SerializeField] public List<Toggle> levelSelectToggleList;

    [Server]
    public void SyncToggleState()
    {
        
        if(SceneChanger.singleton != null) {
            if (SceneChanger.singleton.sceneList.Length != levelSelectToggleList.Count)
            {
                Debug.LogWarning("The number of level selection checkboxes assigned does not equal the number of minigames availible");
            }
        }

        SettingsManager.singleton.levelSelectMap.Clear();
        for (int i = 0; i < levelSelectToggleList.Count; i++)
        {
            Toggle toggle = levelSelectToggleList[i]; //Required so delegate does not capture index
            //Debug.Log("Adding " + toggle.name + ", " + toggle.isOn);
            
            SettingsManager.singleton.levelSelectMap.Add(toggle.name, toggle.isOn);
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(delegate 
            {
                ToggleValueChanged(toggle);
            });
        }
    }

    [Server]
    void ToggleValueChanged(Toggle toggle)
    {
        //Debug.Log("Setting " + toggle.name + ", " + toggle.isOn);
        
        SettingsManager.singleton.levelSelectMap[toggle.name] = toggle.isOn;
        //SettingsManager.singleton.levelSelectMap[name] = SettingsManager.singleton.levelSelectMap[name];
        //Debug.Log(SettingsManager.singleton.levelSelectMap[toggle.name]);
    }

    [Client]
    internal void SetToggle(SyncDictionary<string, bool>.Operation op, string name, bool isOn)
    {
        Toggle t = levelSelectToggleList.SingleOrDefault(x => x.name == name);
        if (t != null)
        {
            t.isOn = isOn;
        }
    }
}
