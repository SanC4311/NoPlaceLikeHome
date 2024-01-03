using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerControl : MonoBehaviour
{
    protected PlayerChar playerChar;
    protected bool isActive;
    protected Action onActionComplete;

    protected virtual void Awake()
    {
        playerChar = GetComponent<PlayerChar>();
    }

}
