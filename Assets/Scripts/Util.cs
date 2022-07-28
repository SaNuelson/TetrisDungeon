using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public static class Utils
    {

        public static Texture2D CreateSolidTexture(int width, int height, Color color)
        {
            var texture = new Texture2D(width, height);
            var texPixs = texture.GetPixels();
            for (var i = 0; i < texPixs.Length; ++i)
            {
                texPixs[i] = color;
            }

            texture.SetPixels(texPixs);
            texture.Apply();
            return texture;
        }

    }

    public enum StrategyType
    {
        Random,
        Bag,
        MultiBag,
        History
    }

    public class RandomPicker<T>
    {
        private Strategy picker;

        public RandomPicker(T[] items, StrategyType strategy, int strategyParam)
        {
            switch (strategy)
            {
                case StrategyType.Random:
                    picker = new PseudoRandomStrategy(items);
                    break;
                case StrategyType.Bag:
                    picker = new BagStrategy(items);
                    break;
                case StrategyType.MultiBag:
                    picker = new MultiBagStrategy(items, strategyParam);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public T Next()
        {
            return picker.Get();
        }

        internal abstract class Strategy
        {
            protected T[] items;
            public abstract T Get();
        }

        internal class PseudoRandomStrategy : Strategy
        {
            public PseudoRandomStrategy(T[] items)
            {
                this.items = items;
            }

            public override T Get()
            {
                return items[UnityEngine.Random.Range(0, items.Length)];
            }
        }

        internal class BagStrategy : Strategy
        {
            private bool[] used;
            private int usedCounter = 0;
            public BagStrategy(T[] items)
            {
                this.items = items;
                used = new bool[items.Length];
            }

            public void Reset()
            {
                for (int i = 0; i < used.Length; i++)
                    used[i] = false;
                usedCounter = 0;
            }

            public bool IsEmpty => usedCounter == used.Length;

            public override T Get()
            {
                if (usedCounter == used.Length)
                    Reset();

                var rnd = UnityEngine.Random.Range(0, used.Length - usedCounter);
                for (int i = 0; i < used.Length; i++)
                {
                    if (!used[i])
                    {
                        if (rnd > 0)
                        {
                            rnd--;
                        }
                        else
                        {
                            used[i] = true;
                            usedCounter++;
                            return items[i];
                        }
                    }
                }
                throw new Exception("Internal error in bag strategy");
            }
        }

        internal class MultiBagStrategy : Strategy
        {
            private BagStrategy[] bags;
            private bool[] used;
            private int usedCounter = 0;

            public MultiBagStrategy(T[] items, int numOfBags)
            {
                this.items = items;
                bags = new BagStrategy[numOfBags];
                for (int i = 0; i < bags.Length; i++)
                    bags[i] = new BagStrategy(items);
            }

            public void Reset()
            {
                for (int i = 0; i < used.Length; i++)
                {
                    bags[i].Reset();
                    used[i] = false;
                }
                usedCounter = 0;
            }

            public override T Get()
            {
                if (usedCounter == used.Length)
                    Reset();

                var rnd = UnityEngine.Random.Range(0, used.Length - usedCounter);
                for (int i = 0; i < used.Length; i++)
                {
                    if (!used[i])
                    {
                        if (rnd > 0)
                        {
                            rnd--;
                        }
                        else
                        {
                            var item = bags[i].Get();
                            if (bags[i].IsEmpty)
                            {
                                used[i] = true;
                                usedCounter++;
                            }
                            return item;
                        }
                    }
                }
                throw new Exception("Internal error in bag strategy");
            }
        }
    }

    public enum SpriteManagerType
    {
        Quad, // manages directions T, R, B, L
        Oct // manages direction T, TR, R, BR, B, BL, L, TL
    }

    public class TileSpriteManager
    {
        private Sprite[] spriteGroup;
        private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

        private SpriteManagerType type;
        public SpriteManagerType Type => type;

        public TileSpriteManager(Sprite[] sprites, SpriteManagerType type)
        {
            spriteGroup = sprites;
            this.type = type;

            AssembleDictionary();
        }

        private void AssembleDictionary()
        {
            foreach (var sprite in spriteGroup)
            {
                if (type == SpriteManagerType.Quad && sprite.name.Length != 4 ||
                    type == SpriteManagerType.Oct && sprite.name.Length != 8)
                {
                    Debug.LogError("Invalid name format of sprite " + sprite.name + " for manager of type " + type);
                    continue;
                }

                sprites[sprite.name] = sprite;
            }
        }

        public Sprite GetSprite(bool top, bool left, bool right, bool bot)
        {
            return sprites[GetSpriteName(top, left, right, bot)];
        }

        public Sprite GetSprite(string name)
        {
            return sprites[name];
        }

        public string GetSpriteName(bool top, bool left, bool right, bool bot)
        {
            return (top ? "1" : "0") + (right ? "1" : "0") + (bot ? "1" : "0") + (left ? "1" : "0");
        }

    }

}
