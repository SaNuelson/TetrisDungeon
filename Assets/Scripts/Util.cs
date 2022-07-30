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

    public enum TileManagerType
    {
        Quad, // manages directions T, R, B, L
        Oct // manages direction T, TR, R, BR, B, BL, L, TL
    }

    public class TileManager
    {
        internal class LazyTile
        {
            public Sprite sprite;
            private int rotation;
            private bool flipped;

            public LazyTile(Sprite sprite, int rotation, bool flipped)
            {
                this.sprite = sprite;
                this.rotation = rotation;
                this.flipped = flipped;
            }

            public GameObject Get()
            {
                var tile = new GameObject("Tile");
                var tileRenderer = tile.AddComponent<SpriteRenderer>();
                tileRenderer.sprite = sprite;

                if (rotation > 0)
                {
                    tile.transform.eulerAngles = new Vector3(0, 0, -90 * rotation);
                }

                if (flipped)
                {
                    if (rotation % 2 == 0)
                        tileRenderer.flipX = true;
                    else
                        tileRenderer.flipY = true;
                }

                return tile;
            }
        }

        private Sprite[] spriteGroup;
        private Dictionary<string, LazyTile> tiles = new Dictionary<string, LazyTile>();

        private TileManagerType type;
        public TileManagerType Type => type;

        public TileManager(Sprite[] sprites, TileManagerType type)
        {
            Debug.Log("New TileManager");
            spriteGroup = sprites;
            this.type = type;

            AssembleDictionary();
        }

        private void AssembleDictionary()
        {
            // for each tile, note rotated and flipped sprites.
            // if any are not provided explicitly, transform or another will try to fill the place
            Dictionary<string, LazyTile> secondaryTiles = new Dictionary<string, LazyTile>();

            foreach (var sprite in spriteGroup)
            {
                if (type == TileManagerType.Quad && sprite.name.Length != 4 ||
                    type == TileManagerType.Oct && sprite.name.Length != 8)
                {
                    Debug.LogError("Invalid name format of sprite " + sprite.name + " for manager of type " + type);
                    continue;
                }

                var combinations = replaceWildcards(sprite.name);
                Debug.Log("replaceWildcards " + sprite.name + " -> " + String.Join(", ", combinations));
                foreach (var combination in combinations)
                {
                    SaveAllCombinations(combination, sprite);
                }
            }

            foreach (var name in secondaryTiles.Keys)
            {
                if (!tiles.ContainsKey(name))
                    tiles[name] = secondaryTiles[name];
            }

            // TODO: Remove
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Mathf.Pow(2, 8); i++)
            {
                var name = Convert.ToString(i, 2).PadLeft(8, '0');
                if (!tiles.ContainsKey(name))
                    sb.AppendLine(name);
            }
            Debug.Log("Missing tiles dump:\n" + sb.ToString());

            List<string> replaceWildcards(string source)
            {
                if (source.Contains('x'))
                {
                    var idx = source.IndexOf('x');
                    var rep0 = source.Remove(idx, 1).Insert(idx, "0");
                    var rep1 = source.Remove(idx, 1).Insert(idx, "1");
                    return replaceWildcards(rep0).Concat(replaceWildcards(rep1)).ToList();
                }
                else
                {
                    return new List<string> { source };
                }
            }

            void SaveAllCombinations(string name, Sprite sprite)
            {
                tiles[name] = new LazyTile(sprite, 0, false);

                var rot90Name = GetRotatedName(name, 1);
                secondaryTiles[rot90Name] = new LazyTile(sprite, 1, false);
                
                var rot180Name = GetRotatedName(name, 2);
                secondaryTiles[rot180Name] = new LazyTile(sprite, 2, false);

                var rot270Name = GetRotatedName(name, 3);
                secondaryTiles[rot270Name] = new LazyTile(sprite, 3, false);

                var flipName = GetFlippedName(name);
                secondaryTiles[flipName] = new LazyTile(sprite, 0, true);

                var flipRot90Name = GetFlippedName(GetRotatedName(name, 1));
                secondaryTiles[flipRot90Name] = new LazyTile(sprite, 1, true);

                var flipRot180Name = GetFlippedName(GetRotatedName(name, 2));
                secondaryTiles[flipRot180Name] = new LazyTile(sprite, 2, true);

                var flipRot270Name = GetFlippedName(GetRotatedName(name, 3));
                secondaryTiles[flipRot270Name] = new LazyTile(sprite, 3, true);

                Debug.Log("AllCombinations of " + name + "\n" +
                    "    R90  " + rot90Name + "\n" +
                    "    R180 " + rot180Name + "\n" +
                    "    R270 " + rot270Name + "\n" +
                    "   FR0   " + flipName + "\n" +
                    "   FR90  " + flipRot90Name + "\n" +
                    "   FR180 " + flipRot180Name + "\n" +
                    "   FR270 " + flipRot270Name);  
            }
        }

        public GameObject GetTile(bool top, bool right, bool bot, bool left)
        {
            if (type == TileManagerType.Oct)
                return GetTile(top, false, right, false, bot, false, left, false); // TODO: maybe precompute default fallback Quad dict?

            var tileName = GetTileName(top, left, right, bot);
            Debug.Log("GetTile -- " + tileName);
            return tiles[tileName].Get();
        }

        public GameObject GetTile(bool top, bool topRight, bool right, bool botRight, bool bot, bool botLeft, bool left, bool topLeft)
        {
            if (type == TileManagerType.Quad)
                return GetTile(top, right, bot, left);

            var tileName = GetTileName(top, topRight, right, botRight, bot, botLeft, left, topLeft);
            Debug.Log("GetTile -- " + tileName);
            return tiles[tileName].Get();
        }

        public GameObject GetTile(string name)
        {
            Debug.Log("-- GetTile " + name);
            if (!IsValidSpriteName(name))
            {
                Debug.LogError("Invalid sprite name " + name);
                return null;
            }
            if (!tiles.ContainsKey(name))
            {
                Debug.LogError("Tiles do not contain name " + name);
                Debug.Log("Tile name dump: " + string.Join(", ", tiles.Keys.ToArray()));
                return null;
            }
            return tiles[name].Get();
        }

        public string GetTileName(bool top, bool right, bool bot, bool left)
        {
            return (top ? "1" : "0") 
                + (right ? "1" : "0") 
                + (bot ? "1" : "0") 
                + (left ? "1" : "0");
        }

        public string GetTileName(bool top, bool topRight, bool right, bool botRight, bool bot, bool botLeft, bool left, bool topLeft)
        {
            return (top ? "1" : "0") 
                + (topRight ? "1" : "0") 
                + (right ? "1" : "0")
                + (botRight ? "1" : "0")
                + (bot ? "1" : "0")
                + (botLeft ? "1" : "0")
                + (left ? "1" : "0")
                + (topLeft ? "1" : "0");
        }

        private string GetFlippedName(string name)
        {
            if (!IsValidSpriteName(name))
                return null;

            string newName;

            if (type == TileManagerType.Quad)
                newName = new string(new char[] { name[0], name[3], name[2], name[1] });
            else
                newName = new string(new char[] { name[0], name[7], name[6], name[5], name[4], name[3], name[2], name[1] });

            return newName;
        }

        private string GetRotatedName(string name, int pihalfs)
        {
            if (!IsValidSpriteName(name))
                return null;

            string newName;

            if (type == TileManagerType.Quad)
                newName = name.Substring(4 - pihalfs) + name.Substring(0, 4 - pihalfs);
            else
                newName = name.Substring(8 - 2 * pihalfs) + name.Substring(0, 8 - 2 * pihalfs);

            return newName;
        }

        public bool IsValidSpriteName(string name)
        {
            if (type == TileManagerType.Quad)
                return name.Length == 4;
            else if (type == TileManagerType.Oct)
                return name.Length == 8;
            throw new NotImplementedException("Missing for manager type " + type);
        }

    }

}
