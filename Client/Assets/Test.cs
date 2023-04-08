using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : UnityMultiCallbacks
{
    public string url = "ws://localhost:8080";
    public bool remote =false;
    // Start is called before the first frame update
    void Start()
    {
        if(remote)url="ws://192.168.1.12:8080";
        Connect(url);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
