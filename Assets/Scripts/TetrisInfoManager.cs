using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisInfoManager : MonoBehaviour
{
    private TetrisManager tetris;

    private GameObject[] previewBoxes;
    private GameObject[] previewItems;

    private float scaleFactor = 0.6f;

    private void Start()
    {
        tetris = transform.parent.GetComponent<TetrisManager>();
        tetris.QueueChanged.AddListener(OnQueueChanged);

        previewBoxes = new GameObject[tetris.QueueSize];
        previewItems = new GameObject[tetris.QueueSize];
        for (int i = 0; i < previewBoxes.Length; i++)
        {
            previewBoxes[i] = transform.Find("Preview" + (i + 1)).gameObject;
        }
    }

    private void OnQueueChanged()
    {
        for (int i = 0; i < tetris.QueueSize; i++)
        {
            if (previewItems[i] != null)
            {
                Destroy(previewItems[i]);
            }

            var minoPreview = Instantiate(tetris.PeekNextMino(i).gameObject);
            var minoPreviewScript = minoPreview.GetComponent<MinoScript>();
            minoPreview.SetActive(true);
            minoPreview.transform.SetParent(previewBoxes[i].transform, false);
            minoPreview.transform.position -= scaleFactor * minoPreviewScript.Anchor;
            minoPreview.transform.localScale = scaleFactor * Vector3.one;

            previewItems[i] = minoPreview;
        }
    }


}
