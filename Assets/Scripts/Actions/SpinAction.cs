using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{

    private float totalSpinAmount;

    protected override void Awake()
    {
        base.Awake();
        SetActionPointsCost(2);
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }
        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;

        if(totalSpinAmount >= 360f)
        {
            totalSpinAmount = 0;
            ActionComplete();
        }
    }
    public override void ActionSelectedVisual()
    {
        
    }

    public override string GetActionName()
    {
        return Unit.SPIN_ACTION;
    }

    public override void PerformAction(Action onActionStart, Action onActionComplete)
    {
        ActionStart(onActionStart, onActionComplete);
        unit.TryToSpendActionPointsToTakeAction(this);
    }
}
