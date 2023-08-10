using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionManager : MonoBehaviour
{
    public static UnitActionManager Instance;

    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask movementLayer;
    [SerializeField] private float maxActionCooldown = 1f;

    private Unit selectedUnit;
    private BaseAction selectedAction;

    private float currentActionCooldown = 0f;

    private bool isBusy = false;

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
        if(currentActionCooldown <= maxActionCooldown)
        {
            currentActionCooldown += Time.deltaTime;
        }
        if (HandleUnitSelection())
        {
            return;
        }

        if (selectedUnit != null && selectedAction != null)
        {
            if (GameInput.Instance.InteractPressed())
            {
                if(!(currentActionCooldown <= maxActionCooldown))
                {
                    selectedAction.PerformAction(SetBusy, ClearBusy);
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
                if (hitInfo.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    selectedUnit = unit;
                    if(unit.transform.TryGetComponent<MoveAction>(out MoveAction moveAction))
                    {
                        selectedAction = moveAction;
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
    }

    private void SetBusy()
    {
        isBusy = true;
    }
}
