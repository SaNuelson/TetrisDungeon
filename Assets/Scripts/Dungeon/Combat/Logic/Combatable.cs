using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Combatable : MonoBehaviour
{
    public int MaxHealth;
    [SerializeField] private int currentHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            var oldHealth = currentHealth;
            currentHealth = value;
            HealthChanged.Invoke(oldHealth, currentHealth);
        }
    }

    public int MinAttackDamage;
    public int MaxAttackDamage;

    public ValueChangeUnityEvent<int> HealthChanged = new ValueChangeUnityEvent<int>();

    private void Start()
    {
        currentHealth = MaxHealth;
    }

    public float ReceiveDamage(int amount)
    {
        CurrentHealth -= amount;
        return amount;
    }

    public float InflictDamage(Combatable receiver)
    {
        var amount = Random.Range(MinAttackDamage, MaxAttackDamage);
        return receiver.ReceiveDamage(amount);
    }
}

[System.Serializable]
public class ValueChangeUnityEvent<T> : UnityEvent<T, T>
{
    public T OldValue;
    public T NewValue;
}