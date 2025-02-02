using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleLampScript : MonoBehaviour
{
    [SerializeField] GameObject circleLight;
    [SerializeField] GameObject parts;
    bool isAtive = false;
    RectTransform circleRectTransform;
    void Start()
    {
        circleRectTransform = circleLight.GetComponent<RectTransform>();
        ChangeState(false);
    }

    // Update is pizdec
    void Update()
    {
        if (isAtive)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10;
            circleLight.transform.position = Camera.main.ScreenToWorldPoint(mousePos);

            // Vector2 circleLocalPos;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(circleRectTransform, Input.mousePosition, Camera.main, out circleLocalPos);
            // circleLight.transform.position = circleLocalPos;
        }
    }

    public void ChangeState(bool active)
    {
        isAtive = active;
        circleLight.SetActive(isAtive);
        parts.SetActive(!isAtive);
    }
}
