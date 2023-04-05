using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test2 : UnityMultiNetworkingCallbacks
{
    public string url = "ws://46.205.209.59:8080";
    public long ms;
    // Start is called before the first frame update
    void Start()
    {
        multiNetworking.Connect(url);
    }

    // Update is called once per frame
    void Update()
    {
        this.ms = multiNetworking.latency;
    }

    public override void OnClientConnected()
    {
        base.OnClientConnected();
        Debug.Log("override with base");
    }
}
