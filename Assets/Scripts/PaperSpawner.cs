using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PaperSpawner : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject paperPrefab;  // default paper prefab
    [SerializeField] List<GameObject> paperFigures;  // different figures

    [SerializeField] List<GameObject> paperSecrets;  // different secrets

    SecretController secretController;

    // paper state
    bool paperSpawned = false;
    bool paperEmpty = true;
    GameObject paperInstance;
    GameObject secretInstance;

    // lamp state
    bool lampIsActive = false;
    CircleLampScript circleLampScript;
    
    void Start() {
        secretController = FindObjectOfType<SecretController>();
    }

    void SpawnPaper()
    {
        secretController.IncreasePaperCount();
        paperSpawned = true;
        paperInstance = Instantiate(paperPrefab, transform, false);

        if (paperSecrets.Count > 0 && secretController.CanSpawnFirstSecret())
        {
            int choice = Random.Range(0, paperSecrets.Count);
            secretInstance = Instantiate(paperSecrets[choice], paperInstance.transform.position, Quaternion.identity, paperInstance.transform);
            secretInstance.SetActive(false);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");
        GameObject pointerDrag = eventData.pointerDrag;
        if (pointerDrag != null)
        {
            if (pointerDrag.CompareTag("PaperStack") && !paperSpawned)
            {
                SpawnPaper();
            }
            else if (pointerDrag.CompareTag("Lamp"))
            {
                ChangeLampState(pointerDrag);
            }
        }
    }

    public void OnPaperClick()
    {
        Debug.Log("OnPaperClick");
        if (paperEmpty)
        {
            Debug.Log("paperEmpty=true");

            // destroy sheet and create figure
            Vector3 position = paperInstance.transform.position;
            Destroy(paperInstance);
            paperEmpty = false;
            int choice = Random.Range(0, paperFigures.Count);
            paperInstance = Instantiate(paperFigures[choice], position, Quaternion.identity, transform);
        }
        else
        {
            Debug.Log("paperEmpty=false");
            Destroy(paperInstance);
            paperEmpty = true;
            paperSpawned = false;
        }
    }

    private void ChangeLampState(GameObject lamp)
    {
        lampIsActive = !lampIsActive;
        if (secretInstance != null)
        {
            secretInstance.SetActive(lampIsActive);
        }
        if (circleLampScript is null)
        {
            circleLampScript = lamp.GetComponent<CircleLampScript>();
        }
        circleLampScript.ChangeState(lampIsActive);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Debug.Log("Cursor entered table with object: " + eventData.pointerDrag.name);
            if (eventData.pointerDrag.CompareTag("Lamp"))
            {
                ChangeLampState(eventData.pointerDrag);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            // Код для обработки выхода курсора с перетаскиваемым объектом
            Debug.Log("Cursor exited table with object: " + eventData.pointerDrag.name);

            if (eventData.pointerDrag.CompareTag("Lamp"))
            {
                ChangeLampState(eventData.pointerDrag);
            }
        }
    }
}
