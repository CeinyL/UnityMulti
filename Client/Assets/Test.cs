using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : UnityMultiCallbacks
{
    public ConnectionHandler con;
    public bool remote=false;
    public string url = "ws://localhost:8080";
    User player;

    void Start()
    {
        player = new User();
        player.username = "Piotr";
        if(remote)url="ws://192.168.1.12:8080";
        Connect(url, player);
    }
    
}
