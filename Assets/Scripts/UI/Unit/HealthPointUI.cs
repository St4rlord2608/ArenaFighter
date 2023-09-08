using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPointUI : MonoBehaviour
{
    [SerializeField] private Transform HealthPointForeground;

    public void ActivateHealthPoint()
    {
        HealthPointForeground.gameObject.SetActive(true);
    }

    public void DeactivateHealthPoint()
    {
        HealthPointForeground.gameObject.SetActive(false);
    }
}
