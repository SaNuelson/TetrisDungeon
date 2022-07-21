using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisInfoManager : MonoBehaviour
{
    private TetrisManager tetris;
    private GridManager grid;

    private GameObject[] queueBoxes;
    private GameObject[] queueMinos;

    private GameObject swapBox;
    private GameObject swapMino = null;

    private float scaleFactor = 0.6f;

    private void Start()
    {
        tetris = transform.parent.GetComponent<TetrisManager>();
        tetris.QueueChanged.AddListener(OnQueueChanged);

        grid = transform.parent.GetComponentInChildren<GridManager>();
        grid.HeldMinoChanged.AddListener(OnHeldMinoChanged);

        queueBoxes = new GameObject[tetris.QueueSize];
        queueMinos = new GameObject[tetris.QueueSize];
        for (int i = 0; i < queueBoxes.Length; i++)
        {
            queueBoxes[i] = transform.Find("Preview" + (i + 1)).gameObject;
        }

        swapBox = transform.Find("HoldPreview").gameObject;

        OnQueueChanged(); // manual first fill
    }

    private void OnQueueChanged()
    {
        for (int i = 0; i < tetris.QueueSize; i++)
        {
            if (queueMinos[i] != null)
            {
                Destroy(queueMinos[i]);
            }

            var minoPreview = ShowPreview(tetris.PeekNextMino(i), queueBoxes[i]);
            queueMinos[i] = minoPreview.gameObject;
        }
    }

    private void OnHeldMinoChanged()
    {
        if (swapMino != null)
        {
            Destroy(swapMino.gameObject);
        }

        var minoPreview = ShowPreview(grid.HeldMino, swapBox);
        swapMino = minoPreview.gameObject;
    }

    private MinoScript ShowPreview(MinoScript mino, GameObject box)
    {
        var minoPreview = Instantiate(mino.gameObject);
        var minoPreviewScript = minoPreview.GetComponent<MinoScript>();
        minoPreview.SetActive(true);
        minoPreview.transform.SetParent(box.transform, false);
        minoPreview.transform.localPosition = -scaleFactor * minoPreviewScript.Anchor;
        minoPreview.transform.localScale = scaleFactor * Vector3.one;
        return minoPreviewScript;
    }
}
