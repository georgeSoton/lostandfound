using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicToggleBehaviour : MonoBehaviour
{
    [SerializeField] GameObject on;
    [SerializeField] GameObject off;
    [SerializeField] AudioSource source;
    bool state = true;
    public void ToggleState()
    {
        if(state == true)
        {
            source.volume = 0;
            off.SetActive(true);
            on.SetActive(false);
            state = false;
        }
        else
        {
            source.volume = 1;
            on.SetActive(true);
            off.SetActive(false);
            state = true;
        }
        

    }
    // Start is called before the first frame update
    void Start()
    {
        state = true;
        on.SetActive(true);
        off.SetActive(false);
        source.volume = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
