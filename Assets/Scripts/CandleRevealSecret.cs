using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CandleRevealSecret : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [SerializeField] Image revealedPart;
    [SerializeField] bool burning = false;
    [SerializeField] int secretNum;
    private bool revealing = false;
    private float progress = 0f;
    private float burningProgress = 0f;
    private Color endColor;
    private float step = 0.25f; // per second

    PaperSpawner paperSpawner;
    private Color startColor;

    bool revealed = false;
    void Start()
    {
        if (!burning) {
            Color c = revealedPart.color;
            c.a = 0f;
            revealedPart.color = c;
            endColor = new Color(1, 0.7f, 0, 0.5f);
        } else {
            endColor = new Color(1, 0.7f, 0, 1f);
            startColor = revealedPart.color;
        }
        paperSpawner = FindObjectOfType<PaperSpawner>();
    }
    
    public Color GetColor() {
        return revealedPart.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (revealing && progress < 1)
        {
            progress += step * Time.deltaTime;
            if (progress > 1)
            {
                progress = 1f;
            }
            if (progress >= 0.40f && !revealed) {
                revealed = true;
                if (!burning) {
                    // end of revealing
                    paperSpawner.RevealSecret(secretNum);
                }
            }

            if (!burning) {
                revealedPart.color = endColor * progress;
            } else {
                Color newColor = endColor * progress + startColor * (1 - progress);
                newColor.a = 1f;
                revealedPart.color = newColor;
            }
        } else if (revealing && progress >= 1f && burning) {
            burningProgress += 5 * step * Time.deltaTime;
            if (burningProgress >= 1)
            {
                burningProgress = 1f;
                // TODO burn it
                paperSpawner.BurnCurrentPaper();
            }
        }
    }

    void StartReveal()
    {
        revealing = true;
    }

    void EndReveal()
    {
        revealing = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            Debug.Log("Cursor entered table with object: " + eventData.pointerDrag.name);
            if (eventData.pointerDrag.CompareTag("Candle"))
            {
                StartReveal();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.CompareTag("Candle"))
            {
                EndReveal();
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.CompareTag("Candle"))
            {
                EndReveal();
            }
        }
        // because paper blocks ray:(
        paperSpawner.OnDrop(eventData);
    }
}
