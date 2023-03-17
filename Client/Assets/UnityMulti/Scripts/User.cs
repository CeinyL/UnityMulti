using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string username { get; private set; }
    public string userId { get; private set; }

    public void SetUsername(string newUsername)
    {
        username = newUsername;
    }

    public void SetId(string newUserId)
    {
        userId = newUserId;
    }

    public void SetUserData(string username, string id)
    {
        this.username = username;
        this.userId = id;
    }

    public User()
    {
        this.username = "";
        this.userId = "";
    }

    public User(string username, string id)
    {
        this.username = username;
        this.userId = id;
    }
}
