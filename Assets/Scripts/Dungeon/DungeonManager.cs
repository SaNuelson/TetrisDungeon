using Assets.Scripts.Tetris;
using Assets.Scripts.Util;
using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonManager : MonoBehaviour
{
    public MinoShapePreset[] RoomShapes;
    public Vector2Int LevelSize;

    public Level CurrentLevel;

    private void Awake()
    {
        GetNextLevel();
    }

    private void GetNextLevel()
    {
        CurrentLevel = new Level(this);
    }
}

[System.Serializable]
public class Level
{
    public Room[,] Grid;
    public Room[] Rooms;

    public Level(DungeonManager dungeon)
    {
        Grid = new Room[dungeon.LevelSize.x, dungeon.LevelSize.y];
        Rooms = new Room[16];

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                var newRoom = new Room();
                Rooms[4 * x + y] = newRoom;
            }
        }
    }
}

[System.Serializable]
public class Room
{
    public Combatable Enemy;
}
