using Assets.Scripts.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tetris
{
    public class TetrisManager : MonoBehaviour
    {
        [Header("Configuration")]
        public MinoShapePreset[] MinoPresets;
        public uint QueueSize;

        [Space(10f)]
        public TileFactoryPreset TileFactoryPreset;
        private TileFactory TileFactory;

        [Tooltip("Strategy in which next minos are selected.")]
        public StrategyType Strategy;
        [Tooltip("In case of MultiBag number of bags, in case of History length of history, otherwise no effect")]
        public int StrategyParam;

        private CollectionSampler<MinoShapePreset> presetPicker;

        [Header("Runtime")]
        public List<MinoScript> MinoQueue;
        public UnityEvent QueueChanged = new UnityEvent();


        public void Init()
        {
            TileFactory = TileFactoryPreset.GetFactory();
            MinoQueue = new List<MinoScript>();
            presetPicker = new CollectionSampler<MinoShapePreset>(MinoPresets, Strategy, StrategyParam);
        }

        private void Awake()
        {
            Init();
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
            QueueChanged.Invoke(); // TODO: workaround for spells (enqueued so queue has leftover minos - pop doesn't trigger fill - queue preview doesn't update)
            return nextMino;
        }

        private void FillQueue()
        {
            if (MinoQueue.Count == QueueSize)
                return;

            while (MinoQueue.Count < QueueSize)
            {
                MinoQueue.Add(CreateMino());
            }

            QueueChanged.Invoke();
        }

        public void EnqueueMino(MinoScript mino)
        {
            MinoQueue.Insert(0, mino);
            QueueChanged.Invoke();
        }

        private MinoScript CreateMino()
        {
            var minoPreset = presetPicker.Next();

            var newMino = new GameObject("Mino");
            newMino.transform.SetParent(transform, false);

            var minoScript = newMino.AddComponent<MinoScript>();
            minoScript.ConstructFromPreset(minoPreset, TileFactory);

            newMino.SetActive(false);

            return minoScript;
        }

    }
}