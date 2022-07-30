using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Tetris
{
    public class GridManager : MonoBehaviour
    {
        [Header("Configuration")]

        [Tooltip("Time for block to fall in seconds")]
        public float GameSpeed = 1f;

        [Tooltip("Time delay between movements when key is held down")]
        public float InputDelay = .2f;

        private KeyCode LastInput = KeyCode.None;
        private float InputDelayLeft;


        public Vector2Int GridSize = new Vector2Int(8, 16);

        public bool EnableKickFlip = true;

        [Header("Controls")]
        public KeyCode DropMinoKey = KeyCode.Space;
        public KeyCode RotateMinoClockwiseKey = KeyCode.UpArrow;
        public KeyCode RotateMinoCounterClockwiseKey = KeyCode.R;
        public KeyCode HoldMinoKey = KeyCode.F;

        [Header("Runtime (Read-only)")]
        public bool IsRunning = true;

        public MinoScript ActiveMino = null;
        public MinoScript ActiveMinoShadow = null;

        public MinoScript HeldMino = null;
        public bool canHoldout = true;

        public UnityEvent MinoFallen = new UnityEvent();
        public UnityEvent LineCleared = new UnityEvent();
        public UnityEvent HeldMinoChanged = new UnityEvent();

        private TetrisManager tetris;

        private IEnumerator _tetrisClock;
        private IEnumerator RunTetrisClock()
        {
            while (IsRunning)
            {
                yield return new WaitForSeconds(GameSpeed);
                Step();
            }
        }
        
        private void ResetTetrisClock()
        {
            StopCoroutine(_tetrisClock);
            _tetrisClock = RunTetrisClock();
            StartCoroutine(_tetrisClock);
        }

        private BlockScript[,] grid;

        private void Start()
        {
            grid = new BlockScript[GridSize.x, GridSize.y];

            tetris = transform.parent.GetComponent<TetrisManager>();

            MinoFallen.AddListener(OnMinoFallen);

            _tetrisClock = RunTetrisClock();
            Step(); // manual first step
            StartCoroutine(_tetrisClock);

            InputDelayLeft = InputDelay;
        }

        private void Update()
        {
            bool acceptHeldInput = false; // delay left <= 0 OR input changed
            if (Input.GetKey(LastInput))
            {
                InputDelayLeft -= Time.deltaTime;
                if (InputDelayLeft <= 0)
                {
                    InputDelayLeft = InputDelay;
                    acceptHeldInput = true;
                }
            }
            else
            {
                InputDelayLeft = InputDelay;
                acceptHeldInput = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) 
                || (acceptHeldInput && Input.GetKey(KeyCode.LeftArrow)))
            {
                TryMoveMino(Vector2Int.left);
                LastInput = KeyCode.LeftArrow;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow)
                || (acceptHeldInput && Input.GetKey(KeyCode.RightArrow)))
            {
                TryMoveMino(Vector2Int.right);
                LastInput = KeyCode.RightArrow;
            }

            if ( Input.GetKeyDown(KeyCode.DownArrow) 
                || (acceptHeldInput && Input.GetKey(KeyCode.DownArrow)))
            {
                LastInput = KeyCode.DownArrow;
                var collided = !TryMoveMino(Vector2Int.down);
                if (collided)
                    FreeMino();
                ResetTetrisClock();
            }

            if (Input.GetKeyDown(DropMinoKey))
            {
                FallMino();
                FreeMino();
            }
            if (Input.GetKeyDown(RotateMinoClockwiseKey))
            {
                TryRotateMino(clockwise: true);
            }
            if (Input.GetKeyDown(RotateMinoCounterClockwiseKey))
            {
                TryRotateMino(clockwise: false);
            }
            if (Input.GetKeyDown(HoldMinoKey))
            {
                SwapMino();
            }
        }

        private void Step()
        {
            if (ActiveMino == null)
            {
                SpawnMino();
                return;
            }

            var collided = !TryMoveMino(Vector2Int.down);
            if (collided)
            {
                FreeMino();
            }
        }

        public void SpawnMino(MinoScript customMino = null)
        {
            if (ActiveMino != null)
            {
                Debug.LogError("SpawnMino -- active mino exists");
            }

            if (customMino != null)
                ActiveMino = customMino;
            else
                ActiveMino = tetris.PopNextMino();

            ActiveMino.gameObject.SetActive(true);
            ActiveMino.transform.SetParent(transform, false);
            ActiveMino.BasePosition = new Vector2Int(GridSize.x / 2, GridSize.y);

            var gridOffset = new Vector3(0, GridSize.y / 2f, 0);
            var blockOffset = new Vector3(0.5f, 0.5f, 0); // TODO: do properly
            ActiveMino.transform.localPosition = transform.localPosition + gridOffset + blockOffset;

            ActiveMinoShadow = Instantiate(ActiveMino);
            ActiveMinoShadow.gameObject.name += " Shadow";
            ActiveMinoShadow.transform.SetParent(transform, false);

            // TODO: Make shadows properly
            foreach (var block in ActiveMinoShadow.Blocks)
            {
                block.Block.Color = Color.gray;
            }

            ResetShadow();
        }

        public void SwapMino()
        {
            if (!canHoldout)
                return;

            var nextMino = HeldMino;
            HeldMino = ActiveMino;
            ActiveMino = null;
            HeldMino.gameObject.SetActive(false);
            Destroy(ActiveMinoShadow.gameObject);
            SpawnMino(nextMino);
            canHoldout = false;
            HeldMinoChanged.Invoke();
        }

        public void FreeMino()
        {
            foreach (var block in ActiveMino.Blocks)
            {
                var blockPos = ActiveMino.BasePosition + block.Offset;
                grid[blockPos.x, blockPos.y] = block.Block;
                block.Block.transform.SetParent(transform, true);
            }
            Destroy(ActiveMino.gameObject);
            ActiveMino = null;

            Destroy(ActiveMinoShadow.gameObject);
            ActiveMinoShadow = null;

            MinoFallen.Invoke();
        }

        public void FallMino()
        {
            while (TryMoveMino(Vector2Int.down)) ;
        }

        public void FreeRows()
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                var filled = true;
                for (int x = 0; x < GridSize.x; x++)
                {
                    if (grid[x, y] == null)
                        filled = false;
                }

                if (filled)
                {
                    ClearRow(y);
                    y--; // repeat on same row (if next filled row was moved in)
                }
            }

            void ClearRow(int y)
            {
                for (int x = 0; x < GridSize.x; x++)
                {
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }

                for (int yy = y + 1; yy < GridSize.y; yy++)
                {
                    for (int x = 0; x < GridSize.x; x++)
                    {
                        if (grid[x, yy] != null)
                            grid[x, yy].transform.localPosition += Vector3.down; // TODO: properly

                        grid[x, yy - 1] = grid[x, yy];
                    }
                }

                LineCleared.Invoke();
            }
        }

        public bool TryMoveMino(Vector2Int direction)
        {
            if (ActiveMino == null)
            {
                Debug.LogError("TryMoveMino -- no active mino");
                return false;
            }

            if (!CanMove(ActiveMino, direction))
            {
                return false;
            }

            ActiveMino.Move(direction);
            ResetShadow();
            return true;
        }

        public bool TryRotateMino(bool clockwise)
        {
            if (ActiveMino == null)
            {
                Debug.LogError("TryRotateMino -- no active mino");
                return false;
            }

            if (!CanRotate(ActiveMino, clockwise))
            {
                if (EnableKickFlip)
                    return TryKickRotateMino(clockwise);
                return false;
            }

            ActiveMino.Rotate(clockwise); // TODO: clock/counterclock-wise support in minos
            ActiveMinoShadow.Rotate(clockwise);
            ResetShadow();
            return true;
        }

        public bool TryKickRotateMino(bool clockwise)
        {
            return TryKickFlip(Vector2Int.left)
                || TryKickFlip(Vector2Int.right)
                || TryKickFlip(Vector2Int.up);

            bool TryKickFlip(Vector2Int direction)
            {
                if (!CanMove(ActiveMino, direction))
                    return false;

                ActiveMino.Move(direction);

                if (CanRotate(ActiveMino, clockwise))
                {
                    ActiveMino.Rotate(clockwise);
                    ActiveMinoShadow.Rotate(clockwise);
                    ResetShadow();
                    return true;
                }

                ActiveMino.Move(-direction);
                return false;
            }
        }

        public void ResetShadow()
        {
            ActiveMinoShadow.MoveTo(ActiveMino.BasePosition);
            while (CanMove(ActiveMinoShadow, Vector2Int.down))
                ActiveMinoShadow.Move(Vector2Int.down);
        }

        public bool IsTileFree(Vector2Int position)
        {
            if (position.x < 0 || position.x >= GridSize.x)
                return false;

            if (position.y < 0)
                return false;

            if (position.y >= GridSize.y)
                return true;

            if (grid[position.x, position.y] != null)
                return false;

            return true;
        }

        public bool CanMove(MinoScript mino, Vector2Int direction)
        {
            var minoPosition = mino.BasePosition;
            foreach (var block in mino.Blocks)
            {
                var blockOffset = block.Offset;

                if (!IsTileFree(minoPosition + blockOffset + direction))
                    return false;
            }
            return true;
        }

        public bool CanRotate(MinoScript mino, bool clockwise)
        {
            var minoPosition = mino.BasePosition;
            var rotatedOffsets = mino.GetRotated(clockwise);
            foreach (var offset in rotatedOffsets)
            {
                if (!IsTileFree(minoPosition + offset))
                    return false;
            }
            return true;
        }

        public void OnMinoFallen()
        {
            canHoldout = true;
            FreeRows();
            SpawnMino();
        }

    }
}