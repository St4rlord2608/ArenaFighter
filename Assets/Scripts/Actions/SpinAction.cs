using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{

    private float totalSpinAmount;

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
        return "Spin";
    }

    public override void PerformAction(Action onActionStart, Action onActionComplete)
    {
        ActionStart(onActionStart, onActionComplete);


    }
}
