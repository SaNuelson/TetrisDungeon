using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SnapGrid : MonoBehaviour
{

    public Vector2Int GridSize;

    public Sprite[] SpriteGroup;
    private TileSpriteManager spritePicker;

    [SerializeField] private bool[] Grid;
    [SerializeField] private GameObject[,] blockGrid;

    private void Update()
    {
    }

    public void Reassemble()
    {
        if (spritePicker == null)
        {
            spritePicker = new TileSpriteManager(SpriteGroup, SpriteManagerType.Quad);
        }

        if (blockGrid == null || blockGrid.GetLength(0) != GridSize.x || blockGrid.GetLength(1) != GridSize.y)
        {
            if (blockGrid != null)
                for (int i = 0; i < blockGrid.GetLength(0); i++)
                    for (int j = 0; j < blockGrid.GetLength(1); j++)
                        if (blockGrid[i, j] != null)
                            DestroyImmediate(blockGrid[i, j]);
            blockGrid = new GameObject[GridSize.x, GridSize.y];
        }

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                CheckBlock(x, y);
            }
        }
    }

    private void CheckBlock(int x, int y)
    {
        var doesExist = blockGrid[x, y] != null;
        var shouldExist = Grid[CoordsToIdx(x, y)];

        if (shouldExist && !doesExist)
        {
            var newBlock = new GameObject("Block");
            var blockSpriteRenderer = newBlock.AddComponent<SpriteRenderer>();
            Debug.Log(spritePicker);
            var blockSprite = spritePicker.GetSprite(HasBlock(x, y + 1), HasBlock(x - 1, y), HasBlock(x + 1, y), HasBlock(x, y - 1));
            blockSpriteRenderer.sprite = Instantiate(blockSprite);
            newBlock.transform.SetParent(transform, false);
            newBlock.transform.localPosition = new Vector2(x, y);
            blockGrid[x, y] = newBlock;
        }
        else if (!shouldExist && doesExist)
        {
            DestroyImmediate(blockGrid[x, y]);
            blockGrid[x, y] = null;
        }
        else if (doesExist)
        {
            var block = blockGrid[x, y];
            var blockSpriteRenderer = block.GetComponent<SpriteRenderer>();
            var expectedName = spritePicker.GetSpriteName(HasBlock(x, y + 1), HasBlock(x - 1, y), HasBlock(x + 1, y), HasBlock(x, y - 1));
            if (blockSpriteRenderer.sprite.name != expectedName)
            {
                blockSpriteRenderer.sprite = spritePicker.GetSprite(expectedName);
            }
        }

        bool HasBlock(int x, int y)
        {
            if (x < 0 || y < 0)
                return false;
            if (x >= GridSize.x || y >= GridSize.y)
                return false;
            return Grid[CoordsToIdx(x, y)];
        }
    }

    private int CoordsToIdx(int x, int y) => x * GridSize.y + y;
    private Vector2Int IdxToCoords(int idx) => new Vector2Int(idx / GridSize.y, idx % GridSize.y);
}

[CustomEditor(typeof(SnapGrid))]
public class SnapGridEditor : Editor
{
    SerializedProperty gridSizeProp;
    SerializedProperty gridProp;
    SerializedProperty spriteGroupProp;

    Vector2 scrollPos;

    void OnEnable()
    {
        gridSizeProp = serializedObject.FindProperty("GridSize");
        gridProp = serializedObject.FindProperty("Grid");
        spriteGroupProp = serializedObject.FindProperty("SpriteGroup");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        EditorGUILayout.PropertyField(gridSizeProp);
        EditorGUILayout.PropertyField(spriteGroupProp);
        DrawGrid();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
            (target as SnapGrid).Reassemble();
    }

    private void DrawGrid()
    {
        Vector2Int gridSize = gridSizeProp.vector2IntValue;
        gridProp.arraySize = gridSize.x * gridSize.y;
        var toggleSize = EditorGUIUtility.singleLineHeight;
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        for (int y = gridSize.y - 1; y >= 0; y--)
        {
            EditorGUILayout.BeginHorizontal(new GUIStyle() { fixedWidth = toggleSize, fixedHeight = toggleSize });
            for (int x = 0; x < gridSize.x; x++)
            {
                var coord = x * gridSize.y + y;
                var oldVal = EditorGUILayout.Toggle(gridProp.GetArrayElementAtIndex(coord).boolValue);
                gridProp.GetArrayElementAtIndex(coord).boolValue = oldVal;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }
}
