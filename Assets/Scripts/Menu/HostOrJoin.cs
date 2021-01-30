using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class HostOrJoin : MonoBehaviour
{
    [SerializeField]
    public UnityEngine.UI.InputField IF;
    public void Join()
    {
        NetworkManager.singleton.networkAddress = IF.text;
        NetworkManager.singleton.StartClient();
    }

    public void Host()
    {
        NetworkManager.singleton.StartHost();
    }
}
