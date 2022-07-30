using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour
{
    public Level CurrentLevel;


}

public class Level
{
    public int[,] Grid;
    public Room[] Rooms;
}

public class Room
{

}
