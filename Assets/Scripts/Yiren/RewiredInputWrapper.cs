using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[System.Serializable]
public class RewiredInputWrapper {
    [SerializeField]
    private string actionName;
    
    [HideInInspector]
    public int playerID;

    public RewiredInputWrapper(string actionName)
    {
        this.actionName = actionName;
    }

    public bool GetButton()
    {
        if (!ReInput.isReady) 
            return false;

        return ReInput.players.GetPlayer(playerID).GetButton(actionName);
    }

    public bool GetButtonDown()
    {
        if (!ReInput.isReady) 
            return false;

        return ReInput.players.GetPlayer(playerID).GetButtonDown(actionName);
    }

    public bool GetButtonUp()
    {
        if (!ReInput.isReady) 
            return false;

        return ReInput.players.GetPlayer(playerID).GetButtonUp(actionName);
    }

    public float GetAxis()
    {
        if (!ReInput.isReady) 
            return 0.0f;

        return ReInput.players.GetPlayer(playerID).GetAxis(actionName);
    }

    public float GetAxisRaw()
    {
        if (!ReInput.isReady) 
            return 0.0f;

        return ReInput.players.GetPlayer(playerID).GetAxisRaw(actionName);
    }

    public bool GetDoubleButtonDown(float inputTime = 1.0f)
    {
        if (!ReInput.isReady) 
            return false;

        return ReInput.players.GetPlayer(playerID).GetButtonDoublePressDown(actionName, inputTime);
    }
	
}