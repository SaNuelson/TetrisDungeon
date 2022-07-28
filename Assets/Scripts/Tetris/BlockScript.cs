using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Tetris
{
    public class BlockScript : MonoBehaviour
    {
        private void Awake()
        {
            renderer = GetComponent<SpriteRenderer>();
            if (renderer == null)
                renderer = gameObject.AddComponent<SpriteRenderer>();
            renderer.sortingLayerName = "Tetris Mainground";
            color = renderer.color;
        }

        public Color Color
        {
            get => color;
            set
            {
                color = value;
                if (renderer == null)
                    renderer = gameObject.AddComponent<SpriteRenderer>();
                renderer.color = value;
            }
        }

        public Sprite Sprite
        {
            get => renderer.sprite;
            set
            {
                if (renderer == null)
                    renderer = gameObject.AddComponent<SpriteRenderer>();
                renderer.sprite = value;
            }
        }

        [SerializeField] private Color color;
        [SerializeField] private SpriteRenderer renderer;
    }
}