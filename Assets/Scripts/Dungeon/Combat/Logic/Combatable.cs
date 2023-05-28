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
            if (value == currentHealth)
                return;

            var oldHealth = currentHealth;
            currentHealth = value;
            HealthChanged.Invoke(oldHealth, currentHealth);

            if (value <= 0)
            {
                Killed.Invoke();
            }
        }
    }

    public int MaxMana;
    [SerializeField] private int currentMana;
    public int CurrentMana
    {
        get => currentMana;
        set
        {
            if (value == currentMana)
                return;

            var oldMana = currentMana;
            currentMana = value;
            ManaChanged.Invoke(oldMana, currentMana);
        }
    }

    public int MinAttackDamage;
    public int MaxAttackDamage;

    public ValueChangeUnityEvent<int> HealthChanged = new ValueChangeUnityEvent<int>();
    public UnityEvent Killed = new UnityEvent();
    public ValueChangeUnityEvent<int> ManaChanged = new ValueChangeUnityEvent<int>();
    public UnityEvent Attacking = new UnityEvent();

    private void Awake()
    {
        currentHealth = MaxHealth;
        currentMana = MaxMana;
    }

    public float ReceiveDamage(int amount)
    {
        var oldHealth = CurrentHealth;
        CurrentHealth -= amount;
        HealthChanged.Invoke(oldHealth, currentHealth);
        return amount;
    }

    public float InflictDamage(Combatable receiver)
    {
        var amount = Random.Range(MinAttackDamage, MaxAttackDamage);
        Attacking.Invoke();
        return receiver.ReceiveDamage(amount);
    }

    public float ChangeMana(int amount)
    {
        var oldMana = CurrentMana;
        CurrentMana += amount;
        ManaChanged.Invoke(oldMana, CurrentMana);
        return amount;
    }
}

[System.Serializable]
public class ValueChangeUnityEvent<T> : UnityEvent<T, T>
{
    public T OldValue;
    public T NewValue;
}