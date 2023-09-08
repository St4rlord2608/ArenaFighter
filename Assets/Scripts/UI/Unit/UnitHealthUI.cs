using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealthUI : MonoBehaviour
{
    [SerializeField] private Transform unitTransform;
    [Space]
    [SerializeField] private Transform healthPointPrefab;
    [SerializeField] private Transform healthPointContainer;

    private List<HealthPointUI> healthPointUIList;
    private UnitHealth unitHealth;
    private int maxHealth;

    private void Awake()
    {
        unitHealth = unitTransform.GetComponent<UnitHealth>();
        maxHealth = unitHealth.GetMaxHealth();
        CreateHealthPoints();
        unitHealth.onHealthChanged += UnitHealth_onHealthChanged;
    }

    private void UnitHealth_onHealthChanged(object sender, UnitHealth.OnHealthChangedEventArgs e)
    {
        for (int healthPointUIListIndex = healthPointUIList.Count; healthPointUIListIndex > e.currentHealth; healthPointUIListIndex--)
        {
            healthPointUIList[healthPointUIListIndex - 1].DeactivateHealthPoint();
        }
    }

    private void CreateHealthPoints()
    {
        foreach (Transform healthPointTransform in healthPointContainer)
        {
            Destroy(healthPointTransform.gameObject);
        }
        healthPointUIList = new List<HealthPointUI>();

        for (int healthCount = 0; healthCount < maxHealth; healthCount++)
        {
            Transform healthPointTransform = Instantiate(healthPointPrefab, healthPointContainer);
            HealthPointUI healthPointUI = healthPointTransform.GetComponent<HealthPointUI>();
            healthPointUI.ActivateHealthPoint();
            healthPointUIList.Add(healthPointUI);
        }
    }
}
