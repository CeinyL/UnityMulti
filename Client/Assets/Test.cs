using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : UnityMultiCallbacks
{

    public string url = "ws://localhost:8080";
    User player;

    void Start()
    {
        player = new User();
        player.username = "Piotr";
        Connect(url, player);
        new WaitForSeconds(5f);
        Debug.Log(userData.userId+" | "+ userData.username);
    }

}
