using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : MonoBehaviour
{
    public const string MOVE_ACTION = "Move";
    public const string SPIN_ACTION = "Spin";

    [SerializeField] private bool isEnemy = false;
    [Space]
    [SerializeField] private float maxMoveDistance = 10f;
    [SerializeField] private float maxSpinAmount = 2f;

    private MoveAction moveAction;
    private BaseAction[] baseActionArray;
    private DynamicGridObstacle gridObstacle;

    private float availableMoveDistance;
    private float availableSpinAmount;

    public static event EventHandler onAnyActionPointsChanged;
    private void Awake()
    {
        ResetActionPoints();
        moveAction = GetComponent<MoveAction>();
        baseActionArray = GetComponents<BaseAction>();
        gridObstacle = GetComponent<DynamicGridObstacle>();
    }

    private void Start()
    {
        UnitActionManager.Instance.onSelectedUnitChanged += UnitActionManager_onSelectedUnitChanged;
        TurnSystem.Instance.onTurnChanged += TurnSystem_onTurnChanged;
    }

    private void TurnSystem_onTurnChanged(object sender, EventArgs e)
    {
        if((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) ||
            (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            ResetActionPoints();
            onAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UnitActionManager_onSelectedUnitChanged(object sender, EventArgs e)
    {
        if(UnitActionManager.Instance.GetSelectedUnit() == this)
        {
            gameObject.layer = UnitActionManager.Instance.GetSelectedUnitLayerIndex();
            gridObstacle.DoUpdateGraphs();
            gridObstacle.enabled = false;
        }
        else
        {
            gameObject.layer = UnitActionManager.Instance.GetUnitLayerIndex();
            gridObstacle.enabled = true;
        }
    }

    private void ResetActionPoints()
    {
        availableMoveDistance = maxMoveDistance;
        availableSpinAmount = maxSpinAmount;
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public MoveAction GetMoveAction()
    {
        return moveAction;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return baseActionArray;
    }

    public bool TryToSpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if(CanSpendActionPoints(baseAction))
        {
            SpendActionPoints(baseAction);
            return true;
        }
        return false;
    }

    public bool CanSpendActionPoints(BaseAction baseAction)
    {
        switch (baseAction.GetActionName())
        {
            case MOVE_ACTION:
                if(availableMoveDistance >= baseAction.GetActionPointsCost())
                {
                    return true;
                }
                break; 
            case SPIN_ACTION:
                if(availableSpinAmount >= baseAction.GetActionPointsCost())
                {
                    return true;
                }
                break;
            default:
                return false;
        }
        return false;
    }

    private void SpendActionPoints(BaseAction baseAction)
    {
        switch(baseAction.GetActionName())
        {
            case MOVE_ACTION:
                availableMoveDistance -= baseAction.GetActionPointsCost();
                break;
            case SPIN_ACTION:
                availableSpinAmount -= baseAction.GetActionPointsCost();
                break;
        }
        onAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);    
    }

    public void SetAvailableActionPoints(BaseAction baseAction, float amount)
    {
        switch (baseAction.GetActionName())
        {
            case MOVE_ACTION:
                availableMoveDistance = amount;
                return;

            case SPIN_ACTION:
                availableSpinAmount = amount;
                return;
        }
        onAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public float GetAvailableActionPoints(BaseAction baseAction)
    {
        switch(baseAction.GetActionName())
        {
            case MOVE_ACTION:
                return availableMoveDistance;

                case SPIN_ACTION:
                return availableSpinAmount;
        }
        return 0;
    }

    
}
