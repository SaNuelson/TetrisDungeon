using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [Header("Floating Configuration")]
    public float ShowTime = 1f;
    private float InitialTime;
    public Vector3 Direction;
    public Color InitialColor;
    public Color ExittingColor;
    private AnimationCurve linearAnim = AnimationCurve.Linear(0, 0, 1, 1);

    private TextMeshPro Text;
    private bool isConstructed;

    public void Construct(string text, Vector3 direction, Color initialColor, Color finalColor)
    {
        if (isConstructed)
        {
            Debug.LogError("FloatingText already constructed");
            return;
        }

        if (Text == null)
        {
            Text = transform.Find("Text (TMP)").GetComponent<TextMeshPro>();
            Text.text = text;
        }

        InitialTime = Time.time;
        Direction = direction;
        InitialColor = initialColor;
        ExittingColor = finalColor;
        gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        transform.position += Direction * Time.deltaTime;

        var tweenFactor = linearAnim.Evaluate((Time.time - InitialTime) / ShowTime);

        if (tweenFactor >= 1)
            Destroy(gameObject);

        Text.color = Color.Lerp(InitialColor, ExittingColor, tweenFactor);
    }
}
