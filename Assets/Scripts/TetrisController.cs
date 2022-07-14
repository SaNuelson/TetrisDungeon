using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable] public class TetrominoDto
{
    public string Name = "New anymino";
    public Vector2Int[] Offsets;
    public Color ForegroundColor = Color.yellow;
    public Color BackgroundColor = Color.black;
}

public class TetrisController : MonoBehaviour
{

    public GameObject BlockPrefab;
    public TetrominoDto[] TetrominoPresets;

    public float FallTime = 1f;

    public Vector2Int GridSize = new Vector2Int(8, 16);
    private Vector3 BlockSize = new Vector3(1, 1, 0);

    public bool IsRunning = true;


    private TetrominoScript _activeTetromino = null;
    [SerializeField] private BlockScript[,] _gridMinos;

    private IEnumerator _fallClock;

    private Vector3 _fieldCenter;
    private Vector3 _fieldDiagonal;

    public Vector2Int GridTop => new Vector2Int(GridSize.x / 2, GridSize.y);

    // Start is called before the first frame update
    void Start()
    {
        _fieldCenter = transform.position;
        _fieldDiagonal = Vector3.Scale(BlockSize, new Vector3(GridSize.x, GridSize.y, 0));

        _gridMinos = new BlockScript[GridSize.y, GridSize.x];

        _fallClock = RunFallClock();
        StartCoroutine(_fallClock);
        SpawnTetromino();
    }

    IEnumerator RunFallClock()
    {
        while (IsRunning)
        {
            yield return new WaitForSeconds(FallTime);

            FallTetromino();
        }
    }

    void FallTetromino()
    {
        var collided = !TryMoveTetromino(Vector2Int.down);
        if (collided)
        {
            FreeTetromino();
            SpawnTetromino();
        }
    }

    bool TryMoveTetromino(Vector2Int direction)
    {

        if (_activeTetromino == null)
        {
            Debug.LogError("TryMoveTetromino -- no active tetromino");
            return false;
        }

        var basePos = _activeTetromino.BasePosition;
        foreach (var block in _activeTetromino.Blocks)
        {
            var newBlockPos = basePos + block.Offset + direction;
            print("Block new pos " + newBlockPos);

            if (newBlockPos.x < 0 || newBlockPos.x >= GridSize.x)
                return false;

            if (newBlockPos.y < 0)
                return false;

            if (newBlockPos.y >= GridSize.y)
                continue;

            if (_gridMinos[newBlockPos.y, newBlockPos.x] != null)
                return false;
        }

        print("MOVE");
        _activeTetromino.Move(direction);
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

        var basePos = _activeTetromino.BasePosition;
        foreach (var block in _activeTetromino.Blocks)
        {
            var newBlockPos = basePos + new Vector2Int(-block.Offset.y, block.Offset.x);
            print("Block new pos " + newBlockPos);

            if (newBlockPos.x < 0 || newBlockPos.x >= GridSize.x)
                return false;

            if (newBlockPos.y < 0)
                return false;

            if (newBlockPos.y >= GridSize.y)
                continue;

            if (_gridMinos[newBlockPos.y, newBlockPos.x] != null)
                return false;
        }

        print("ROTATE");
        _activeTetromino.Rotate();
        return true;

    }

    /// <summary>
    /// Drop tetromino all the way down.
    /// </summary>
    void DropTetromino()
    {
        while (TryMoveTetromino(Vector2Int.down)) ;
    }

    void SpawnTetromino()
    {
        if (_activeTetromino != null)
        {
            Debug.LogError("SpawnTetromino -- active tetromino exists");
        }

        var tetroPreset = TetrominoPresets[Random.Range(0, TetrominoPresets.Length)];
        var newTetro = new GameObject("Tetromino");

        newTetro.transform.SetParent(transform, false);
        // newTetro.transform.position

        var tetroScript = (TetrominoScript)newTetro.AddComponent(typeof(TetrominoScript));
        tetroScript.Blocks = new MinoBlockDto[tetroPreset.Offsets.Length];
        tetroScript.BasePosition = GridTop;
        tetroScript.transform.position = GridToLocal(GridTop);

        for (int i = 0; i < tetroPreset.Offsets.Length; i++)
        {
            var newBlock = Instantiate(BlockPrefab);
            var blockOffset = tetroPreset.Offsets[i];
            var newBlockScript = newBlock.GetComponent<BlockScript>();

            newBlock.transform.SetParent(newTetro.transform, false);
            newBlock.transform.position += new Vector3(blockOffset.x, blockOffset.y, 0); // TODO: Account for block size
            
            newBlockScript.ForegroundColor = tetroPreset.ForegroundColor;
            newBlockScript.BackgroundColor = tetroPreset.BackgroundColor;

            tetroScript.Blocks[i] = new MinoBlockDto()
            {
                Block = newBlockScript,
                Offset = blockOffset
            };
        }

        _activeTetromino = tetroScript;
    }

    void FreeTetromino()
    {
        print("DROP");
        if (_activeTetromino == null)
        {
            Debug.LogError("DropTetromino -- no active tetromino");
        }

        var tetroBlocks = _activeTetromino.Blocks;
        var tetroPos = _activeTetromino.BasePosition;

        foreach (var block in tetroBlocks)
        {
            var blockPos = tetroPos + block.Offset;

            if (_gridMinos[blockPos.y, blockPos.x] != null)
            {
                Debug.LogError("DropTetromino -- dropping on non-null tile" + blockPos);
            }

            _gridMinos[blockPos.y, blockPos.x] = block.Block;
        }

        _activeTetromino = null;
        ClearFilledRows();
    }

    void ClearFilledRows()
    {
        for (int y = 0; y < GridSize.y; y++)
        {
            print("Check row " + y);

            var all = true;
            for (int x = 0; x < GridSize.x; x++)
            {
                if (_gridMinos[y, x] == null)
                {
                    print("..has empty tile at " + x);
                    all = false;
                    // break;
                }
            }

            if (all)
            {
                print("Found full row, removing...");
                for (int xx = 0; xx < GridSize.x; xx++)
                {
                    Destroy(_gridMinos[y, xx].gameObject);
                    _gridMinos[y, xx] = null;
                }

                for (int yy = y + 1; yy < GridSize.y; yy++)
                {
                    for (int xx = 0; xx < GridSize.x; xx++)
                    {
                        if (_gridMinos[yy, xx] != null)
                            _gridMinos[yy, xx].transform.position += Vector3.down * BlockSize.y;
                        if (yy > 0)
                            _gridMinos[yy - 1, xx] = _gridMinos[yy, xx];
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

    // Update is called once per frame
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

    Vector3 GridToLocal(Vector2Int position)
    {
        var bottomLeft = _fieldCenter - _fieldDiagonal / 2;
        return Vector3.Scale(BlockSize, new Vector3(position.x, position.y, 0)) + bottomLeft + BlockSize / 2;
    }

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

                if (_gridMinos != null && _gridMinos[y, x] != null)
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
}
