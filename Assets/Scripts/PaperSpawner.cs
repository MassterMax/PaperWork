using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PaperSpawner : MonoBehaviour, IDropHandler
{
    [SerializeField] GameObject paperPrefab;  // default paper prefab
    [SerializeField] List<GameObject> paperFigures;  // different figures

    // paper state
    bool paperSpawned = false;
    bool paperEmpty = true;
    GameObject paperInstance;

    void SpawnPaper() {
        paperSpawned = true;
        paperInstance = Instantiate(paperPrefab, transform, false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        GameObject pointerDrag = eventData.pointerDrag;
        if (pointerDrag != null && pointerDrag.CompareTag("PaperStack") && !paperSpawned) {
            SpawnPaper();
        }
    }

    public void OnPaperClick() {
        Debug.Log("OnPaperClick");
        if (paperEmpty) {
            Debug.Log("paperEmpty=true");

            // destroy sheet and create figure
            Vector3 position = paperInstance.transform.position;
            Destroy(paperInstance);
            paperEmpty = false;
            int choice = Random.Range(0, paperFigures.Count);
            paperInstance = Instantiate(paperFigures[choice], position, Quaternion.identity, transform);
        } else {
            Debug.Log("paperEmpty=false");
            Destroy(paperInstance);
            paperEmpty = true;
            paperSpawned = false;
        }
    }
}
