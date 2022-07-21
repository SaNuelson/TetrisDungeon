using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerScript : MonoBehaviour
{
    private Animator animator;

    public float MaximumHealth;
    public float CurrentHealth;

    public float AttackPower;

    private List<float> lineFillTimes = new List<float>();
    public float ComboTimeFrame = 1f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateAnimation();
    }

    public void OnAttack()
    {
        lineFillTimes.Add(Time.time);
        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        while (lineFillTimes.Count > 0 && lineFillTimes[0] <= Time.time - ComboTimeFrame)
            lineFillTimes.RemoveAt(0);

        print("Combo = " + lineFillTimes.Count);

        animator.SetInteger("combo", lineFillTimes.Count);
        
    }
}
