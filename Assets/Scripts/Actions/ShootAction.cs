using Cinemachine;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    private enum ShootingState
    {
        Aiming,
        Shooting,
        Ending
    }

    [SerializeField] private LayerMask shootableLayer;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationOffset = 5f;
    [Space]
    [SerializeField] private CinemachineVirtualCamera shootingVirtualCam;
    [SerializeField] private Transform shootingCameraAimPointTransform;

    private Unit targetUnit;
    private ShootingState shootingState;

    private float debugTimer = 0;


    protected override void Awake()
    {
        base.Awake();
        SetActionPointsCost(1);
        shootingState = ShootingState.Aiming;
    }

    private void Update()
    {
        if (!isActive)
        {
            shootingVirtualCam.gameObject.SetActive(false);
            return;
        }
        switch (shootingState)
        {
            case ShootingState.Aiming:
                var lookingDir = targetUnit.transform.position - transform.position;
                var lookingRotation = Mathf.Atan2(lookingDir.x, lookingDir.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.x, lookingRotation, transform.rotation.z)), rotationSpeed * Time.deltaTime);
                var angleDifference = ((transform.rotation.eulerAngles.y - lookingRotation) + 360f) % 360;
                if (angleDifference <= rotationOffset || angleDifference >= 360 - rotationOffset)
                {
                    shootingState = ShootingState.Shooting;
                }
                break;
                case ShootingState.Shooting:
                if(debugTimer >= 5)
                {
                    debugTimer = 0;
                    shootingState = ShootingState.Ending;
                }
                else
                {
                    debugTimer += Time.deltaTime;
                }
                break;
                case ShootingState.Ending:
                debugTimer = 0;
                shootingVirtualCam.gameObject.SetActive(false);
                ActionComplete();
                shootingState = ShootingState.Aiming;
                unit.TryToSpendActionPointsToTakeAction(this);
                break;
        }
    }
    public override void ActionSelectedVisual()
    {

    }

    public override string GetActionName()
    {
        return Unit.SHOOT_ACTION;
    }

    public override void PerformAction(Action onActionStart, Action onActionComplete)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, shootableLayer))
        {
            Transform targetUnitTransform = hitInfo.transform;
            if(targetUnitTransform.TryGetComponent<Unit>(out Unit unit))
            {
                targetUnit = unit;
            }
            ActionStart(onActionStart, onActionComplete);
            shootingVirtualCam.gameObject.SetActive(true);
            shootingVirtualCam.Follow = shootingCameraAimPointTransform;
            shootingVirtualCam.LookAt = shootingCameraAimPointTransform;
        }
    }
}
