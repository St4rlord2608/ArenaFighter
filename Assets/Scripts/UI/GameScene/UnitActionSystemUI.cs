using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] private TextMeshProUGUI actionPointsText;

    private List<ActionButtonUI> actionButtonUIList;

    private bool isBusy;

    private void Awake()
    {
        actionPointsText.gameObject.SetActive(false);
        actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionManager.Instance.onSelectedUnitChanged += UnitActionManager_onSelectedUnitChanged;
        UnitActionManager.Instance.onSelectedActionChanged += UnitActionManager_onSelectedActionChanged;
        UnitActionManager.Instance.onBusyChanged += UnitActionManager_onBusyChanged;
        Unit.onAnyActionPointsChanged += Unit_onAnyActionPointsChanged;

        CreateUnitActionButtons();
    }

    private void Unit_onAnyActionPointsChanged(object sender, System.EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UnitActionManager_onSelectedActionChanged(object sender, System.EventArgs e)
    {
        UpdateSelectedVisual();
        UpdateActionPoints();
    }

    private void UnitActionManager_onBusyChanged(object sender, bool isBusy)
    {
        this.isBusy = isBusy;
        actionPointsText.gameObject.SetActive(!isBusy);
        actionButtonContainerTransform.gameObject.SetActive(!isBusy);
        UpdateActionPoints();
    }

    private void UnitActionManager_onSelectedUnitChanged(object sender, System.EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
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

    private void UpdateActionPoints()
    {
        Unit selectedUnit = UnitActionManager.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionManager.Instance.GetSelectedAction();
        if(selectedUnit == null ||selectedAction == null)
        {
            actionPointsText.gameObject.SetActive(false);
            return;
        }else if(!isBusy)
        {
            actionPointsText.gameObject.SetActive(true);
        }
        
        if(selectedUnit.GetAvailableActionPoints(selectedAction) == 0)
        {
            actionPointsText.text = "0";
        }
        else
        {
            actionPointsText.text = (Mathf.Round(selectedUnit.GetAvailableActionPoints(selectedAction) * 100) / 100).ToString();
        }
        
    }
}
