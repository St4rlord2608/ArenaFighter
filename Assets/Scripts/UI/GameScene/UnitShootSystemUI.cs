using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitShootSystemUI : MonoBehaviour
{
    public static UnitShootSystemUI Instance;

    [SerializeField] private Transform shootButtonContainer;
    [SerializeField] private Transform shootButtonUIPrefab;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UnitActionManager.Instance.onBusyChanged += UnitActionManager_onBusyChanged;
        UnitActionManager.Instance.onSelectedActionChanged += UnitActionManager_onSelectedActionChanged;
        Hide();
    }

    private void UnitActionManager_onBusyChanged(object sender, bool isBusy)
    {
        if (UnitActionManager.Instance.GetSelectedAction() && UnitActionManager.Instance.GetSelectedAction().GetActionName() == Unit.SHOOT_ACTION)
        {
            gameObject.SetActive(!isBusy);
        }
        else
        {
            Hide();
        }
       
    }

    private void UnitActionManager_onSelectedActionChanged(object sender, System.EventArgs e)
    {
        HandleVisibility();
    }

    public void CreateShootButtons(List<Transform> seeableEnemieList)
    {
        foreach(Transform shootButton in shootButtonContainer)
        {
            Destroy(shootButton.gameObject);
        }
        foreach(Transform enemyTransform in seeableEnemieList)
        {
            Transform actionButtonTranform = Instantiate(shootButtonUIPrefab, shootButtonContainer);
            ShootButtonUI shootButtonUI = actionButtonTranform.GetComponent<ShootButtonUI>();
            shootButtonUI.SetEnemy(enemyTransform);

        }
    }

    private void HandleVisibility()
    {
        if (UnitActionManager.Instance.GetSelectedAction() && UnitActionManager.Instance.GetSelectedAction().GetActionName() == Unit.SHOOT_ACTION)
        {
            Show();
        }
        else
        {
            Hide();
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
}
