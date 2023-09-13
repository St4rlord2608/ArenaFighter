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
    private ColorBlock defaultColorBlock;
    private ColorBlock selectedColorBlock;

    private bool moveToPos = false;

    private void Awake()
    {
        defaultColorBlock = shootButton.colors;
        selectedColorBlock = shootButton.colors;
        selectedColorBlock.normalColor = selectedColorBlock.highlightedColor;
    }

    private void Update()
    {
        if(moveToPos)
        {
            if (CameraController.Instance.MoveToPosition(enemyTransform.position))
            {
                moveToPos = false;
            }
        }
    }
    public void SetEnemy(Transform enemyTransform)
    {
        this.enemyTransform = enemyTransform;
        enemyUnit = enemyTransform.GetComponent<Unit>();
        detectionAmountText.text = enemyUnit.GetHitChance() + "%";
        shootButton.onClick.AddListener(() =>
        {
            HideSelectedVisual();
            Unit selectedUnit = UnitActionManager.Instance.GetSelectedUnit();
            ShootAction selectedAction = (ShootAction)UnitActionManager.Instance.GetSelectedAction();
            if (selectedUnit.CanSpendActionPoints(selectedAction))
            {
                selectedAction.AltPerformAction(UnitActionManager.Instance.SetBusy, UnitActionManager.Instance.ClearBusy, enemyTransform);
            }
        });
    }

    public void ShowSelectedVisual()
    {
        moveToPos = true;
        SelectedVisual selectedVisual = enemyUnit.GetSelectedVisual();
        selectedVisual.Show();
    }

    public void HideSelectedVisual()
    {
        moveToPos = false;
        SelectedVisual selectedVisual = enemyUnit.GetSelectedVisual();
        selectedVisual.Hide();
    }

    public Unit GetEnemyUnit()
    {
        return enemyUnit;
    }

    public void SetActive()
    {
        shootButton.colors = selectedColorBlock;
    }

    public void SetInactive()
    {
        shootButton.colors = defaultColorBlock;
    }
}
