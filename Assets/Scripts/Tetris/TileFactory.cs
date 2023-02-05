using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Util
{
    public enum TileType
    {
        Quad, // manages 4 cardinal directions
        Oct // manages direction T, TR, R, BR, B, BL, L, TL
    }

    [ExecuteAlways]
    public class TileFactory
    {
        internal class TileTemplate
        {
            public Sprite sprite;
            private int rotation;
            private bool flipped;
            private string name;

            public TileTemplate(string name, Sprite sprite, int rotation, bool flipped)
            {
                this.sprite = sprite;
                this.rotation = rotation;
                this.flipped = flipped;
                this.name = name;
            }

            public GameObject Get()
            {
                var tile = new GameObject(name);
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

        private static TileFactory defaultFactory = null;
        public static TileFactory GetDefault()
        {
            if (defaultFactory == null)
            {
                var defaultSprite = Sprite.Create(
                    Utils.CreateSolidTexture(100, 100, Color.red),
                    new Rect(0, 0, 100, 100),
                    new Vector2(0.5f, 0.5f));
                defaultSprite.name = "xxxx";
                var defaultSprites = new Sprite[] { defaultSprite };

                defaultFactory = new TileFactory(defaultSprites, TileType.Quad);
            }
            return defaultFactory;
        }


        private Sprite[] SpriteGroup;
        private TileType type;
        public TileType Type => type;

        private Dictionary<string, TileTemplate> tiles = new Dictionary<string, TileTemplate>();

        public TileFactory(Sprite[] sprites, TileType type)
        {
            SpriteGroup = sprites;
            this.type = type;
            AssembleDictionary();
        }

        private void AssembleDictionary()
        {
            if (SpriteGroup == null)
            {
                Debug.LogError("TileFactory has no sprites");
                return;
            }

            // for each tile, note rotated and flipped sprites.
            // if any are not provided explicitly, transform or another will try to fill the place
            Dictionary<string, TileTemplate> secondaryTiles = new Dictionary<string, TileTemplate>();

            foreach (var sprite in SpriteGroup)
            {
                if (type == TileType.Quad && sprite.name.Length != 4 ||
                    type == TileType.Oct && sprite.name.Length != 8)
                {
                    Debug.LogError("Invalid name format of sprite " + sprite.name + " for manager of Type " + type);
                    continue;
                }

                var combinations = replaceWildcards(sprite.name);
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
            var nameLen = type == TileType.Quad ? 4 : 8;
            for (int i = 0; i < Mathf.Pow(2, nameLen); i++)
            {
                var name = Convert.ToString(i, 2).PadLeft(nameLen, '0');
                if (!tiles.ContainsKey(name))
                    sb.AppendLine(name);
            }

            if (sb.Length > 0)
            {
                Debug.LogWarning("Missing tiles dump (" + type + "):\n" + sb.ToString());
            }

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
                tiles[name] = new TileTemplate(name, sprite, 0, false);

                var rot90Name = GetRotatedName(name, 1);
                secondaryTiles[rot90Name] = new TileTemplate(rot90Name, sprite, 1, false);
                
                var rot180Name = GetRotatedName(name, 2);
                secondaryTiles[rot180Name] = new TileTemplate(rot180Name, sprite, 2, false);

                var rot270Name = GetRotatedName(name, 3);
                secondaryTiles[rot270Name] = new TileTemplate(rot270Name, sprite, 3, false);

                var flipName = GetFlippedName(name);
                secondaryTiles[flipName] = new TileTemplate(flipName, sprite, 0, true);

                var flipRot90Name = GetFlippedName(GetRotatedName(name, 1));
                secondaryTiles[flipRot90Name] = new TileTemplate(flipRot90Name, sprite, 1, true);

                var flipRot180Name = GetFlippedName(GetRotatedName(name, 2));
                secondaryTiles[flipRot180Name] = new TileTemplate(flipRot180Name, sprite, 2, true);

                var flipRot270Name = GetFlippedName(GetRotatedName(name, 3));
                secondaryTiles[flipRot270Name] = new TileTemplate(flipRot270Name, sprite, 3, true);
            }
        }

        public GameObject MakeTile(bool top, bool right, bool bot, bool left)
        {
            if (type == TileType.Oct)
            {
                // Debug.LogWarning("MakeTitle -- quad params for oct factory");
                return MakeTile(top, false, right, false, bot, false, left, false); // TODO: maybe precompute default fallback Quad dict?
            }

            var tileName = GetTileName(top, right, bot, left);
            return tiles[tileName].Get();
        }

        public GameObject MakeTile(bool top, bool topRight, bool right, bool botRight, bool bot, bool botLeft, bool left, bool topLeft)
        {
            if (type == TileType.Quad)
            {
                // Debug.LogWarning("MakeTitle -- oct params for quad factory");
                return MakeTile(top, right, bot, left);
            }

            var tileName = GetTileName(top, topRight, right, botRight, bot, botLeft, left, topLeft);
            return tiles[tileName].Get();
        }

        public GameObject MakeTile(string name)
        {
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

            if (type == TileType.Quad)
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

            if (type == TileType.Quad)
                newName = name.Substring(4 - pihalfs) + name.Substring(0, 4 - pihalfs);
            else
                newName = name.Substring(8 - 2 * pihalfs) + name.Substring(0, 8 - 2 * pihalfs);

            return newName;
        }

        public bool IsValidSpriteName(string name)
        {
            if (type == TileType.Quad)
                return name.Length == 4;
            else if (type == TileType.Oct)
                return name.Length == 8;
            throw new NotImplementedException("Missing for manager Type " + type);
        }

    }

}
