using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class RemoveFromPlayer : MonoBehaviour
{
    public void OnPlayerDied()
    {
        print("Hello");
        transform.parent = null;
    }

    protected virtual void OnEnable()
    {
        EventManager.StartListening("PlayerDied", OnPlayerDied);
    }

    protected virtual void OnDisable()
    {
        EventManager.StopListening("PlayerDied", OnPlayerDied);
    }
}
