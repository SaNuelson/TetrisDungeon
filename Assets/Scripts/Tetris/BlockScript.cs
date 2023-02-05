using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tetris
{
    public class BlockScript : MonoBehaviour
    {

        [SerializeField] private Color color;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public UnityEvent BeforeDestroyed = new UnityEvent();

        private void Init()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingLayerName = "Tetris Mainground";
            color = spriteRenderer.color;
        }

        private void Awake()
        {
            Init();
        }

        private void OnDestroy()
        {
            BeforeDestroyed.Invoke();
        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                if (spriteRenderer == null)
                    Init();
                spriteRenderer.color = value;
            }
        }

        public Sprite Sprite
        {
            get => spriteRenderer.sprite;
            set
            {
                if (spriteRenderer == null)
                    Init();
                spriteRenderer.sprite = value;
            }
        }
    }
}