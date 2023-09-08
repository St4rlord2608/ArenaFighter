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
        Positioning,
        Aiming,
        Shooting,
        Ending
    }

    [SerializeField] private LayerMask shootableLayer;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float rotationOffset = 5f;
    [SerializeField] private float positionTimer = 1f;
    [SerializeField] private float aimTimer = 2f;
    [SerializeField] private float endTimer = 1f;
    [Space]
    [SerializeField] private CinemachineVirtualCamera shootingVirtualCam;
    [SerializeField] private Transform shootingCameraAimPointTransform;
    [Space]
    [SerializeField] private AnimationEventHandler animationEventHandler;

    private Unit targetUnit;
    private ShootingState shootingState;
    private bool isShooting = false;
    private float stateTimer = 0f;

    public event EventHandler onStartShooting;
    public event EventHandler onStopShooting;
    public event EventHandler onStartAiming;
    public event EventHandler onStopAiming;

    protected override void Awake()
    {
        base.Awake();
        SetActionPointsCost(1);
        shootingState = ShootingState.Positioning;
        animationEventHandler.OnFinish += AnimationEventHandler_OnFinish;
    }

    private void AnimationEventHandler_OnFinish(object sender, EventArgs e)
    {
        if(isShooting)
        {
            isShooting = false;
        }
    }

    private void Update()
    {
        Debug.Log(stateTimer);
        if (!isActive)
        {
            shootingVirtualCam.gameObject.SetActive(false);
            return;
        }
        switch (shootingState)
        {
            case ShootingState.Positioning:
                stateTimer += Time.deltaTime;
                var lookingDir = targetUnit.transform.position - transform.position;
                var lookingRotation = Mathf.Atan2(lookingDir.x, lookingDir.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(transform.rotation.x, lookingRotation, transform.rotation.z)), rotationSpeed * Time.deltaTime);
                var angleDifference = ((transform.rotation.eulerAngles.y - lookingRotation) + 360f) % 360;
                if ((angleDifference <= rotationOffset || angleDifference >= 360 - rotationOffset) && stateTimer >= positionTimer)
                {
                    stateTimer = 0;
                    onStartAiming?.Invoke(this, EventArgs.Empty);
                    shootingState = ShootingState.Aiming;
                }
                break;
            case ShootingState.Aiming:
                stateTimer += Time.deltaTime;
                if (stateTimer >= aimTimer)
                {
                    stateTimer = 0;
                    isShooting = true;
                    onStopAiming?.Invoke(this, EventArgs.Empty);
                    onStartShooting?.Invoke(this, EventArgs.Empty);
                    shootingState = ShootingState.Shooting;
                }
                break;
                case ShootingState.Shooting:
                if(!isShooting)
                {
                    onStopShooting?.Invoke(this, EventArgs.Empty);
                    shootingState = ShootingState.Ending;
                    UnitHealth targetUnitHealth = targetUnit.GetComponent<UnitHealth>();
                    targetUnitHealth.Damage(1);
                }
                break;
                case ShootingState.Ending:
                stateTimer += Time.deltaTime;
                if(stateTimer >= endTimer)
                {
                    stateTimer = 0;
                    shootingVirtualCam.gameObject.SetActive(false);
                    ActionComplete();
                    shootingState = ShootingState.Aiming;
                    unit.TryToSpendActionPointsToTakeAction(this);
                }
                
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
