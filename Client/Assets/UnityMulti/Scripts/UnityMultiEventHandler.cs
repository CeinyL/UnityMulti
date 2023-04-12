using System.Collections;
using UnityEngine;

public class UnityMultiEventHandler : MonoBehaviour
{
    private UnityMultiNetworking multiNetworking;

    public float ReconnectDelaySeconds = 10f;
    public int maxReconnectAttempt = 10;
    private int reconnectAttempt = 0;

    private void Awake()
    {
        multiNetworking = UnityMultiNetworking.Instance;
        multiNetworking.Reconnect += OnReconnect;
    }

    private void OnReconnect(string url, User clientData, bool _isReconnecting)
    {
        StartCoroutine(ReconnectCoroutine(url, clientData, _isReconnecting));
    }

    private IEnumerator ReconnectCoroutine(string url, User clientData, bool _isReconnecting)
    {
        while (_isReconnecting && reconnectAttempt < maxReconnectAttempt && !multiNetworking.IsConnected)
        {
            Debug.Log("Attempting to reconnect... " + (reconnectAttempt + 1) + "/" + maxReconnectAttempt);
            multiNetworking.Connect(url, clientData.username);

            yield return new WaitForSeconds(ReconnectDelaySeconds);
            reconnectAttempt++;
        }

        if (reconnectAttempt >= maxReconnectAttempt)
        {
            Debug.LogWarning("Reached max reconnect attempts: " + maxReconnectAttempt);
        }

        multiNetworking.StopReconnecting();
        reconnectAttempt = 0;
    }

}
