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

    public static GameObject Construct(string text, Vector3 direction, Color initColor, Color finalColor)
    {
        var go = new GameObject("Floating combat text");
        go.transform.localScale = Vector3.one * 0.25f;

        var floatingText = go.AddComponent<FloatingText>();

        floatingText.InitialTime = Time.time;
        floatingText.Direction = direction;
        floatingText.InitialColor = initColor;
        floatingText.ExittingColor = finalColor;

        floatingText.Text = go.AddComponent<TextMeshPro>();
        floatingText.Text.text = text;
        floatingText.Text.alignment = TextAlignmentOptions.Center;

        return go;
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
