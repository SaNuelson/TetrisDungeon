using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TetrisManager : MonoBehaviour
{
    public Sprite[] BlockSprites;
    public MinoPreset[] MinoPresets;
    // GenericPropertyJSON:{"name":"MinoPresets","type":-1,"arraySize":7,"arrayType":"MinoPreset","children":[{"name":"Array","type":-1,"arraySize":7,"arrayType":"MinoPreset","children":[{"name":"size","type":12,"val":7},{"name":"data","type":-1,"children":[{"name":"Name","type":3,"val":"I"},{"name":"ForegroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":1},{"name":"b","type":2,"val":0.421422958},{"name":"a","type":2,"val":1}]},{"name":"BackgroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":0.5},{"name":"b","type":2,"val":0.220603943},{"name":"a","type":2,"val":1}]},{"name":"BoxSize","type":20,"val":"Vector2(4,4)"},{"name":"Offsets","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"Array","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"size","type":12,"val":4},{"name":"data","type":20,"val":"Vector2(1,0)"},{"name":"data","type":20,"val":"Vector2(1,1)"},{"name":"data","type":20,"val":"Vector2(1,2)"},{"name":"data","type":20,"val":"Vector2(1,3)"}]}]},{"name":"Anchor","type":8,"children":[{"name":"x","type":2,"val":1.5},{"name":"y","type":2,"val":1.5}]}]},{"name":"data","type":-1,"children":[{"name":"Name","type":3,"val":"L"},{"name":"ForegroundColor","type":4,"children":[{"name":"r","type":2,"val":0.94718194},{"name":"g","type":2,"val":1},{"name":"b","type":2,"val":0},{"name":"a","type":2,"val":1}]},{"name":"BackgroundColor","type":4,"children":[{"name":"r","type":2,"val":0.464327782},{"name":"g","type":2,"val":0.5019608},{"name":"b","type":2,"val":0},{"name":"a","type":2,"val":1}]},{"name":"BoxSize","type":20,"val":"Vector2(3,3)"},{"name":"Offsets","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"Array","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"size","type":12,"val":4},{"name":"data","type":20,"val":"Vector2(1,0)"},{"name":"data","type":20,"val":"Vector2(1,1)"},{"name":"data","type":20,"val":"Vector2(1,2)"},{"name":"data","type":20,"val":"Vector2(2,0)"}]}]},{"name":"Anchor","type":8,"children":[{"name":"x","type":2,"val":1},{"name":"y","type":2,"val":1}]}]},{"name":"data","type":-1,"children":[{"name":"Name","type":3,"val":"J"},{"name":"ForegroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":0.7575803},{"name":"b","type":2,"val":1},{"name":"a","type":2,"val":1}]},{"name":"BackgroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":0.3763781},{"name":"b","type":2,"val":0.5019608},{"name":"a","type":2,"val":1}]},{"name":"BoxSize","type":20,"val":"Vector2(3,3)"},{"name":"Offsets","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"Array","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"size","type":12,"val":4},{"name":"data","type":20,"val":"Vector2(1,0)"},{"name":"data","type":20,"val":"Vector2(1,1)"},{"name":"data","type":20,"val":"Vector2(1,2)"},{"name":"data","type":20,"val":"Vector2(0,0)"}]}]},{"name":"Anchor","type":8,"children":[{"name":"x","type":2,"val":1},{"name":"y","type":2,"val":1}]}]},{"name":"data","type":-1,"children":[{"name":"Name","type":3,"val":"S"},{"name":"ForegroundColor","type":4,"children":[{"name":"r","type":2,"val":0.849558353},{"name":"g","type":2,"val":0},{"name":"b","type":2,"val":1},{"name":"a","type":2,"val":1}]},{"name":"BackgroundColor","type":4,"children":[{"name":"r","type":2,"val":0.424681932},{"name":"g","type":2,"val":0},{"name":"b","type":2,"val":0.5019608},{"name":"a","type":2,"val":1}]},{"name":"BoxSize","type":20,"val":"Vector2(3,3)"},{"name":"Offsets","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"Array","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"size","type":12,"val":4},{"name":"data","type":20,"val":"Vector2(0,0)"},{"name":"data","type":20,"val":"Vector2(1,0)"},{"name":"data","type":20,"val":"Vector2(1,1)"},{"name":"data","type":20,"val":"Vector2(2,1)"}]}]},{"name":"Anchor","type":8,"children":[{"name":"x","type":2,"val":1},{"name":"y","type":2,"val":1}]}]},{"name":"data","type":-1,"children":[{"name":"Name","type":3,"val":"Z"},{"name":"ForegroundColor","type":4,"children":[{"name":"r","type":2,"val":1},{"name":"g","type":2,"val":0.224928975},{"name":"b","type":2,"val":0},{"name":"a","type":2,"val":1}]},{"name":"BackgroundColor","type":4,"children":[{"name":"r","type":2,"val":0.5019608},{"name":"g","type":2,"val":0.148452312},{"name":"b","type":2,"val":0},{"name":"a","type":2,"val":1}]},{"name":"BoxSize","type":20,"val":"Vector2(3,3)"},{"name":"Offsets","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"Array","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"size","type":12,"val":4},{"name":"data","type":20,"val":"Vector2(0,1)"},{"name":"data","type":20,"val":"Vector2(1,1)"},{"name":"data","type":20,"val":"Vector2(1,0)"},{"name":"data","type":20,"val":"Vector2(2,0)"}]}]},{"name":"Anchor","type":8,"children":[{"name":"x","type":2,"val":1},{"name":"y","type":2,"val":1}]}]},{"name":"data","type":-1,"children":[{"name":"Name","type":3,"val":"T"},{"name":"ForegroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":1},{"name":"b","type":2,"val":0.9897845},{"name":"a","type":2,"val":1}]},{"name":"BackgroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":0.5019608},{"name":"b","type":2,"val":0.482103676},{"name":"a","type":2,"val":1}]},{"name":"BoxSize","type":20,"val":"Vector2(3,3)"},{"name":"Offsets","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"Array","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"size","type":12,"val":4},{"name":"data","type":20,"val":"Vector2(0,1)"},{"name":"data","type":20,"val":"Vector2(1,1)"},{"name":"data","type":20,"val":"Vector2(1,0)"},{"name":"data","type":20,"val":"Vector2(2,1)"}]}]},{"name":"Anchor","type":8,"children":[{"name":"x","type":2,"val":1},{"name":"y","type":2,"val":1}]}]},{"name":"data","type":-1,"children":[{"name":"Name","type":3,"val":"O"},{"name":"ForegroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":1},{"name":"b","type":2,"val":0.5286541},{"name":"a","type":2,"val":1}]},{"name":"BackgroundColor","type":4,"children":[{"name":"r","type":2,"val":0},{"name":"g","type":2,"val":0.5019608},{"name":"b","type":2,"val":0.285164177},{"name":"a","type":2,"val":1}]},{"name":"BoxSize","type":20,"val":"Vector2(2,2)"},{"name":"Offsets","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"Array","type":-1,"arraySize":4,"arrayType":"Vector2Int","children":[{"name":"size","type":12,"val":4},{"name":"data","type":20,"val":"Vector2(0,0)"},{"name":"data","type":20,"val":"Vector2(1,0)"},{"name":"data","type":20,"val":"Vector2(0,1)"},{"name":"data","type":20,"val":"Vector2(1,1)"}]}]},{"name":"Anchor","type":8,"children":[{"name":"x","type":2,"val":0.5},{"name":"y","type":2,"val":0.5}]}]}]}]}

    public uint QueueSize;
    public List<MinoScript> MinoQueue;

    public StrategyType Strategy;
    private RandomPicker<MinoPreset> presetPicker;

    [Tooltip("In case of MultiBag number of bags, in case of History length of history, otherwise no effect")]
    public int StrategyParam;

    public UnityEvent QueueChanged = new UnityEvent();

    private void Awake()
    {
        MinoQueue = new List<MinoScript>();
        presetPicker = new RandomPicker<MinoPreset>(MinoPresets, Strategy, StrategyParam);
        FillQueue();
    }

    public MinoScript PeekNextMino(int index)
    {
        if (index < 0 || index >= QueueSize)
        {
            Debug.LogError("PeekNextMino -- invalid index " + index);
            return null;
        }

        return MinoQueue[index];
    }

    public MinoScript PopNextMino()
    {
        var nextMino = MinoQueue[0];
        MinoQueue.RemoveAt(0);
        FillQueue();
        return nextMino;
    }

    private void FillQueue()
    {
        if (MinoQueue.Count == QueueSize)
            return;

        while(MinoQueue.Count < QueueSize)
        {
            MinoQueue.Add(CreateMino());
        }

        QueueChanged.Invoke();
    }

    private MinoScript CreateMino()
    {
        var minoPreset = presetPicker.Next();

        var newMino = new GameObject("Tetromino");
        newMino.transform.SetParent(transform, false);

        var minoScript = newMino.AddComponent<MinoScript>();
        minoScript.ConstructFromPreset(minoPreset, BlockSprites);
        minoScript.gameObject.SetActive(false);

        return minoScript;
    }

}
