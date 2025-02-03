using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AshScript : MonoBehaviour, IDropHandler
{
    PaperSpawner paperSpawner;
    private void Start()
    {
        paperSpawner = FindObjectOfType<PaperSpawner>();
    }
    public void OnDrop(PointerEventData eventData)
    {
        // because paper blocks ray:(
        // order matters. first drop then grow flower
        paperSpawner.OnDrop(eventData);

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // skip right button drop
            return;
        }

        if (eventData.pointerDrag != null)
        {
            if (eventData.pointerDrag.CompareTag("Flower"))
            {
                Flower flower = eventData.pointerDrag.GetComponent<Flower>();
                if (!flower.IsGrown())
                {
                    eventData.pointerDrag.GetComponent<Flower>().Grow();
                    paperSpawner.GrowFlowerRemoveAsh();
                }
            }
        }
    }
}
