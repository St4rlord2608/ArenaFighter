using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int currentHealth;

    public event EventHandler<OnHealthChangedEventArgs> onHealthChanged;
    
    public class OnHealthChangedEventArgs : EventArgs
    {
        public int currentHealth;
    }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Damage(int damageAmount)
    {
        currentHealth -= damageAmount;
        onHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            currentHealth = currentHealth,
        });

    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        onHealthChanged?.Invoke(this, new OnHealthChangedEventArgs
        {
            currentHealth = currentHealth,
        });
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
