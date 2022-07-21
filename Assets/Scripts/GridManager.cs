using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridManager : MonoBehaviour
{
    [Header("Configuration")]

    [Tooltip("Time for block to fall in seconds")]
    public float GameSpeed = 1f;

    public Vector2Int GridSize = new Vector2Int(8, 16);

    [Header("Controls")]
    public KeyCode DropMinoKey = KeyCode.Space;
    public KeyCode RotateMinoClockwiseKey = KeyCode.UpArrow;
    public KeyCode RotateMinoCounterClockwiseKey = KeyCode.R;
    public KeyCode HoldMinoKey = KeyCode.F;

    [Header("Runtime")]
    public bool IsRunning = true;

    public MinoScript ActiveMino = null;
    public MinoScript ActiveMinoShadow = null;

    public UnityEvent MinoFallen = new UnityEvent();
    public UnityEvent LineCleared = new UnityEvent();

    private TetrisManager tetris;

    private IEnumerator _tetrisClock;
    private IEnumerator RunTetrisClock()
    {
        while (IsRunning)
        {
            Step();
            yield return new WaitForSeconds(GameSpeed);
        }
    }

    private BlockScript[,] grid;

    private void Start()
    {
        grid = new BlockScript[GridSize.x, GridSize.y];

        tetris = transform.parent.GetComponent<TetrisManager>();

        MinoFallen.AddListener(OnMinoFallen);

        _tetrisClock = RunTetrisClock();
        StartCoroutine(_tetrisClock);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMoveMino(Vector2Int.left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMoveMino(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            var collided = !TryMoveMino(Vector2Int.down);
            if (collided)
                FreeMino();
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
    }

    private void Step()
    {
        if (ActiveMino == null)
        {
            print("TV2: Step -- no mino, spawning");
            SpawnMino();
            return;
        }

        var collided = !TryMoveMino(Vector2Int.down);
        if (collided)
        {
            FreeMino();
            SpawnMino();
        }
    }

    public void SpawnMino()
    {
        ActiveMino = tetris.PopNextMino();
        ActiveMino.gameObject.SetActive(true);
        ActiveMino.transform.SetParent(transform, false);
        ActiveMino.BasePosition = new Vector2Int(GridSize.x / 2, GridSize.y);

        var gridOffset = new Vector3(0, GridSize.y / 2f, 0);
        var blockOffset = new Vector3(0.5f, 0.5f, 0); // TODO: do properly
        ActiveMino.transform.position = transform.position + gridOffset + blockOffset;

        ActiveMinoShadow = Instantiate(ActiveMino);
        ActiveMinoShadow.gameObject.name += " Shadow";
        ActiveMinoShadow.transform.SetParent(transform, false);

        // TODO: Make shadows properly
        foreach(var block in ActiveMinoShadow.Blocks)
        {
            block.Block.BackgroundColor = Color.black;
            block.Block.ForegroundColor = Color.gray;
        }

        ResetShadow();
    }

    public void FreeMino()
    {
        // TODO: Destroy, check lines
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
                        grid[x, yy].transform.position += Vector3.down; // TODO: properly
                    
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
            print("TV2: Move collided");
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
            return false;

        ActiveMino.Rotate(); // TODO: clock/counterclock-wise support in minos
        ActiveMinoShadow.Rotate();
        ResetShadow();
        return true;
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
        foreach(var block in mino.Blocks)
        {
            var blockOffset = block.Offset;

            Vector2Int rotatedOffset;
            if (clockwise)
            {
                rotatedOffset = new Vector2Int(-blockOffset.y, blockOffset.x);
            }
            else
            {
                rotatedOffset = new Vector2Int(blockOffset.y, -blockOffset.x);
            }

            if (!IsTileFree(minoPosition + rotatedOffset))
                return false;
        }
        return true;
    }

    public void OnMinoFallen()
    {
        FreeRows();
        SpawnMino();
    }

}
