using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PaperSpawner : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject paperPrefab;  // default paper prefab
    [SerializeField] GameObject firstPaperPrefab;  // first paper on the table
    [SerializeField] List<GameObject> paperFigures;  // different figures
    [SerializeField] List<Sprite> defaultSprites;  // different drawings 
    [SerializeField] GameObject ashPrefab;  // ash for burnable paper
    [SerializeField] GameObject pochitaAshPrefab;  // ash for pochita
    [SerializeField] GameObject goldenPaperPrefab;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject otherScreen;

    List<Color> paperColors = new List<Color>(new Color[] {
        new Color(130.0f/256, 130.0f/256, 210.0f/256),
        new Color(210.0f/256, 92.0f/256, 92.0f/256),
        new Color(180.0f/256, 84.0f/256, 180.0f/256),
    });

    SecretController secretController;

    // paper state
    bool paperSpawned = false;
    bool figureSpawned = false;
    GameObject paperInstance;
    GameObject uvInstance;
    int prevFigureChoice = -1;

    // lamp state
    bool lampIsActive = false;
    CircleLampScript circleLampScript;

    // candle state
    bool candleIsActive = false;
    CandleCircleScript candleCircleScript;

    // flower state
    Flower flower;

    // common state
    bool holdingSomething = false;
    bool pochitaSpawned = false;

    // vault
    Vault vault;
    bool gameFinished = false;
    void Start()
    {
        secretController = FindObjectOfType<SecretController>();

        // in start we spawn first-firt paper
        secretController.IncreasePaperCount();
        paperSpawned = true;
        paperInstance = Instantiate(firstPaperPrefab, transform, false);
        flower = FindObjectOfType<Flower>();

        vault = FindAnyObjectByType<Vault>();

        endScreen.SetActive(false);
    }

    void SpawnPaper()
    {
        secretController.IncreasePaperCount();
        paperSpawned = true;
        int secretNumber = secretController.CurrentSecret();

        // the number we working with now (0 = neutral)
        if (secretNumber <= 0)
        {
            // spawn default paper
            paperInstance = Instantiate(paperPrefab, transform, false);

            // get current cycle state (3 means we can draw)
            // if (secretController.PotentialCurrentSecret() == 3 || secretController.PotentialCurrentSecret() == 4)
            if (secretController.PotentialCurrentSecret() >= 3)
            {
                if (Random.Range(0f, 1f) >= 0.33f && defaultSprites.Count > 0)
                {
                    int index = Random.Range(0, defaultSprites.Count);
                    paperInstance.GetComponent<Image>().sprite = defaultSprites[index];
                }
            }

            // trying to color paper
            if (secretController.PotentialCurrentSecret() >= 4)
            {
                Color paperColor;
                if (secretNumber == -1)
                {
                    paperColor = Color.green;
                }
                else if (Random.Range(0f, 1f) >= 0.66f)
                {
                    int index = Random.Range(0, paperColors.Count);
                    paperColor = paperColors[index];
                }
                else
                {
                    paperColor = Color.white;
                }
                paperInstance.GetComponent<Image>().color = paperColor;
            }
        }
        else
        {
            // spawn secret prefab
            GameObject secretPrefab = secretController.GetSecretPrefab(secretNumber - 1);
            paperInstance = Instantiate(secretPrefab, transform, false);

            if (paperInstance.CompareTag("uvInstance"))
            {
                uvInstance = paperInstance.transform.GetChild(0).gameObject;
                uvInstance.SetActive(false);
            }
            if (secretNumber == 1)
            {
                secretController.RevealSecret(1);
            }
        }
    }

    void SpawnFlowerPaper()
    {
        int currentSecret = secretController.PotentialCurrentSecret();
        GameObject flowerPrefab = secretController.GetSecretPrefab(currentSecret - 1);
        paperInstance = Instantiate(flowerPrefab, transform, false);
        paperSpawned = true;
    }

    void SpawnGoldenPaper()
    {
        paperInstance = Instantiate(goldenPaperPrefab, transform, false);
        gameFinished = true;
        paperSpawned = true;
    }

    public void OnPaperClick()
    {
        if (holdingSomething)
        {
            // do not allow to change state if something in hands
            return;
        }
        if (gameFinished)
        {
            // FINISH THE GAME
            endScreen.SetActive(true);
            otherScreen.SetActive(false);
        }

        Debug.Log("OnPaperClick");
        if (!figureSpawned)
        {
            Debug.Log("figureSpawned=true");

            // destroy sheet and create figure
            Vector3 position = paperInstance.transform.position;

            // check if figure should be colored
            // TODO papers with default colors
            Color figureColor = Color.white;
            if (paperInstance.CompareTag("FireInstance"))
            {
                // extract color
                figureColor = paperInstance.GetComponent<CandleRevealSecret>().GetColor();
                figureColor.a = 1;
            }
            else
            {
                // paper with defined color
                figureColor = paperInstance.GetComponent<Image>().color;
            }

            Destroy(paperInstance);
            figureSpawned = true;

            if (secretController.CanSpawnPochita())
            {
                paperInstance = Instantiate(paperFigures[0], position, Quaternion.identity, transform);
                prevFigureChoice = 0;
                pochitaSpawned = true;
            }
            else
            {
                int choice = Random.Range(1, paperFigures.Count);
                for (int attempt = 0; attempt < 3; ++attempt)
                {
                    if (choice == prevFigureChoice)
                    {
                        choice = Random.Range(1, paperFigures.Count);  // 0 stays for pochita
                    }
                    else
                    {
                        break;
                    }
                }
                paperInstance = Instantiate(paperFigures[choice], position, Quaternion.identity, transform);
                prevFigureChoice = choice;
            }

            // apply color if need
            ApplyColorToAllChildren(paperInstance, figureColor);
        }
        else
        {
            // destroy figure
            Debug.Log("figureSpawned=false");
            Destroy(paperInstance);
            figureSpawned = false;
            paperSpawned = false;
            pochitaSpawned = false;
        }
    }

    private void ApplyColorToAllChildren(GameObject gameObject, Color color)
    {
        Image[] images = gameObject.GetComponentsInChildren<Image>();
        foreach (Image img in images)
        {
            Debug.Log("Found Image component on: " + img.gameObject.name);
            img.color = (img.color + color) / 2;
        }
    }

    private void ChangeLampState(GameObject lamp)
    {
        lampIsActive = !lampIsActive;
        Debug.Log("ChangeLampState, now: " + lampIsActive.ToString());
        if (uvInstance != null)
        {
            uvInstance.SetActive(lampIsActive);

            if (lampIsActive)
            {
                int currentSecret = secretController.CurrentSecret();
                if (currentSecret == 2 || currentSecret == 4)
                {
                    // if player use lamp - we reveal secret
                    secretController.RevealSecret(currentSecret);
                }
            }
        }
        if (circleLampScript is null)
        {
            circleLampScript = lamp.GetComponent<CircleLampScript>();
        }
        circleLampScript.ChangeState(lampIsActive);
    }

    private void ChangeCandleState(GameObject candle)
    {
        candleIsActive = !candleIsActive;
        Debug.Log("ChangeCandleState, now: " + candleIsActive.ToString());
        if (candleCircleScript is null)
        {
            candleCircleScript = candle.GetComponent<CandleCircleScript>();
        }
        candleCircleScript.ChangeState(candleIsActive);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Debug.Log("Cursor entered table with object: " + eventData.pointerDrag.name);
            if (eventData.pointerDrag.CompareTag("Lamp"))
            {
                ChangeLampState(eventData.pointerDrag);
                holdingSomething = true;
            }
            else if (eventData.pointerDrag.CompareTag("Candle"))
            {
                ChangeCandleState(eventData.pointerDrag);
                holdingSomething = true;
            }
            else if (eventData.pointerDrag.CompareTag("PaperStack"))
            {
                holdingSomething = true;
            }
            else if (eventData.pointerDrag.CompareTag("Flower"))
            {
                holdingSomething = true;
            }
            else if (eventData.pointerDrag.CompareTag("GoldenPaper"))
            {
                holdingSomething = true;
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
                holdingSomething = false;
            }
            else if (eventData.pointerDrag.CompareTag("Candle"))
            {
                ChangeCandleState(eventData.pointerDrag);
                holdingSomething = false;
            }
            else if (eventData.pointerDrag.CompareTag("PaperStack"))
            {
                holdingSomething = false;
            }
            else if (eventData.pointerDrag.CompareTag("Flower"))
            {
                holdingSomething = false;
            }
            else if (eventData.pointerDrag.CompareTag("GoldenPaper"))
            {
                holdingSomething = false;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("PaperSpawner - OnDrop");
        GameObject pointerDrag = eventData.pointerDrag;
        if (pointerDrag != null)
        {
            if (pointerDrag.CompareTag("PaperStack"))
            {
                if (!paperSpawned) SpawnPaper();
                holdingSomething = false;
            }
            else if (pointerDrag.CompareTag("Flower"))
            {
                if (!paperSpawned && flower.IsGrown())
                {
                    RevealSecret(5);
                    SpawnFlowerPaper();
                }
                holdingSomething = false;
            }
            else if (pointerDrag.CompareTag("Lamp"))
            {
                ChangeLampState(pointerDrag);
                holdingSomething = false;
            }
            else if (eventData.pointerDrag.CompareTag("Candle"))
            {
                ChangeCandleState(pointerDrag);
                holdingSomething = false;
            }
            else if (eventData.pointerDrag.CompareTag("GoldenPaper"))
            {
                if (!paperSpawned)
                {
                    // FINISH THE GAME, reject all controls
                    SpawnGoldenPaper();
                    eventData.pointerDrag.gameObject.SetActive(false);
                    Debug.LogWarning("YOU WIN!");
                }
                holdingSomething = false;
            }
        }
    }

    public void RevealSecret(int secretNum)
    {
        secretController.RevealSecret(secretNum);
    }

    public void BurnCurrentPaper()
    {
        Debug.Log("burn current paper");

        // destroy sheet and create ash (ash is a fugure actually)
        Vector3 position = paperInstance.transform.position;
        Destroy(paperInstance);
        figureSpawned = true;

        // specific handle for pochita
        if (pochitaSpawned)
        {
            paperInstance = Instantiate(pochitaAshPrefab, position, Quaternion.identity, transform);
            vault.Reveal();
        }
        else
        {
            paperInstance = Instantiate(ashPrefab, position, Quaternion.identity, transform);
        }

        pochitaSpawned = false;
    }

    public void GrowFlowerRemoveAsh()
    {
        Debug.Log("GrowFlowerRemoveAsh");
        OnPaperClick();
        // Destroy(paperInstance);
        // figureSpawned = false;
        // paperSpawned = false;
    }
}
