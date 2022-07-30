using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Combatable))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class CombatableRenderer : MonoBehaviour
{
    private Combatable Combatable;
    private SpriteRenderer SpriteRenderer;
    private Animator Animator;

    [Header("OnHit Effect")]
    public FloatingText FlyingText;
    public AnimationCurve HitAnimationCurve;
    public float HitShowTime = 1f;
    private bool hitShowing = false;
    private float hitStartTime;

    // Start is called before the first frame update
    void Start()
    {
        if (Combatable == null)
            Combatable = GetComponent<Combatable>();

        Combatable.HealthChanged.AddListener(OnHealthChanged);
    
        if (SpriteRenderer == null)
            SpriteRenderer = GetComponent<SpriteRenderer>();

        if (Animator == null)
            Animator = GetComponent<Animator>();

        FlyingText.gameObject.SetActive(false);
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

    public void OnHit(float amount)
    {
        hitShowing = true;
        hitStartTime = Time.time;
        Animator.SetTrigger("hit");
        ShowHit(amount);
    }

    private void ShowHit(float amount)
    {
        GameObject hitText = Instantiate(FlyingText.gameObject);
        hitText.transform.position = transform.position + new Vector3(Random.Range(-1f, 1f), 2.5f, 0);
        var hitTextScript = hitText.GetComponent<FloatingText>();
        hitTextScript.Construct("-" + amount, Vector3.up, Color.red, Color.clear);
    }

    public void OnDeath()
    {
        Animator.SetTrigger("dying");
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
