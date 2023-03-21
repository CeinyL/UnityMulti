using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : UnityMultiCallbacks
{

    public string url = "ws://localhost:8080";
    User ja;

    void Start()
    {
        ja = new User();
        ja.username = "Piotr";
        Connect(url, ja);
        //StartCoroutine(disc(5f));
    }

    IEnumerator disc(float ile)
    {
        yield return new WaitForSeconds(ile);
        Disconnect();
    }
}
