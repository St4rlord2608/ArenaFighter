using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private const string IS_WALKING = "isWalking";
    private const string IS_ROTATING = "isRotating";
    private const string IS_SHOOTING = "isShooting";
    private const string IS_AIMING = "isAiming";

    private MoveAction moveAction;
    private ShootAction shootAction;
    private void Awake()
    {
        if(TryGetComponent<MoveAction>(out moveAction))
        {
            moveAction.onStartMoving += MoveAction_onStartMoving;
            moveAction.onStopMoving += MoveAction_onStopMoving;
            moveAction.onStartRotating += MoveAction_onStartRotating;
            moveAction.onStopRotating += MoveAction_onStopRotating;
        }
        if(TryGetComponent<ShootAction>(out shootAction))
        {
            shootAction.onStartShooting += ShootAction_onStartShooting;
            shootAction.onStopShooting += ShootAction_onStopShooting;
            shootAction.onStartAiming += ShootAction_onStartAiming;
        }
    }

    private void ShootAction_onStartAiming(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_AIMING, true);
    }

    private void ShootAction_onStopShooting(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_SHOOTING, false);
    }

    private void ShootAction_onStartShooting(object sender, System.EventArgs e)
    {
        animator.SetBool(IS_SHOOTING, true);
        animator.SetBool(IS_AIMING, false);
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
