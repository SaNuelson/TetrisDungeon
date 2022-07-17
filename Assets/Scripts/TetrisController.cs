using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class TetrisController : MonoBehaviour
{
    [Header("Game Settings")]
    public Vector2Int GridSize = new Vector2Int(8, 16);
    private Vector3 BlockSize = new Vector3(1, 1, 0);

    [Header("Generation Settings")]
    public GameObject BlockPrefab;
    public TetrominoPreset[] TetrominoPresets;

    [Tooltip("Size of queue of next planned tetrominos")]
    public int QueueSize = 5;
    private List<MinoScript> _minoQueue = new List<MinoScript>();

    private MinoScript _activeTetromino = null;
    private MinoScript _activeTetrominoShadow = null;

    private BlockScript[,] _grid;

    [Header("Runtime Settings")]

    public bool IsRunning = true;
    public float FallTime = 1f;

    private IEnumerator _tetrisClock;

    private Vector3 _fieldCenter;
    private Vector3 _fieldDiagonal;

    public Vector2Int GridTop => new Vector2Int(GridSize.x / 2, GridSize.y);

    public UnityEvent QueueChangedEvent = new UnityEvent();

    IEnumerator RunFallClock()
    {
        while (IsRunning)
        {
            yield return new WaitForSeconds(FallTime);

            FallTetromino();
        }
    }

    #region Unity methods

    void Start()
    {
        _fieldCenter = transform.position;
        _fieldDiagonal = Vector3.Scale(BlockSize, new Vector3(GridSize.x, GridSize.y, 0));

        _grid = new BlockScript[GridSize.y, GridSize.x];

        while (_minoQueue.Count < QueueSize)
            _minoQueue.Add(CreateMino());
        QueueChangedEvent.Invoke();

        _tetrisClock = RunFallClock();
        StartCoroutine(_tetrisClock);
        SpawnMino();


        //foreach(var preset in TetrominoPresets)
        //{
        //    var test = new GameObject();
        //    var script = test.AddComponent<MinoScript>();
        //    script.ConstructFromPreset(preset, this);
        //    print("Preset " + preset.Name + " has center " + script.Center);
        //}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMoveTetromino(Vector2Int.left);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMoveTetromino(Vector2Int.right);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryMoveTetromino(Vector2Int.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            DropTetromino();
            FallTetromino();
        }

        var rotate = Input.GetKeyDown(KeyCode.UpArrow);
        if (rotate)
        {
            TryRotateTetromino();
        }
    }

    #endregion

    #region Create & Dispose Methods

    MinoScript CreateMino()
    {
        var minoPreset = TetrominoPresets[UnityEngine.Random.Range(0, TetrominoPresets.Length)];

        var newMino = new GameObject("Tetromino");
        newMino.transform.SetParent(transform, false);

        var minoScript = newMino.AddComponent<MinoScript>();
        minoScript.ConstructFromPreset(minoPreset, this);
        minoScript.gameObject.SetActive(false);

        return minoScript;
    }

    /// <summary>
    /// Generate a new random tetromino and its shadow.
    /// </summary>
    void SpawnMino()
    {
        if (_activeTetromino != null)
        {
            Debug.LogError("SpawnTetromino -- active tetromino exists");
        }

        _activeTetromino = _minoQueue.First();
        _activeTetromino.gameObject.SetActive(true);

        _minoQueue.RemoveAt(0);
        _minoQueue.Add(CreateMino());
        QueueChangedEvent.Invoke();
        
        _activeTetrominoShadow = Instantiate(_activeTetromino);
        _activeTetrominoShadow.transform.SetParent(transform, false);
        foreach(var block in _activeTetrominoShadow.Blocks)
        {
            block.Block.BackgroundColor = new Color(.4f, .4f, .4f);
            block.Block.ForegroundColor = new Color(.8f, .8f, .8f);
        }

        ResetShadow();
    }

    /// <summary>
    /// Let go of tetromino and put it into the game field.
    /// Also destroy the shadow.
    /// </summary>
    void FreeMino()
    {
        if (_activeTetromino == null)
        {
            Debug.LogError("DropTetromino -- no active tetromino");
        }

        var tetroBlocks = _activeTetromino.Blocks;
        var tetroPos = _activeTetromino.BasePosition;

        foreach (var block in tetroBlocks)
        {
            var blockPos = tetroPos + block.Offset;

            if (_grid[blockPos.y, blockPos.x] != null)
            {
                Debug.LogError("DropTetromino -- dropping on non-null tile" + blockPos);
            }

            _grid[blockPos.y, blockPos.x] = block.Block;
        }

        _activeTetromino = null;
        Destroy(_activeTetrominoShadow.gameObject);
        _activeTetrominoShadow = null;

        ClearFilledRows();
    }

    #endregion

    /// <summary>
    /// Compute and set the position of shadow based on position of active tetromino.
    /// </summary>
    private void ResetShadow()
    {
        if (_activeTetromino == null || _activeTetrominoShadow == null)
        {
            Debug.LogError("ComputeShadow -- no active tetromino or shadow");
        }

        _activeTetrominoShadow.MoveTo(_activeTetromino.BasePosition);
        while(CanMove(_activeTetrominoShadow, Vector2Int.down))
        {
            _activeTetrominoShadow.Move(Vector2Int.down);
        }
    }

    /// <summary>
    /// Single step, move mino down, get a new one if path is obstructed
    /// </summary>
    void FallTetromino()
    {
        var collided = !TryMoveTetromino(Vector2Int.down);
        if (collided)
        {
            FreeMino();
            SpawnMino();
        }
    }

    /// <summary>
    /// Try move tetromino in specified direction.
    /// Also recompute shadow position.
    /// </summary>
    /// <param name="direction">2D vector on the grid (x left to right, y bottom to top)</param>
    /// <returns>True if successful, false if failed and aborted.</returns>
    bool TryMoveTetromino(Vector2Int direction)
    {
        if (_activeTetromino == null)
        {
            Debug.LogError("TryMoveTetromino -- no active tetromino");
            return false;
        }

        if (!CanMove(_activeTetromino, direction))
            return false;

        _activeTetromino.Move(direction);
        ResetShadow();
        return true;
    }

    /// <summary>
    /// Try rotate tetromino counter-clockwise
    /// </summary>
    /// <returns>If successful, finish rotation and return true. Do nothing and return false otherwise.</returns>
    bool TryRotateTetromino()
    {
        if (_activeTetromino == null)
        {
            Debug.LogError("TryRotateTetromino -- no active tetromino");
            return false;
        }

        if (!CanRotate(_activeTetromino))
            return false;

        _activeTetromino.Rotate();
        _activeTetrominoShadow.Rotate();
        ResetShadow();
        return true;

    }

    /// <summary>
    /// Drop tetromino all the way down.
    /// </summary>
    void DropTetromino()
    {
        while (TryMoveTetromino(Vector2Int.down)) ;
    }

    /// <summary>
    /// Check for any filled rows and remove them if found.
    /// </summary>
    void ClearFilledRows()
    {
        for (int y = 0; y < GridSize.y; y++)
        {
            print("Check row " + y);

            var all = true;
            for (int x = 0; x < GridSize.x; x++)
            {
                if (_grid[y, x] == null)
                {
                    all = false;
                    // break;
                }
            }

            if (all)
            {
                for (int xx = 0; xx < GridSize.x; xx++)
                {
                    Destroy(_grid[y, xx].gameObject);
                    _grid[y, xx] = null;
                }

                for (int yy = y + 1; yy < GridSize.y; yy++)
                {
                    for (int xx = 0; xx < GridSize.x; xx++)
                    {
                        if (_grid[yy, xx] != null)
                            _grid[yy, xx].transform.position += Vector3.down * BlockSize.y;
                        if (yy > 0)
                            _grid[yy - 1, xx] = _grid[yy, xx];
                    }
                }

                // re-check currently moved row
                y--;
            }
        }
    }

    void GenerateTest()
    {
        var offset = Vector3.Scale(BlockSize, ((Vector3Int)GridSize)) / 2 - BlockSize / 2;

        var newBlock = Instantiate(BlockPrefab);
        newBlock.transform.SetParent(transform, false);
        newBlock.transform.position = GridToLocal(new Vector2Int(0, 0));

        newBlock = Instantiate(BlockPrefab);
        newBlock.transform.SetParent(transform, false);
        newBlock.transform.position = GridToLocal(new Vector2Int(1, 1));
        
        newBlock = Instantiate(BlockPrefab);
        newBlock.transform.SetParent(transform, false);
        newBlock.transform.position = GridToLocal(new Vector2Int(2, 2));
    }

    void GenerateGrid()
    {
        var offset = Vector3.Scale(BlockSize, ((Vector3Int)GridSize)) / 2 - BlockSize / 2;

        for (int i = 0; i < GridSize.x; i++)
        {
            for (int j = 0; j < GridSize.y; j++)
            {
                var newBlock = Instantiate(BlockPrefab);
                newBlock.transform.SetParent(transform, false);
                newBlock.transform.position = transform.position + new Vector3(i, j, 0) - offset;

                var newBlockScript = newBlock.GetComponent<BlockScript>();
                var shadedColor = newBlockScript.ForegroundColor * (i * GridSize.x + j) / (GridSize.x * GridSize.y);
                shadedColor.a = 1;
                newBlockScript.ForegroundColor = shadedColor;
            }
        }
    }

    #region API

    public MinoScript GetQueuedMino(int i)
    {
        if (i < 0 || i >= _minoQueue.Count)
        {
            Debug.LogError("GetQueuedMino -- invalid index " + i);
            return null;
        }
        return _minoQueue[i];
    }

    public Vector3 GridToLocal(Vector2Int position)
    {
        var bottomLeft = _fieldCenter - _fieldDiagonal / 2;
        return Vector3.Scale(BlockSize, new Vector3(position.x, position.y, 0)) + bottomLeft + BlockSize / 2;
    }

    public bool IsTileFree(Vector2Int position)
    {
        if (position.x < 0 || position.x >= GridSize.x)
            return false;

        if (position.y < 0)
            return false;

        if (position.y >= GridSize.y)
            return true;

        if (_grid[position.y, position.x] != null)
            return false;

        return true;
    }

    public bool CanRotate(MinoScript tetromino)
    {
        var basePos = tetromino.BasePosition;
        foreach (var block in tetromino.Blocks)
        {
            var newBlockPos = basePos + new Vector2Int(-block.Offset.y, block.Offset.x);

            if (!IsTileFree(newBlockPos))
                return false;
        }
        return true;
    }

    public bool CanMove(MinoScript tetromino, Vector2Int direction)
    {
        var basePos = tetromino.BasePosition;
        foreach (var block in tetromino.Blocks)
        {
            var newBlockPos = basePos + block.Offset + direction;

            if (!IsTileFree(newBlockPos))
                return false;
        }
        return true;
    }

    #endregion

    #region Editor & Gizmos

    private void OnDrawGizmos()
    {
        _fieldCenter = transform.position;
        _fieldDiagonal = Vector3.Scale(BlockSize, new Vector3(GridSize.x, GridSize.y, 0));

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                var center = GridToLocal(new Vector2Int(x, y));
                var topLeft = center + new Vector3(BlockSize.x / 2, -BlockSize.y / 2);
                var botRight = center + new Vector3(-BlockSize.x / 2, BlockSize.y / 2);

                if (_grid != null && _grid[y, x] != null)
                    DrawGizmoBox(topLeft, botRight, Color.red);
                else
                    DrawGizmoBox(topLeft, botRight);
            }
        }
    }

    private void DrawGizmoBox(Vector3 tl, Vector3 br, Color? color = null)
    {
        Color oldColor = Color.black;
        if (color.HasValue)
        {
            oldColor = Gizmos.color;
            Gizmos.color = color.Value;
        }

        var tr = new Vector3(br.x, tl.y);
        var bl = new Vector3(tl.x, br.y);

        Gizmos.DrawLine(tl, tr);
        Gizmos.DrawLine(tr, br);
        Gizmos.DrawLine(br, bl);
        Gizmos.DrawLine(bl, tl);
        Gizmos.DrawLine(tl, br);
        Gizmos.DrawLine(tr, bl);
    
        if (color.HasValue)
        {
            Gizmos.color = oldColor;
        }
    }

    #endregion

    #region Events
    #endregion
}
