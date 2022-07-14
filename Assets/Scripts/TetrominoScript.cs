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

    public void Move(Vector2Int direction)
    {
        foreach (var block in Blocks)
        {
            block.Block.transform.position += new Vector3(direction.x, direction.y, 0);
        }

        BasePosition += direction;
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
