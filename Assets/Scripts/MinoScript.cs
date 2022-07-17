using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool _isConstructed = false;

    public void ConstructFromPreset(TetrominoPreset preset, TetrisController tetris)
    {
        if (_isConstructed)
        {
            Debug.LogError("TetrominoScript.Construct -- already constructed");
        }

        Blocks = new MinoBlockDto[preset.Offsets.Length];
        BasePosition = tetris.GridTop;
        transform.position = tetris.GridToLocal(tetris.GridTop);

        for (int i = 0; i < preset.Offsets.Length; i++)
        {
            var newBlock = Instantiate(tetris.BlockPrefab);
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

        // Center-point debug
        var centerBlock = Instantiate(tetris.BlockPrefab);
        centerBlock.transform.SetParent(transform, false);
        centerBlock.transform.position = Center - Vector3.one * 0.5f;
        centerBlock.transform.position = new Vector3(centerBlock.transform.position.x, centerBlock.transform.position.y , 0);
        var centerBlockScript = centerBlock.GetComponent<BlockScript>();
        centerBlockScript.BackgroundColor = Color.black;
        centerBlockScript.ForegroundColor = Color.red;

        _isConstructed = true;
    }

    public void Move(Vector2Int direction)
    {
        foreach (var block in Blocks)
        {
            block.Block.transform.position += new Vector3(direction.x, direction.y, 0);
        }
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

    public void Rotate()
    {
        foreach (var block in Blocks)
        {
            var newBlockOffset = new Vector2Int(-block.Offset.y, block.Offset.x);
            var oldBlockTransformOffset = new Vector3(block.Offset.x, block.Offset.y);
            var newBlockTransformOffset = new Vector3(-block.Offset.y, block.Offset.x);
            block.Offset = newBlockOffset;
            block.Block.transform.position += newBlockTransformOffset - oldBlockTransformOffset;
        }
    }

    public Vector3 GetCenter()
    {
        float minX = 0, minY = 0, maxX = 0, maxY = 0;
        foreach(var block in Blocks)
        {
            float blockX = block.Offset.x;
            float blockY = block.Offset.y;

            if (blockX < minX) minX = blockX;
            if (blockY < minY) minY = blockY;
            if (blockX > maxX) maxX = blockX;
            if (blockY > maxY) maxY = blockY;
        }


        return new Vector3((minX + maxX) / 2, (minY + maxY) / 2, 0);
    }

    public Vector3 Center => GetCenter();

}
