using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsView : MonoBehaviour
{
    public Combatable Combatable;

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


        if (Combatable == null)
            Combatable = transform.parent.GetComponentInChildren<Combatable>();

        Combatable.HealthChanged.AddListener(OnHealthChanged);
        Combatable.ManaChanged.AddListener(OnManaChanged);

        OnHealthChanged(0, 0);
        OnManaChanged(0, 0);
    }

    public void OnHealthChanged(int oldHealth, int newHealth)
    {
        var percent = 100 * Combatable.CurrentHealth / Combatable.MaxHealth;
        healthBar.value = percent;
        healthLabel.text = Combatable.CurrentHealth + "/" + Combatable.MaxHealth;
    }

    public void OnManaChanged(int oldMana, int newMana)
    {
        var percent = 100 * Combatable.CurrentMana / Combatable.MaxMana;
        manaBar.value = percent;
        manaLabel.text = Combatable.CurrentMana + "/" + Combatable.MaxMana;
    }
}
