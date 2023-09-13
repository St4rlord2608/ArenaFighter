using Cinemachine;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootingPoint;
    [SerializeField] private Transform detectionStartPoint;
    [Space]
    [SerializeField] private LayerMask detectionLayerMask;

    private Unit targetUnit;
    private ShootingState shootingState;
    private bool isShooting = false;
    private bool shotHitsTarget = false;
    private float stateTimer = 0f;
    private List<Transform> seeableEnemieList = new List<Transform>();
    private SelectedVisual lastSelectedVisual;
    private ShootButtonUI lastShootButtonUI;

    public event EventHandler onStartShooting;
    public event EventHandler onStopShooting;
    public event EventHandler onStartAiming;
    public event EventHandler onStopAiming;

    protected override void Awake()
    {
        base.Awake();
        SetActionPointsCost(1);
        shootingState = ShootingState.Positioning;
        
    }

    private void Start()
    {
        animationEventHandler.OnFinish += AnimationEventHandler_OnFinish;
        animationEventHandler.OnShoot += AnimationEventHandler_OnShoot;
        UnitActionManager.Instance.onSelectedActionChanged += UnitActionManager_onSelectedActionChanged;
    }

    private void UnitActionManager_onSelectedActionChanged(object sender, EventArgs e)
    {
        ResetLastSelectedVisual();
        if (UnitActionManager.Instance.GetSelectedAction() == this)
        {
            seeableEnemieList.Clear();
            Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, Mathf.Infinity);
            foreach(Collider collider in nearbyColliders)
            {
                if(collider.gameObject.layer == UnitActionManager.Instance.GetEnemyLayerIndex())
                {
                    bool seesEnemy = false;
                    float detectionAmount = 0;
                    Unit enemyUnit = collider.transform.GetComponent<Unit>();
                    enemyUnit.ResetSeeableDetectionPointList();
                    foreach(Transform detectionPositionTransform in enemyUnit.GetDetectionPositionContainer())
                    {
                        Physics.Linecast(detectionStartPoint.position, detectionPositionTransform.position, out RaycastHit hitInfo, detectionLayerMask);
                        if (hitInfo.transform == collider.transform)
                        {
                            seesEnemy = true;
                            detectionAmount++;
                            enemyUnit.AddSeeableDetectionPointToList(detectionPositionTransform);
                        }
                    }
                    if (seesEnemy)
                    {
                        CalculateHitChance(detectionAmount, enemyUnit);
                        seeableEnemieList.Add(collider.transform);
                    }
                    else
                    {
                        CalculateHitChance(0, enemyUnit);
                    }
                }
            }
            UnitShootSystemUI.Instance.CreateShootButtons(seeableEnemieList);
        }
    }

    public override void MouseOverUI()
    {
        ResetLastSelectedVisual();
    }

    private void AnimationEventHandler_OnShoot(object sender, EventArgs e)
    {
        if(isShooting)
        {
            Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootingPoint.position, Quaternion.identity);
            BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
            if (shotHitsTarget)
            {
                List<Transform> detectionPointList = targetUnit.GetSeeableDetectionPointList();
                Vector3 minHitPos = detectionPointList[0].position;
                Vector3 maxHitPos = detectionPointList[detectionPointList.Count - 1].position;
                Vector3 shootPos = new Vector3(Random.Range(minHitPos.x, maxHitPos.x), Random.Range(minHitPos.y, maxHitPos.y), Random.Range(minHitPos.z, maxHitPos.z));
                bulletProjectile.Setup(shootPos);
            }
            else
            {
                bulletProjectile.Setup(targetUnit.GetTargetPoint().position + new Vector3(Random.Range(10f, -10f), Random.Range(10f, -10), 0));
            }
            
        }
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
                    if (shotHitsTarget)
                    {
                        UnitHealth targetUnitHealth = targetUnit.GetComponent<UnitHealth>();
                        targetUnitHealth.Damage(1);
                    }
                    
                }
                break;
                case ShootingState.Ending:
                stateTimer += Time.deltaTime;
                if(stateTimer >= endTimer)
                {
                    stateTimer = 0;
                    shootingVirtualCam.gameObject.SetActive(false);
                    ActionComplete();
                    shootingState = ShootingState.Positioning;
                    unit.TryToSpendActionPointsToTakeAction(this);
                }
                
                break;
        }
    }
    public override void ActionSelectedVisual()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, shootableLayer))
        {
            Transform targetUnitTransform = hitInfo.transform;
            if (targetUnitTransform.TryGetComponent<Unit>(out Unit unit))
            {
                targetUnit = unit;
            }
            if (targetUnit != null && targetUnit.GetHitChance() > 0)
            {
                if(lastSelectedVisual != null && lastSelectedVisual != targetUnit.GetSelectedVisual())
                {
                    ResetLastSelectedVisual();
                }
                lastSelectedVisual = targetUnit.GetSelectedVisual();
                lastSelectedVisual.Show();
                lastShootButtonUI = UnitShootSystemUI.Instance.GetShootButtonUIForUnit(targetUnit);
                lastShootButtonUI.SetActive();
            }
        }else if(lastSelectedVisual != null)
        {
            ResetLastSelectedVisual();
        }
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
            HandleShootAction(targetUnitTransform, onActionStart, onActionComplete);
        }
    }

    public void AltPerformAction(Action onActionStart, Action onActionComplete, Transform enemyTransform)
    {
        Transform targetUnitTransform = enemyTransform;
        HandleShootAction(targetUnitTransform, onActionStart, onActionComplete);
    }

    private void HandleShootAction(Transform targetUnitTransform, Action onActionStart, Action onActionComplete)
    {
        if (targetUnitTransform.TryGetComponent<Unit>(out Unit unit))
        {
            targetUnit = unit;
        }
        if (targetUnit != null && targetUnit.GetHitChance() > 0)
        {
            ResetLastSelectedVisual();
            ActionStart(onActionStart, onActionComplete);
            shootingVirtualCam.gameObject.SetActive(true);
            shootingVirtualCam.Follow = shootingCameraAimPointTransform;
            shootingVirtualCam.LookAt = shootingCameraAimPointTransform;
        }
    }

    private void ResetLastSelectedVisual()
    {
        if (lastSelectedVisual != null)
        {
            lastSelectedVisual.Hide();
            lastSelectedVisual = null;
            if(lastShootButtonUI != null)
            {
                lastShootButtonUI.SetInactive();
                lastShootButtonUI = null;
            }
        }
    }

    private void CalculateHitChance(float detectionAmount, Unit enemyUnit)
    {
        float hitChance = (detectionAmount / enemyUnit.GetMaxDetectionAmount()) * 100;
        enemyUnit.SetHitChance(hitChance);
    }

    override protected void ActionStart(Action onActionStart, Action onActionComplete)
    {
        float randomHitPercentage = Random.Range(0, 100);
        if(randomHitPercentage <= targetUnit.GetHitChance())
        {
            shotHitsTarget = true;
        }
        else
        {
            shotHitsTarget = false;
        }
        base.ActionStart(onActionStart, onActionComplete);
    }
}
