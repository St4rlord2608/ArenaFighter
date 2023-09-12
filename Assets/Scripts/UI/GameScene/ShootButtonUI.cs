using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShootButtonUI : MonoBehaviour
{
    [SerializeField] private Button shootButton;
    [SerializeField] private TextMeshProUGUI detectionAmountText;

    private Transform enemyTransform;
    private Unit enemyUnit;

    public void SetEnemy(Transform enemyTransform)
    {
        this.enemyTransform = enemyTransform;
        enemyUnit = enemyTransform.GetComponent<Unit>();
        detectionAmountText.text = ((enemyUnit.GetDetectionAmount() / enemyUnit.GetMaxDetectionAmount()) * 100) + "%";
        shootButton.onClick.AddListener(() =>
        {
            Unit selectedUnit = UnitActionManager.Instance.GetSelectedUnit();
            ShootAction selectedAction = (ShootAction)UnitActionManager.Instance.GetSelectedAction();
            if (selectedUnit.CanSpendActionPoints(selectedAction))
            {
                selectedAction.AltPerformAction(UnitActionManager.Instance.SetBusy, UnitActionManager.Instance.ClearBusy, enemyTransform);
            }
        });
    }
}
