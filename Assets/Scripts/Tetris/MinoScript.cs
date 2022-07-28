using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    public MinoPreset Preset;

    private bool _isConstructed = false;

    public Vector3 Anchor => new Vector3(Preset.Anchor.x, Preset.Anchor.y, 0);

    public void ConstructFromPreset(MinoPreset preset, Sprite[] BlockSprites)
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
            var newBlock = new GameObject("NewTile");
            var newBlockScript = newBlock.AddComponent<BlockScript>();
            var blockOffset = preset.Offsets[i];

            newBlock.transform.SetParent(transform, false);
            newBlock.transform.position += new Vector3(blockOffset.x, blockOffset.y, 0);

            bool hasTopEdge = blockOffset.y >= preset.BoxSize.y - 1 || !fillMap[blockOffset.x, blockOffset.y + 1];
            bool hasBotEdge = blockOffset.y <= 0 || !fillMap[blockOffset.x, blockOffset.y - 1];
            bool hasLeftEdge = blockOffset.x <= 0 || !fillMap[blockOffset.x - 1, blockOffset.y];
            bool hasRightEdge = blockOffset.x >= preset.BoxSize.x - 1 || !fillMap[blockOffset.x + 1, blockOffset.y];

            string seekedName = (hasTopEdge ? "0" : "1")
                + (hasRightEdge ? "0" : "1")
                + (hasBotEdge ? "0" : "1") 
                + (hasLeftEdge ? "0" : "1");

            newBlockScript.Color = preset.ForegroundColor;
            foreach(var sprite in BlockSprites)
            {
                if (sprite.name == seekedName)
                {
                    newBlockScript.Sprite = sprite;
                    break;
                }
            }

            Blocks[i] = new MinoBlockDto()
            {
                Block = newBlockScript,
                Offset = blockOffset
            };
        }

        _isConstructed = true;
    }

    public void ConstructPreview(MinoPreset preset)
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
            var newBlock = new GameObject("NewTile");
            var newBlockScript = newBlock.AddComponent<BlockScript>();
            var blockOffset = preset.Offsets[i];

            newBlock.transform.SetParent(transform, false);
            newBlock.transform.position += new Vector3(blockOffset.x, blockOffset.y, 0);

            newBlockScript.Color = preset.ForegroundColor;
            newBlockScript.Sprite = Sprite.Create(Util.CreateSolidTexture(100, 100, Color.yellow), new Rect(0, 0, 100, 100), new Vector2(0.5f, 0.5f));

            Blocks[i] = new MinoBlockDto()
            {
                Block = newBlockScript,
                Offset = blockOffset
            };
        }

        _isConstructed = true;
    }

    public void Move(Vector2Int direction)
    {
        transform.position += new Vector3(direction.x, direction.y, 0);
        BasePosition += direction;
    }

    public void MoveTo(Vector2Int newBasePosition)
    {
        Vector2Int baseOffset = newBasePosition - BasePosition;
        foreach (var block in Blocks)
        {
            block.Block.transform.position += new Vector3(baseOffset.x, baseOffset.y, 0);
        }
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
            block.Block.transform.position += new Vector3(newBlockCenterOffset.x - oldBlockCenterOffset.x, newBlockCenterOffset.y - oldBlockCenterOffset.y, 0);
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

    //private void OnDrawGizmos()
    //{
    //    var botleft = transform.position - new Vector3(.5f, .5f, 0);
    //    var botright = new Vector3(botleft.x + Preset.BoxSize.x, botleft.y);
    //    var topleft = new Vector3(botleft.x, botleft.y + Preset.BoxSize.y);
    //    var topright = new Vector3(botright.x, topleft.y);

    //    Gizmos.DrawLine(topleft, topright);
    //    Gizmos.DrawLine(topright, botright);
    //    Gizmos.DrawLine(botright, botleft);
    //    Gizmos.DrawLine(botleft, topleft);
    //}
}
