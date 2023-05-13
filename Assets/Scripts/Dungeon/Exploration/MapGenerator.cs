using Assets.Scripts.Tetris;
using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MapGenerator
{
    public DungeonManager dungeonManager;
    public bool[,] FillMap;
    public TileFactoryPreset FactoryPreset;
    private TileFactory factory;
    public TileFactory Factory
    {
        get
        {
            if (factory == null)
                factory = FactoryPreset != null ? FactoryPreset.GetFactory() : TileFactory.GetDefault();
            return factory;
        }
    }
    public MinoScript ActiveRoom;
    public System.Random random = new System.Random();
    
    public void Generate()
    {
        FillMap = new bool[dungeonManager.LevelSize.y, dungeonManager.LevelSize.x];
        dungeonManager.StartCoroutine(DebugGenerate());
        return;
        for (int i = 0; i < 10; i++)
        {
            SpawnMino();
            while (random.NextFloat() < 0.5f && TryRotateMino()) ;
            while (TryMoveMino()) ;
        }
    }

    private IEnumerator DebugGenerate()
    {
        float stepTime = 0.1f;
        for (int i = 0; i < 20; i++)
        {

            SpawnMino();
            yield return new WaitForSeconds(stepTime);
            while(true)
            {
                if (random.NextFloat() < stepTime)
                    TryRotateMino();
                else if (!TryMoveMino())
                    break;
                yield return new WaitForSeconds(stepTime);
            }
        }
    }

    public void SpawnMino()
    {
        var preset = random.Choice(dungeonManager.RoomShapes);
        ActiveRoom = MinoScript.CreateFromPreset(preset, Factory);
        ActiveRoom.transform.SetParent(dungeonManager.transform, false);

        var maxx = dungeonManager.LevelSize.x - preset.BoxSize.x;
        var maxy = dungeonManager.LevelSize.y - preset.BoxSize.y;
        var rx = random.Next(maxx);
        var ry = random.Next(maxy);

        var startPosRoll = random.NextFloat();
        if (startPosRoll < .25f)
            ActiveRoom.MoveTo(new Vector2Int(0, ry));
        else if (startPosRoll < .5f)
            ActiveRoom.MoveTo(new Vector2Int(rx, 0));
        else if (startPosRoll < .75f)
            ActiveRoom.MoveTo(new Vector2Int(maxx, ry));
        else
            ActiveRoom.MoveTo(new Vector2Int(rx, maxy));
        Debug.Log("Roll " + startPosRoll);

        foreach(var block in ActiveRoom.Blocks)
        {
            FillMap[ActiveRoom.BasePosition.y + block.Offset.y, ActiveRoom.BasePosition.x + block.Offset.x] = true;
        }
    }

    public bool TryRotateMino()
    {
        // clear old pos
        if (!TrySetBlocks(ActiveRoom, false))
        {
            Debug.LogError("TryMoveMino, failed to clear");
            return false;
        }
        // check if can move
        if (!TrySetBlocks(ActiveRoom.BasePosition, ActiveRoom.GetRotated(true), true))
        {
            // if not, set original position back
            if (!TrySetBlocks(ActiveRoom, true))
            {
                Debug.LogError("TryMoveMino, failed to set back");
            }
            return false;
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

        // clear old pos
        if (!TrySetBlocks(ActiveRoom, false))
        {
            Debug.LogError("TryMoveMino, failed to clear");
            return false;
        }
        // check if can move
        if (!TrySetBlocks(ActiveRoom.BasePosition, ActiveRoom.GetMoved(moveVec), true))
        {
            // if not, set original position back
            if (!TrySetBlocks(ActiveRoom, true))
            {
                Debug.LogError("TryMoveMino, failed to set back");
            }
            return false;
        }
        // else apply move
        ActiveRoom.Move(moveVec);
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
            if (FillMap[basePos.y + offset.y, basePos.x + offset.x] == value)
                return false;
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
}