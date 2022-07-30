using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PlayerScript : MonoBehaviour
{
    public Combatable Combatable;
    public Combatable Target;

    private Animator animator;

    private int comboCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnTriggerAttack()
    {
        comboCounter++;
        animator.SetInteger("combo", comboCounter);
        Combatable.InflictDamage(Target);
    }

    public void OnAttackEnd()
    {
        comboCounter--;
        animator.SetInteger("combo", comboCounter);
    }
}
