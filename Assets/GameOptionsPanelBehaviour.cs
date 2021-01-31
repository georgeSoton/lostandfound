using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
public class GameOptionsPanelBehaviour : MonoBehaviour
{
    [SerializeField] List<Toggle> levelSelectToggleList;
    public Dictionary<string, bool> levelSelectMap;

    // Start is called before the first frame update
    void Start()
    {
        
        if(SceneChanger.singleton != null) {
            if (SceneChanger.singleton.sceneList.Length != levelSelectToggleList.Count)
            {
                Debug.LogWarning("The number of level selection checkboxes assigned does not equal the number of minigames availible");
            }
        }

        levelSelectMap = new Dictionary<string, bool>();
        for (int i = 0; i < levelSelectToggleList.Count; i++)
        {
            Toggle toggle = levelSelectToggleList[i]; //Required so delegate does not capture index
            Debug.Log("Adding " + toggle.name + ", " + toggle.isOn);
            levelSelectMap.Add(toggle.name, toggle.isOn);
            toggle.onValueChanged.AddListener(delegate 
            {
                ToggleValueChanged(toggle);
            });

        }
    }

    void ToggleValueChanged(Toggle toggle)
    {
        Debug.Log("Setting " + toggle.name + ", " + toggle.isOn);
        levelSelectMap[name] = toggle.isOn;
        Debug.Log(levelSelectMap[name]);
    }
}
