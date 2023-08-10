using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private void Start()
    {
        UnitActionManager.Instance.onSelectedUnitChanged += UnitActionManager_onSelectedUnitChanged;
        gameObject.SetActive(false);
    }

    private void UnitActionManager_onSelectedUnitChanged(object sender, System.EventArgs e)
    {
        if(UnitActionManager.Instance.GetSelectedUnit() != unit)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }
}
