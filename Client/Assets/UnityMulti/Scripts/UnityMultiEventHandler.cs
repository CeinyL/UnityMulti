using System;
using System.Collections;
using UnityEngine;

public class UnityMultiEventHandler : MonoBehaviour
{
    private UnityMultiNetworking multiNetworking;

    private void Awake()
    {
        multiNetworking = UnityMultiNetworking.Instance;
    }

}
