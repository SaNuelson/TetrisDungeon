using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Combatable))]
public class AutoCombatable : MonoBehaviour
{
    public Combatable Combatable;
    public Combatable Target;

    public bool IsActive;
    public bool IsAlive => IsActive && Combatable.CurrentHealth >= 0;

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

    private PlayerScript player;

    private void Awake()
    {
        if (Combatable == null)
            Combatable = GetComponent<Combatable>();
    }

    private void Start()
    {
        attackClock = GetAttackClock();
        StartCoroutine(GetAttackClock());
    }

    private void TakeAction()
    {
        Combatable.InflictDamage(Target);
    }
}
