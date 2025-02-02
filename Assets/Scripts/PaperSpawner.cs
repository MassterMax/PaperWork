using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaperSpawner : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject paperPrefab;  // default paper prefab
    [SerializeField] GameObject firstPaperPrefab;  // first paper on the table
    [SerializeField] List<GameObject> paperFigures;  // different figures
    [SerializeField] List<Sprite> defaultSprites;  // different drawings 

    SecretController secretController;

    // paper state
    bool paperSpawned = false;
    bool paperEmpty = true;
    GameObject paperInstance;
    GameObject uvInstance;

    // lamp state
    bool lampIsActive = false;
    CircleLampScript circleLampScript;
    
    void Start() {
        secretController = FindObjectOfType<SecretController>();

        // in start we spawn first-firt paper
        secretController.IncreasePaperCount();
        paperSpawned = true;
        paperInstance = Instantiate(firstPaperPrefab, transform, false);
    }

    void SpawnPaper()
    {
        secretController.IncreasePaperCount();
        paperSpawned = true;
        int secretNumber = secretController.CurrentSecret();
        if (secretNumber == 0) {
            // spawn default paper
            paperInstance = Instantiate(paperPrefab, transform, false);
            if (secretController.PotentialCurrentSecret() == 3) {
                if (Random.Range(0f, 1f) >= 0.5f && defaultSprites.Count > 0) {
                    int index = Random.Range(0, defaultSprites.Count);
                    paperInstance.GetComponent<Image>().sprite = defaultSprites[index];
                }
            }
        } else {
            // spawn secret prefab
            GameObject secretPrefab = secretController.GetSecretPrefab(secretNumber - 1);
            paperInstance = Instantiate(secretPrefab, transform, false);

            if (paperInstance.CompareTag("uvInstance")) {
                uvInstance = paperInstance.transform.GetChild(0).gameObject;
                uvInstance.SetActive(false);
            }
            if (secretNumber == 1) {
                secretController.RevealSecret(1);
            }
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
        if (uvInstance != null)
        {
            uvInstance.SetActive(lampIsActive);

            if (lampIsActive == true && secretController.CurrentSecret() == 2) {
                // if player use lamp - we reveal secret
                secretController.RevealSecret(2);
            }
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
