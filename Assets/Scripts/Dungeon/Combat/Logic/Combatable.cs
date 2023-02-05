using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Combatable : MonoBehaviour
{
    public int MaxHealth;
    [SerializeField] private int currentHealth;

    public AudioClip[] AttackSoundSet;
    public AudioClip[] HurtSoundSet;
    public AudioClip[] DeathSoundSet;

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

    private void Start()
    {
        currentHealth = MaxHealth;
        currentMana = MaxMana;

        HealthChanged.AddListener(OnHealthChanged);
        Killed.AddListener(OnKilled);
        Attacking.AddListener(OnAttacking);
    }

    public float ReceiveDamage(int amount)
    {
        CurrentHealth -= amount;
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
        CurrentMana += amount;
        return amount;
    }

    private void OnHealthChanged(int oldHp, int newHp)
    {
        if (oldHp > newHp && HurtSoundSet.Length > 0)
            SoundManager.instance.Play(HurtSoundSet[Random.Range(0, HurtSoundSet.Length)]);
    }

    private void OnKilled()
    {
        if (DeathSoundSet.Length > 0)
            SoundManager.instance.Play(DeathSoundSet[Random.Range(0, DeathSoundSet.Length)]);
    }

    private void OnAttacking()
    {
        if (AttackSoundSet.Length > 0)
            SoundManager.instance.Play(AttackSoundSet[Random.Range(0, AttackSoundSet.Length)]);
    }
}

[System.Serializable]
public class ValueChangeUnityEvent<T> : UnityEvent<T, T>
{
    public T OldValue;
    public T NewValue;
}