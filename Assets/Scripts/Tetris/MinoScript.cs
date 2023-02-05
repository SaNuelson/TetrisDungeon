using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace Assets.Scripts.Tetris
{
    [System.Serializable]
    public class MinoBlockDto
    {
        public BlockScript Block;
        public Vector2Int Offset;
    }

    public class MinoScript : MonoBehaviour
    {

        public MinoBlockDto[] Blocks;
        public Vector2Int BasePosition;

        public MinoShapePreset Preset;

        private int blocksLeftCounter;
        public UnityEvent BeforeDestroyed = new UnityEvent();

        private bool _isConstructed = false;

        public Vector3 Anchor => new Vector3(Preset.Anchor.x, Preset.Anchor.y, 0);

        public void ConstructFromPreset(MinoShapePreset preset, TileFactory factory, bool showAnchor = false)
        {
            if (_isConstructed)
            {
                Debug.LogError("TetrominoScript.Construct -- already constructed");
                return;
            }

            Blocks = new MinoBlockDto[preset.Offsets.Length];
            gameObject.name = preset.Name;
            Preset = preset;

            var fillMap = new bool[preset.BoxSize.x, preset.BoxSize.y];
            foreach (var offset in preset.Offsets)
                fillMap[offset.x, offset.y] = true;

            for (int i = 0; i < preset.Offsets.Length; i++)
            {
                var blockOffset = preset.Offsets[i];

                bool topRow = blockOffset.y >= preset.BoxSize.y - 1;
                bool botRow = blockOffset.y <= 0;
                bool leftCol = blockOffset.x <= 0;
                bool rightCol = blockOffset.x >= preset.BoxSize.x - 1;

                bool top = !topRow && fillMap[blockOffset.x, blockOffset.y + 1];
                bool topRight = !topRow && !rightCol && fillMap[blockOffset.x + 1, blockOffset.y + 1];
                bool right = !rightCol && fillMap[blockOffset.x + 1, blockOffset.y];
                bool botRight = !botRow && !rightCol && fillMap[blockOffset.x + 1, blockOffset.y - 1];
                bool bot = !botRow && fillMap[blockOffset.x, blockOffset.y - 1];
                bool botLeft = !botRow && !leftCol && fillMap[blockOffset.x - 1, blockOffset.y - 1];
                bool left = !leftCol && fillMap[blockOffset.x - 1, blockOffset.y];
                bool topLeft = !topRow && !leftCol && fillMap[blockOffset.x - 1, blockOffset.y + 1];

                var newBlock = factory.MakeTile(top, topRight, right, botRight, bot, botLeft, left, topLeft);
                var newBlockScript = newBlock.AddComponent<BlockScript>();
                newBlock.transform.SetParent(transform, false);
                newBlock.transform.localPosition += new Vector3(blockOffset.x, blockOffset.y, 0);
                newBlockScript.Color = preset.Color;
                newBlockScript.BeforeDestroyed.AddListener(OnBlockDestroyed);

                Blocks[i] = new MinoBlockDto()
                {
                    Block = newBlockScript,
                    Offset = blockOffset
                };
            }

            if (showAnchor)
            {
                var anchorBlock = factory.MakeTile(false, false, false, false, false, false, false, false);
                var anchorBlockScript = anchorBlock.AddComponent<BlockScript>();
                anchorBlockScript.Color = Color.red;
                anchorBlock.transform.SetParent(transform, false);
                anchorBlock.transform.localScale = 0.5f * Vector3.one;
                anchorBlock.transform.localPosition = new Vector3(preset.Anchor.x, preset.Anchor.y, 0);
            }

            blocksLeftCounter = Blocks.Length;
            _isConstructed = true;
        }

        public void Move(Vector2Int direction)
        {
            transform.localPosition += new Vector3(direction.x, direction.y, 0);
            BasePosition += direction;
        }

        public void MoveTo(Vector2Int newBasePosition)
        {
            var offset = newBasePosition - BasePosition;
            transform.localPosition += new Vector3(offset.x, offset.y, 0);
            BasePosition = newBasePosition;
        }

        public void Rotate(bool clockwise)
        {
            foreach (var block in Blocks)
            {
                var oldBlockCenterOffset = block.Offset - Preset.Anchor;
                Vector2 newBlockCenterOffset;
                if (clockwise)
                {
                    newBlockCenterOffset = new Vector2(oldBlockCenterOffset.y, -oldBlockCenterOffset.x);
                }
                else
                {
                    newBlockCenterOffset = new Vector2(-oldBlockCenterOffset.y, oldBlockCenterOffset.x);
                }
                var newBlockBaseOffset = newBlockCenterOffset + Preset.Anchor;

                Assert.AreEqual(newBlockBaseOffset.x % 1, 0);
                Assert.AreEqual(newBlockBaseOffset.y % 1, 0);

                block.Offset = new Vector2Int((int)newBlockBaseOffset.x, (int)newBlockBaseOffset.y);
                block.Block.transform.localPosition += new Vector3(newBlockCenterOffset.x - oldBlockCenterOffset.x, newBlockCenterOffset.y - oldBlockCenterOffset.y, 0);
                block.Block.transform.eulerAngles -= Vector3.forward * 90;
            }
        }

        public Vector2Int[] GetRotated(bool clockwise)
        {
            Vector2Int[] rotatedOffsets = new Vector2Int[Blocks.Length];
            for (int i = 0; i < Blocks.Length; i++)
            {
                MinoBlockDto block = Blocks[i];
                var oldBlockCenterOffset = block.Offset - Preset.Anchor;
                Vector2 newBlockCenterOffset;
                if (clockwise)
                {
                    newBlockCenterOffset = new Vector2(oldBlockCenterOffset.y, -oldBlockCenterOffset.x);
                }
                else
                {
                    newBlockCenterOffset = new Vector2(-oldBlockCenterOffset.y, oldBlockCenterOffset.x);
                }
                var newBlockBaseOffset = newBlockCenterOffset + Preset.Anchor;

                Assert.AreEqual(newBlockBaseOffset.x % 1, 0);
                Assert.AreEqual(newBlockBaseOffset.y % 1, 0);

                rotatedOffsets[i] = new Vector2Int((int)newBlockBaseOffset.x, (int)newBlockBaseOffset.y);
            }
            return rotatedOffsets;
        }

        private void OnDestroy()
        {
            BeforeDestroyed.Invoke();
        }

        private void OnBlockDestroyed()
        {
            blocksLeftCounter--;

            if (blocksLeftCounter == 0)
                Destroy(gameObject);
        }
    }
}