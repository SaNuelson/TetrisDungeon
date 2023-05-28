using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CombatableRenderer : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer;
    private Animator Animator;
    private Combatable Combatable;

    [Header("SFX")]
    public AudioClip[] AttackSoundSet;
    public AudioClip[] HurtSoundSet;
    public AudioClip[] DeathSoundSet;

    [Header("OnHit Effect")]
    public AnimationCurve HitAnimationCurve;
    public float HitShowTime = 1f;
    private bool hitShowing = false;
    private float hitStartTime;

    // Start is called before the first frame update
    void Start()
    {
        if (Combatable == null)
            Combatable = transform.parent.GetComponentInChildren<Combatable>();

        Combatable.HealthChanged.AddListener(OnHealthChanged);
        Combatable.Attacking.AddListener(OnAttack);
        Combatable.Killed.AddListener(OnDeath);
    
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();

        if (Animator == null)
            Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (hitShowing)
            UpdateHitShow();
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (newHealth <= 0)
        {
            OnDeath();
        }
        else if (oldHealth > newHealth)
        {
            OnHit(oldHealth - newHealth);
        }
    }

    public void OnAttack()
    {
        Animator.SetTrigger("attack");

        if (AttackSoundSet != null && AttackSoundSet.Length > 0)
            SoundManager.instance.Play(AttackSoundSet[Random.Range(0, AttackSoundSet.Length)]);
    }

    public void OnHit(float amount)
    {
        Animator.SetTrigger("hit");
        ShowHit(amount);

        if (HurtSoundSet != null && HurtSoundSet.Length > 0)
            SoundManager.instance.Play(HurtSoundSet[Random.Range(0, HurtSoundSet.Length)]);
    }

    private void ShowHit(float amount)
    {
        hitShowing = true;
        hitStartTime = Time.time;

        GameObject hitText = FloatingText.Construct("-" + amount, Vector3.up, Color.red, Color.clear);
        hitText.transform.position = transform.position + new Vector3(Random.Range(-1f, 1f), 2.5f, 0);
    }

    public void OnDeath()
    {
        Animator.SetTrigger("dying");

        if (DeathSoundSet != null && DeathSoundSet.Length > 0)
            SoundManager.instance.Play(DeathSoundSet[Random.Range(0, DeathSoundSet.Length)]);
    }

    void UpdateHitShow()
    {
        var currentTime = Time.time - hitStartTime;

        if (currentTime > HitShowTime)
        {
            SpriteRenderer.color = Color.white;
            hitShowing = false;
            return;
        }

        var tweenFactor = HitAnimationCurve.Evaluate(currentTime / HitShowTime);
        SpriteRenderer.color = Color.Lerp(Color.white, Color.red, tweenFactor);
    }

}
