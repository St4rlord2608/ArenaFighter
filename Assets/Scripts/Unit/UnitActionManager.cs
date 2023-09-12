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
    [SerializeField] private int enemyLayerIndex = 8;
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
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            SetSelectedAction(null);
            SetSelectedUnit(null);
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
                    TryToPerformAction();

                }

            }
            else
            {
                selectedAction.ActionSelectedVisual();
            }

        }
    }

    public void TryToPerformAction()
    {
        if (selectedUnit.CanSpendActionPoints(selectedAction))
        {
            selectedAction.PerformAction(SetBusy, ClearBusy);
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
                    return true;
                }
            }
        }
        return false;
    }

    public void ClearBusy()
    {
        isBusy = false;
        onBusyChanged?.Invoke(this, isBusy);
    }

    public void SetBusy()
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
        SetSelectedAction(null);
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

    public int GetEnemyLayerIndex()
    {
        return enemyLayerIndex;
    }
}
