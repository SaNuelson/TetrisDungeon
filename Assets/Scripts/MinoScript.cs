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

    public void ConstructFromPreset(MinoPreset preset, GameObject blockPrefab)
    {
        if (_isConstructed)
        {
            Debug.LogError("TetrominoScript.Construct -- already constructed");
        }

        Blocks = new MinoBlockDto[preset.Offsets.Length];
        gameObject.name = preset.Name;
        Preset = preset;

        for (int i = 0; i < preset.Offsets.Length; i++)
        {
            var newBlock = Instantiate(blockPrefab);
            var blockOffset = preset.Offsets[i];
            var newBlockScript = newBlock.GetComponent<BlockScript>();

            newBlock.transform.SetParent(transform, false);
            newBlock.transform.position += new Vector3(blockOffset.x, blockOffset.y, 0); // TODO: Account for block size

            newBlockScript.ForegroundColor = preset.ForegroundColor;
            newBlockScript.BackgroundColor = preset.BackgroundColor;

            Blocks[i] = new MinoBlockDto()
            {
                Block = newBlockScript,
                Offset = blockOffset
            };
        }

        //// Box debug
        //for (int x = 0; x < preset.BoxSize.x; x++)
        //{
        //    for (int y = 0; y < preset.BoxSize.y; y++)
        //    {
        //        var newBlock = Instantiate(blockPrefab);
        //        var newBlockScript = newBlock.GetComponent<BlockScript>();

        //        newBlock.transform.SetParent(transform, false);
        //        newBlock.transform.position += new Vector3(x, y, 1);

        //        newBlockScript.ForegroundColor = Color.white;
        //        newBlockScript.BackgroundColor = Color.gray;
        //    }
        //}

        //// Center-point debug
        //var centerBlock = Instantiate(blockPrefab);
        //centerBlock.transform.SetParent(transform, false);
        //centerBlock.transform.position = new Vector3(preset.Anchor.x, preset.Anchor.y, 0);
        //var centerBlockScript = centerBlock.GetComponent<BlockScript>();
        //centerBlockScript.BackgroundColor = Color.black;
        //centerBlockScript.ForegroundColor = Color.red;
        //centerBlock.transform.localScale = 0.5f * Vector3.one;

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

    private void OnDrawGizmos()
    {
        var botleft = transform.position - new Vector3(.5f, .5f, 0);
        var botright = new Vector3(botleft.x + Preset.BoxSize.x, botleft.y);
        var topleft = new Vector3(botleft.x, botleft.y + Preset.BoxSize.y);
        var topright = new Vector3(botright.x, topleft.y);

        Gizmos.DrawLine(topleft, topright);
        Gizmos.DrawLine(topright, botright);
        Gizmos.DrawLine(botright, botleft);
        Gizmos.DrawLine(botleft, topleft);
    }
}
