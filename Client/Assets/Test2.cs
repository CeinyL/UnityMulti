using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : UnityMultiNetworkingCallbacks
{
    public string url = "ws://localhost:8080";

    public long ms;
    // Start is called before the first frame update
    void Start()
    {   
        multiNetworking.Connect(url);
    }

    // Update is called once per frame
    void Update()
    {
        this.ms = latency;
    }

    private void OnDisable()
    {
        multiNetworking.Disconnect();
    }
}
