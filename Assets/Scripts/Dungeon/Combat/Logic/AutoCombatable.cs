using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Combatable))]
[RequireComponent(typeof(CombatableRenderer))]
public class AutoCombatable : MonoBehaviour
{
    public Combatable Self;

    public Combatable Target;


    public bool IsActive;
    public bool IsAlive => IsActive && Self.CurrentHealth >= 0;

    public float AttackSpeed;
    private IEnumerator attackClock;
    private IEnumerator GetAttackClock()
    {
        while(IsAlive)
        {
            yield return new WaitForSeconds(AttackSpeed);
            TakeAction();
        }
    }


    private void Awake()
    {
        if (Self == null)
            Self = GetComponent<Combatable>();
    }

    private void Start()
    {
        attackClock = GetAttackClock();
        StartCoroutine(GetAttackClock());
    }

    private void TakeAction()
    {
        Self.InflictDamage(Target);
    }
}
