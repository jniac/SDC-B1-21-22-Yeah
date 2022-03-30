using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static List<Player> instances = new List<Player>();

    public static void BroadcastAll(
        string methodName,
        object parameter = null,
        SendMessageOptions options = SendMessageOptions.DontRequireReceiver)
    {
        foreach (var player in instances)
            player.BroadcastMessage(methodName, parameter, options);
    }

    void OnEnable()
    {
        instances.Add(this);
    }

    void OnDisable()
    {
        instances.Remove(this);
    }
}
