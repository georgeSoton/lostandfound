using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class SyncSettings : NetworkBehaviour
{
    //This Script is needed to prevent the panel object requiring a network Identity
    [SerializeField]
    public GameOptionsPanelBehaviour optionsPanel;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            optionsPanel.SyncToggleState();
        }
        if (isClientOnly)
        {
            SettingsManager.singleton.levelSelectMap.Callback += optionsPanel.SetToggle;
            foreach (Toggle t in optionsPanel.levelSelectToggleList)
            {
                t.interactable = false;
            }
        }
    }

    public override void OnStartClient()
    {
        foreach (var p in SettingsManager.singleton.levelSelectMap)
        {
            optionsPanel.SetToggle(SyncDictionary<string, bool>.Operation.OP_SET, p.Key, p.Value);
        }
    }
}
