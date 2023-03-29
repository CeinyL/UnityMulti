using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : UnityMultiCallbacks
{
    public string url = "ws://localhost:8080";
    // Start is called before the first frame update
    void Start()
    {
        Connect(url);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
