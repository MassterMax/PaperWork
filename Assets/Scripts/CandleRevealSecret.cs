using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CandleRevealSecret : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [SerializeField] Image revealedPart;
    private bool revealing = false;
    private float progress = 0f;
    private Color endColor = new Color(1, 0.7f, 0, 0.5f);
    private float step = 0.25f; // per second

    PaperSpawner paperSpawner;

    bool revealed = false;
    void Start()
    {
        Color c = revealedPart.color;
        c.a = 0f;
        revealedPart.color = c;

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
            if (progress >= 0.33f && !revealed) {
                revealed = true;
                paperSpawner.RevealSecret(3);
            }
            revealedPart.color = endColor * progress;
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
