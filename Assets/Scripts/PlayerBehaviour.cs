using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerBehaviour : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        { 
            var prev = this.transform.position;
        
            var newv = new Vector3(prev.x + Input.GetAxis("Horizontal") * 0.1f,
                                    prev.y + Input.GetAxis("Vertical") * 0.1f,
                                    prev.z);

            this.transform.position = newv;
        }
    }
}
