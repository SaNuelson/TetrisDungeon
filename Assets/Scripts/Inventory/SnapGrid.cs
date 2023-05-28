using Assets.Scripts.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapGrid : MonoBehaviour
{

    public Vector2Int GridSize;

    public TileFactory TileFactory;

    [SerializeField] private bool[] Grid;
    [SerializeField] private GameObject[,] blockGrid;

    public void Reassemble(bool force = false)
    {
        Debug.Log("REASSEMBLE");
        if (blockGrid == null || blockGrid.GetLength(0) != GridSize.x || blockGrid.GetLength(1) != GridSize.y)
        {
            if (blockGrid != null)
                for (int i = 0; i < blockGrid.GetLength(0); i++)
                    for (int j = 0; j < blockGrid.GetLength(1); j++)
                        if (blockGrid[i, j] != null)
                            DestroyImmediate(blockGrid[i, j]);
            blockGrid = new GameObject[GridSize.x, GridSize.y];
        }

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                CheckBlock(x, y);
            }
        }
    }

    private void CheckBlock(int x, int y)
    {
        var doesExist = blockGrid[x, y] != null;
        var shouldExist = Grid[CoordsToIdx(x, y)];

        if (shouldExist && !doesExist)
        {
            GameObject newBlock = TileFactory.MakeTile(GetTileName(x, y));
            newBlock.transform.SetParent(transform, false);
            newBlock.transform.localPosition = new Vector2(x, y);
            blockGrid[x, y] = newBlock;
        }
        else if (!shouldExist && doesExist)
        {
            DestroyImmediate(blockGrid[x, y]);
            blockGrid[x, y] = null;
        }
        else if (doesExist)
        {
            var block = blockGrid[x, y];
            var blockSpriteRenderer = block.GetComponent<SpriteRenderer>();
            var expectedName = GetTileName(x, y);
            if (blockSpriteRenderer.sprite.name != expectedName)
            {
                MonoBehaviour.DestroyImmediate(block);
                var newBlock = TileFactory.MakeTile(expectedName);
                newBlock.transform.SetParent(transform, false);
                newBlock.transform.localPosition = new Vector2(x, y);
                blockGrid[x, y] = newBlock;
            }
        }

        string GetTileName(int x, int y)
        {
            if (TileFactory.Type == TileType.Quad)
            {
                return TileFactory.GetTileName(
                    HasBlock(x, y + 1),
                    HasBlock(x + 1, y),
                    HasBlock(x, y - 1),
                    HasBlock(x - 1, y));
            }
            else
            {
                return TileFactory.GetTileName(
                    HasBlock(x, y + 1),
                    HasBlock(x + 1, y + 1),
                    HasBlock(x + 1, y),
                    HasBlock(x + 1, y - 1),
                    HasBlock(x, y - 1),
                    HasBlock(x - 1, y - 1),
                    HasBlock(x - 1, y),
                    HasBlock(x - 1, y + 1));
            }
        }

        bool HasBlock(int x, int y)
        {
            if (x < 0 || y < 0)
                return false;
            if (x >= GridSize.x || y >= GridSize.y)
                return false;
            return Grid[CoordsToIdx(x, y)];
        }
    }

    private int CoordsToIdx(int x, int y) => x * GridSize.y + y;
    private Vector2Int IdxToCoords(int idx) => new Vector2Int(idx / GridSize.y, idx % GridSize.y);
}