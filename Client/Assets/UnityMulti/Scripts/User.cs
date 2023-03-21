using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    public string username { get; set; }
    public string userId { get; private set; }

    public void SetUserId(string userId)
    {
        this.userId = userId;
    }
    public void SetUserName(string username)
    {
        this.username = username;
    }
}
