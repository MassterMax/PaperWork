using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleCircleScript : MonoBehaviour
{
    [SerializeField] GameObject candleLight;
    [SerializeField] GameObject parts;
    bool isActive = false;
    void Start()
    {
        ChangeState(false);
    }

    // Update is pizdec
    void Update()
    {
        if (isActive)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            candleLight.transform.position = Camera.main.ScreenToWorldPoint(mousePos);

            // Vector2 circleLocalPos;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(circleRectTransform, Input.mousePosition, Camera.main, out circleLocalPos);
            // circleLight.transform.position = circleLocalPos;
        }
    }

    public void ChangeState(bool active)
    {
        isActive = active;
        candleLight.SetActive(isActive);
        parts.SetActive(!isActive);
    }
}
