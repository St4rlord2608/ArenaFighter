using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionManager : MonoBehaviour
{
    public static UnitActionManager Instance;

    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask movementLayer;
    [SerializeField] private float maxActionCooldown = 1f;
    [Space]
    [SerializeField] private int unitLayerIndex = 3;
    [SerializeField] private int selectedUnitLayerIndex = 7;

    private Unit selectedUnit;
    private BaseAction selectedAction;

    private float currentActionCooldown = 0f;

    private bool isBusy = false;

    public event EventHandler onSelectedUnitChanged;
    public event EventHandler onSelectedActionChanged;
    public event EventHandler<bool> onBusyChanged;

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if(isBusy)
        {
            currentActionCooldown = 0f;
            return;
        }

        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if(currentActionCooldown <= maxActionCooldown)
        {
            currentActionCooldown += Time.deltaTime;
        }
        if (HandleUnitSelection())
        {
            currentActionCooldown = 0f;
            return;
        }
        HandleSelectedAction();
        
    }

    private void HandleSelectedAction()
    {
        if (selectedUnit != null && selectedAction != null)
        {
            if (GameInput.Instance.InteractPressed())
            {
                if (!(currentActionCooldown <= maxActionCooldown))
                {
                    if (selectedAction.GetActionName() == Unit.MOVE_ACTION)
                    {
                        if (selectedUnit.CanSpendActionPoints(selectedAction))
                        {
                            selectedAction.PerformAction(SetBusy, ClearBusy);
                        }
                    }
                    else
                    {
                        if (selectedUnit.TryToSpendActionPointsToTakeAction(selectedAction))
                        {
                            selectedAction.PerformAction(SetBusy, ClearBusy);
                        }
                    }

                }

            }
            else
            {
                selectedAction.ActionSelectedVisual();
            }

        }
    }

    private bool HandleUnitSelection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, unitLayer))
        {
            if (GameInput.Instance.InteractPressed())
            {
                if (hitInfo.transform.TryGetComponent<Unit>(out Unit unit) && unit != selectedUnit)
                {
                    if (unit.IsEnemy())
                    {
                        return false;
                    }
                    SetSelectedUnit(unit);
                    if(unit.transform.TryGetComponent<MoveAction>(out MoveAction moveAction))
                    {
                        SetSelectedAction(moveAction);
                    }
                    return true;
                }
            }
        }
        return false;
    }

    private void ClearBusy()
    {
        isBusy = false;
        onBusyChanged?.Invoke(this, isBusy);
    }

    private void SetBusy()
    {
        isBusy = true;
        onBusyChanged?.Invoke(this, isBusy);
    }

    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;
        onSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    public BaseAction GetSelectedAction()
    {
        return selectedAction;
    }

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        onSelectedUnitChanged?.Invoke(this, EventArgs.Empty);

    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }

    public int GetSelectedUnitLayerIndex()
    {
        return selectedUnitLayerIndex;
    }

    public int GetUnitLayerIndex()
    {
        return unitLayerIndex;
    }
}
