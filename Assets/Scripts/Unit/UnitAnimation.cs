using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private const string IS_WALKING = "isWalking";
    private const string IS_ROTATING = "isRotating";

    private MoveAction moveAction;
    private void Awake()
    {
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            this.moveAction = moveAction;
            moveAction.onStartMoving += MoveAction_onStartMoving;
            moveAction.onStopMoving += MoveAction_onStopMoving;
            moveAction.onStartRotating += MoveAction_onStartRotating;
            moveAction.onStopRotating += MoveAction_onStopRotating;
        }
    }

    private void MoveAction_onStopRotating(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_ROTATING, false);
    }

    private void MoveAction_onStartRotating(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_ROTATING, true);
    }

    private void MoveAction_onStopMoving(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_WALKING, false);
    }

    private void MoveAction_onStartMoving(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_WALKING, true);
    }
}
