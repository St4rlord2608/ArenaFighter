using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{

    protected Unit unit;

    protected Action onActionComplete;
    protected Action onActionStart;

    protected bool isActive = false;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }
    public abstract void PerformAction(Action onActionStart, Action onActionComplete);

    public abstract void ActionSelectedVisual();

    protected void ActionStart(Action onActionStart, Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;
        onActionStart();
    }

    protected void ActionComplete()
    {
        isActive = false;
        onActionComplete();
    }
}
