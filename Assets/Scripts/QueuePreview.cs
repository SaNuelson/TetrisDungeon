using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuePreview : MonoBehaviour
{
    public TetrisController Controller;
    [SerializeField] private GameObject[] _previewBoxes;

    // Start is called before the first frame update
    void Start()
    {
        Controller.QueueChangedEvent.AddListener(OnQueueChange);
    }

    public void OnQueueChange()
    {
        print("Redrawing previews");
        for (int i = 0; i < _previewBoxes.Length; i++)
        {
            var oldMino = _previewBoxes[i].transform.Find("MinoPreview" + i);
            if (oldMino != null)
                Destroy(oldMino.gameObject);

            GameObject queuedMino = Instantiate(Controller.GetQueuedMino(i).gameObject);
            MinoScript queuedMinoScript = queuedMino.GetComponent<MinoScript>();
            queuedMino.transform.name = "MinoPreview" + i;
            queuedMino.transform.SetParent(_previewBoxes[i].transform, false);
            queuedMino.transform.localPosition = -queuedMinoScript.Center;
            queuedMino.transform.localScale = Vector3.one * 0.7f;
            queuedMino.gameObject.SetActive(true);
        }
    }
}
