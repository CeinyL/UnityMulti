using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    public string username { get; set; } = "";
    public string userID { get; private set; } = "";

    public int? validation;

    public void SetUserId(string userId)
    {
        this.userID = userId;
    }
    public void SetUserName(string username)
    {
        this.username = username;
    }

    public User() { }
    public User(string username)
    {
        this.username = username;
    }
}
