using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockScript : MonoBehaviour
{
    public Color ForegroundColor
    {
        get => _foregroundColor;
        set
        {
            _foregroundColor = value;
            _foregroundSprite.color = value;
        }
    }
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            _backgroundSprite.color = value;
        }
    }

    [SerializeField] private Color _foregroundColor;
    [SerializeField] private Color _backgroundColor;

    [SerializeField] private SpriteRenderer _foregroundSprite;
    [SerializeField] private SpriteRenderer _backgroundSprite;
}
