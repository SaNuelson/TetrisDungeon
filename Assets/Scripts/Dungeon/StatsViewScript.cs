using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsViewScript : MonoBehaviour
{
    public int MaximumHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = value;
            OnHealthChanged();
        }
    }
    private int currentHealth;

    public int MaximumMana;
    public int CurrentMana
    {
        get => currentMana;
        set
        {
            currentMana = value;
            OnManaChanged();
        }
    }
    private int currentMana;

    private Slider healthBar;
    private Text healthLabel;

    private Slider manaBar;
    private Text manaLabel;

    private void Start()
    {
        var hpGroup = transform.Find("HPBar");
        healthBar = hpGroup.GetComponentInChildren<Slider>();
        healthLabel = hpGroup.GetComponentInChildren<Text>();

        var mpGroup = transform.Find("MPBar");
        manaBar = mpGroup.GetComponentInChildren<Slider>();
        manaLabel = mpGroup.GetComponentInChildren<Text>();
    }

    public void OnHealthChanged()
    {
        var percent = 100 * currentHealth / MaximumHealth;
        healthBar.value = percent;
        healthLabel.text = currentHealth + "/" + MaximumHealth;
    }

    public void OnManaChanged()
    {
        var percent = 100 * currentMana / MaximumMana;
        manaBar.value = percent;
        manaLabel.text = currentMana + "/" + MaximumMana;
    }
}
