using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tetris
{
    public class BlockScript : MonoBehaviour
    {
        private void Awake()
        {

            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Tetris Mainground";
            color = spriteRenderer.color;
        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                if (spriteRenderer == null)
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.color = value;
            }
        }

        public Sprite Sprite
        {
            get => spriteRenderer.sprite;
            set
            {
                if (spriteRenderer == null)
                    spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = value;
            }
        }

        [SerializeField] private Color color;
        [SerializeField] private SpriteRenderer spriteRenderer;
    }
}