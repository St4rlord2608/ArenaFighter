using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private void Start()
    {
        UnitActionManager.Instance.onSelectedUnitChanged += UnitActionManager_onSelectedUnitChanged;
        Hide();
    }

    private void UnitActionManager_onSelectedUnitChanged(object sender, System.EventArgs e)
    {
        if(UnitActionManager.Instance.GetSelectedUnit() != unit)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UnitActionManager.Instance.onSelectedActionChanged -= UnitActionManager_onSelectedUnitChanged;
    }
}
