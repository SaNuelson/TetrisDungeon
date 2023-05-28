using Assets.Scripts.Tetris;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class MapGenerator
{
    public DungeonManager dungeonManager;
    public bool[,] FillMap;
    public TileFactoryPreset FactoryPreset;
    private TileFactory factory;
    public float StepTime;
    public TileFactory Factory
    {
        get
        {
            if (factory == null)
                factory = FactoryPreset != null ? FactoryPreset.GetFactory() : TileFactory.GetDefault();
            return factory;
        }
    }
    public System.Random random = new System.Random();

    public MinoScript ActiveRoom;
    public Direction4 ActiveDirection;

    public void Generate()
    {
        FillMap = new bool[dungeonManager.LevelSize.y, dungeonManager.LevelSize.x];
        dungeonManager.StartCoroutine(DebugGenerate());
    }

    private IEnumerator DebugGenerate()
    {
        TrySpawnFirstMino();
        yield return new WaitForSeconds(StepTime);
        for (int i = 0; i < 20; i++)
        {
            if (!TrySpawnMino())
                yield break;

            yield return new WaitForSeconds(StepTime);
            while(MakeAwareStep())
            {
                yield return new WaitForSeconds(StepTime);
            }
        }
    }

    public bool TrySpawnFirstMino()
    {
        var preset = random.Choice(dungeonManager.RoomShapes);
        ActiveRoom = MinoScript.CreateFromPreset(preset, Factory);
        ActiveRoom.transform.SetParent(dungeonManager.transform, false);
        ActiveRoom.MoveTo(new Vector2Int(dungeonManager.LevelSize.x / 2, dungeonManager.LevelSize.y / 2));
        foreach (var block in ActiveRoom.Blocks)
            FillMap[ActiveRoom.BasePosition.y + block.Offset.y, ActiveRoom.BasePosition.x + block.Offset.x] = true;
        return true;
    }

    public bool TrySpawnMino()
    {
        var preset = random.Choice(dungeonManager.RoomShapes);
        ActiveRoom = MinoScript.CreateFromPreset(preset, Factory);
        ActiveRoom.transform.SetParent(dungeonManager.transform, false);

        var maxx = dungeonManager.LevelSize.x - preset.BoxSize.x;
        var maxy = dungeonManager.LevelSize.y - preset.BoxSize.y;
        var rx = random.Next(maxx);
        var ry = random.Next(maxy);

        ActiveDirection = random.Choice<Direction4>();
        switch(ActiveDirection)
        {
            case Direction4.Right:
                ActiveRoom.MoveTo(new Vector2Int(0, ry));
                break;
            case Direction4.Up:
                ActiveRoom.MoveTo(new Vector2Int(rx, 0));
                break;
            case Direction4.Left:
                ActiveRoom.MoveTo(new Vector2Int(maxx, ry));
                break;
            case Direction4.Down:
                ActiveRoom.MoveTo(new Vector2Int(rx, maxy));
                break;
        }

        if (!CanSetBlocks(ActiveRoom, true))
        {
            UnityEngine.Object.Destroy(ActiveRoom);
            ActiveRoom = null;
            return false;
        }

        foreach(var block in ActiveRoom.Blocks)
            FillMap[ActiveRoom.BasePosition.y + block.Offset.y, ActiveRoom.BasePosition.x + block.Offset.x] = true;

        return true;
    }

    public bool MakeAwareStep()
    {
        var basePos = ActiveRoom.BasePosition;
        
        var activeOffset = ActiveDirection.ToVec2Int();
        var clockwiseOffset = ActiveDirection.Next().ToVec2Int();
        var counterClockwiseOffset = ActiveDirection.Prev().ToVec2Int();

        // clear tiles to not confuse checking mechanism
        TrySetBlocks(ActiveRoom, false);

        int[] distances = new int[]
        {
            GetMaxFallDistance(ActiveRoom, ActiveDirection),
            GetMaxFallDistance(basePos, ActiveRoom.GetMoved(clockwiseOffset), ActiveDirection),
            GetMaxFallDistance(basePos, ActiveRoom.GetMoved(counterClockwiseOffset), ActiveDirection),
            GetMaxFallDistance(basePos, ActiveRoom.GetRotated(true), ActiveDirection),
            GetMaxFallDistance(basePos, ActiveRoom.GetRotated(false), ActiveDirection)
        };
        Debug.Log($"Aware movement choice in dir {ActiveDirection} dists = (none - {distances[0]}, left - {distances[1]}, right - {distances[2]}, crot {distances[3]}, ccrot {distances[4]})");

        Func<bool>[] movements = new Func<bool>[]
        {
            () => TryMoveMino(activeOffset),
            () => TryMoveMino(clockwiseOffset),
            () => TryMoveMino(counterClockwiseOffset),
            () => TryRotateMino(true),
            () => TryRotateMino(false)
        };

        // put tiles back before executing
        TrySetBlocks(ActiveRoom, true);

        var argMax = distances.ArgMax();
        if (distances[argMax] <= 0)
            return MakeRandomStep();
        
        return movements[distances.ArgMax()]();
    }
    
    public bool MakeRandomStep()
    {
        if (random.NextFloat() < 0.8f)
            return TryMoveMino();
        else
            return TryRotateMino();
    }

    public bool TryRotateMino()
    {
        return TryRotateMino(random.Flip());
    }
    public bool TryRotateMino(bool clockwise)
    {
        // clear old pos
        if (!TrySetBlocks(ActiveRoom, false))
            throw new BigOopsieException("Failed to clear old position");

        // check if can move
        if (!TrySetBlocks(ActiveRoom.BasePosition, ActiveRoom.GetRotated(true), true))
        {
            // if not, set original position back
            if (!TrySetBlocks(ActiveRoom, true))
                throw new BigOopsieException("Failed to add back position");
        }
        // else apply move
        ActiveRoom.Rotate(true);
        return true;
    }

    public bool TryMoveMino()
    {
        var goalCenter = new Vector2(dungeonManager.LevelSize.x / 2f, dungeonManager.LevelSize.y / 2f);
        var minoCenter = ActiveRoom.Preset.Anchor + new Vector2(0.5f, 0.5f);

        var goalDirection = goalCenter - ActiveRoom.BasePosition - minoCenter;
        // Debug.Log($"TryMoveMino goal {goalCenter}, mino {ActiveRoom.BasePosition} + {minoCenter}, dir {goalDirection}");
        if (goalDirection.sqrMagnitude <= 1f)
            return false;

        var moveVec = new Vector2Int((int)Mathf.Sign(goalDirection.x), (int)Mathf.Sign(goalDirection.y));
        if (moveVec.sqrMagnitude == 2)
            moveVec = (random.NextFloat() < 0.5f) ? new Vector2Int(moveVec.x, 0) : new Vector2Int(0, moveVec.y);

        return TryMoveMino(moveVec);
    }
    public bool TryMoveMino(Vector2Int direction)
    {
        // clear old pos
        if (!TrySetBlocks(ActiveRoom, false))
            throw new BigOopsieException("Failed to clear old position");

        // check if can move
        if (!TrySetBlocks(ActiveRoom.BasePosition, ActiveRoom.GetMoved(direction), true))
        {
            // if not, set original position back
            if (!TrySetBlocks(ActiveRoom, true))
                throw new BigOopsieException("Failed to add back position");
            return false;
        }
        // else apply move
        ActiveRoom.Move(direction);
        return true;
    }

    private bool CanSetBlocks(MinoScript mino, bool value)
    {
        return CanSetBlocks(mino.BasePosition, mino.Blocks, value);
    }
    private bool CanSetBlocks(Vector2Int basePos, MinoBlockData[] blocks, bool value)
    {
        Vector2Int[] blockOffsets = new Vector2Int[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
            blockOffsets[i] = blocks[i].Offset;
        return CanSetBlocks(basePos, blockOffsets, value);
    }
    private bool CanSetBlocks(Vector2Int basePos, Vector2Int[] blockOffsets, bool value)
    {
        foreach (var offset in blockOffsets)
        {
            var x = basePos.x + offset.x;
            var y = basePos.y + offset.y;

            if (IsOutOfBounds(new Vector2Int(x, y)))
            {
                Debug.LogWarning($"CanSetBlocks({basePos}, [{string.Join(", ", blockOffsets)}], {value}) -- OOB");
                return false;
            }

            if (FillMap[y, x] == value)
            {
                Debug.LogWarning($"CanSetBlocks({basePos}, [{string.Join(", ", blockOffsets)}], {value}) -- Y{y}, X{x}] failed to set to {value} (on grid Y{FillMap.GetLength(0)}, X{FillMap.GetLength(1)}).");
                return false;
            }
        }
        return true;
    }

    private bool TrySetBlocks(MinoScript mino, bool value)
    {
        return TrySetBlocks(mino.BasePosition, mino.Blocks, value);
    }
    private bool TrySetBlocks(Vector2Int basePos, MinoBlockData[] blocks, bool value)
    {
        Vector2Int[] blockOffsets = new Vector2Int[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
            blockOffsets[i] = blocks[i].Offset;
        return TrySetBlocks(basePos, blockOffsets, value);
    }
    private bool TrySetBlocks(Vector2Int basePos, Vector2Int[] offsets, bool value)
    {
        if (!CanSetBlocks(basePos, offsets, value))
            return false;
        foreach (var offset in offsets)
        {
            FillMap[basePos.y + offset.y, basePos.x + offset.x] = value;
        }
        return true;
    }

    private int GetMaxFallDistance(MinoScript mino, Direction4 direction)
    {
        return GetMaxFallDistance(mino.BasePosition, mino.Blocks, direction);
    }
    private int GetMaxFallDistance(Vector2Int basePos, MinoBlockData[] blocks, Direction4 direction)
    {
        Vector2Int[] blockOffsets = new Vector2Int[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
            blockOffsets[i] = blocks[i].Offset;
        return GetMaxFallDistance(basePos, blockOffsets, direction);
    }
    private int GetMaxFallDistance(Vector2Int basePos, Vector2Int[] offsets, Direction4 direction)
    {
        Vector2Int delta = direction.ToVec2Int();

        // Incrementally try offseting and checking a ghost
        int distance = 0;
        Vector2Int[] tempOffsets = new Vector2Int[offsets.Length];
        offsets.CopyTo(tempOffsets, 0);
        while(CanSetBlocks(basePos, tempOffsets, true))
        {
            distance++;
            for (int i = 0; i < tempOffsets.Length; i++)
                tempOffsets[i] += delta;

            if (IsOutOfBounds(basePos, tempOffsets))
                return -1;
        }

        return distance;
    }
    private bool IsOutOfBounds(MinoScript mino)
    {
        return IsOutOfBounds(mino.BasePosition, mino.Blocks);
    }
    private bool IsOutOfBounds(Vector2Int basePos, MinoBlockData[] blocks)
    {
        foreach (var block in blocks)
            if (IsOutOfBounds(basePos + block.Offset))
                return true;
        return false;
    }
    private bool IsOutOfBounds(Vector2Int basePos, Vector2Int[] offsets)
    {
        foreach (var offset in offsets)
            if (IsOutOfBounds(basePos + offset))
                return true;
        return false;
    }
    private bool IsOutOfBounds(Vector2Int position)
    {
        return position.x < 0 
            || position.y < 0
            || position.x >= dungeonManager.LevelSize.x
            || position.y >= dungeonManager.LevelSize.y;
    }
}