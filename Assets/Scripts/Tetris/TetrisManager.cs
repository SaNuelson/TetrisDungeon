using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TetrisManager : MonoBehaviour
{
    public GameObject BlockPrefab;
    public MinoPreset[] TetrominoPresets;

    public uint QueueSize;
    public List<MinoScript> MinoQueue;

    public UnityEvent QueueChanged = new UnityEvent();

    private void Awake()
    {
        MinoQueue = new List<MinoScript>();
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
        var minoPreset = TetrominoPresets[UnityEngine.Random.Range(0, TetrominoPresets.Length)];

        var newMino = new GameObject("Tetromino");
        newMino.transform.SetParent(transform, false);

        var minoScript = newMino.AddComponent<MinoScript>();
        minoScript.ConstructFromPreset(minoPreset, BlockPrefab);
        minoScript.gameObject.SetActive(false);

        return minoScript;
    }

}
