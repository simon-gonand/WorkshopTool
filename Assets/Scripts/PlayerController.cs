using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Transform self;
    // Start is called before the first frame update
    void Start()
    {
        Transform spawn = GameObject.FindGameObjectWithTag("Player").transform;
        self = spawn;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
