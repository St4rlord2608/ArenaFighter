using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button endTurnButton;

    private bool isBusy;

    private void Awake()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });
    }

    private void UnitActionManager_onBusyChanged(object sender, bool isBusy)
    {
        this.isBusy = isBusy;
        endTurnButton.gameObject.SetActive(!isBusy);
    }

    private void Start()
    {
        TurnSystem.Instance.onTurnChanged += TurnSystem_onTurnChanged;
        UnitActionManager.Instance.onBusyChanged += UnitActionManager_onBusyChanged;
    }

    private void TurnSystem_onTurnChanged(object sender, System.EventArgs e)
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
