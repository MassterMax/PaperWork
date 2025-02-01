using UnityEngine;

public class MaskController : MonoBehaviour
{
    public Material maskMaterial;
    public float radius = 0.2f;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ChangeRadius(float value) {
        maskMaterial.SetFloat("_Radius", value);
    }

    void Update()
    {
        if (maskMaterial == null) return;

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out localPos);
        Debug.Log(localPos);

        float normX = (localPos.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width;
        float normY = (localPos.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height;

        Debug.Log($"Normalized cursor position (UV): ({normX}, {normY})");

        maskMaterial.SetVector("_CursorPos", new Vector4(normX, normY, 0, 0));
    }
}