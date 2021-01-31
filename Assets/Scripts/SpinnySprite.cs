using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnySprite : MonoBehaviour
{   
    public float SpinSpeed = 50f;
    void Update()
    {
        transform.Rotate(new Vector3(0,0,SpinSpeed*Time.deltaTime));
    }
}
