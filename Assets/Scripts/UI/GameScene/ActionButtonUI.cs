using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI actionText;
    [SerializeField] private Button button;
    [SerializeField] private Transform selectedTransform;

    private BaseAction action;

    public void SetBaseAction(BaseAction action)
    {
        this.action = action;
        actionText.text = action.GetActionName();

        button.onClick.AddListener(() =>
        {
            UnitActionManager.Instance.SetSelectedAction(this.action);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionManager.Instance.GetSelectedAction();
        selectedTransform.gameObject.SetActive(selectedBaseAction == action);
    }
}
