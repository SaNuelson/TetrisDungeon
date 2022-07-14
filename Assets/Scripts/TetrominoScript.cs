using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinoBlockDto
{
    public BlockScript Block;
    public Vector2Int Offset;
}

public class TetrominoScript : MonoBehaviour
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

}
