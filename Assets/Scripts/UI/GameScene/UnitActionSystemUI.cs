using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;

    private List<ActionButtonUI> actionButtonUIList;

    private void Awake()
    {
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionManager.Instance.onSelectedUnitChanged += UnitActionManager_onSelectedUnitChanged;
        UnitActionManager.Instance.onSelectedActionChanged += UnitActionManager_onSelectedActionChanged;
        UnitActionManager.Instance.onBusyChanged += UnitActionManager_onBusyChanged;

        CreateUnitActionButtons();
    }

    private void UnitActionManager_onSelectedActionChanged(object sender, System.EventArgs e)
    {
        UpdateSelectedVisual();
    }

    private void UnitActionManager_onBusyChanged(object sender, bool isBusy)
    {
        if(isBusy)
        {
            actionButtonContainerTransform.gameObject.SetActive(false);
        }
        else
        {
            actionButtonContainerTransform.gameObject.SetActive(true);
        }
    }

    private void UnitActionManager_onSelectedUnitChanged(object sender, System.EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }

    private void CreateUnitActionButtons()
    {
        foreach(Transform buttonTransform in actionButtonContainerTransform)
        {
            Destroy(buttonTransform.gameObject);
        }

        actionButtonUIList.Clear();
        Unit selectedUnit = UnitActionManager.Instance.GetSelectedUnit();

        if(selectedUnit == null)
        {
            return;
        }

        foreach(BaseAction action in selectedUnit.GetBaseActionArray())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(action);

            actionButtonUIList.Add(actionButtonUI);
        }
    }

    private void UpdateSelectedVisual()
    {
        foreach(ActionButtonUI actionButtonUI in actionButtonUIList)
        {
            actionButtonUI.UpdateSelectedVisual();
        }
    }
}
