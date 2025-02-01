using UnityEngine;

public class MaskController : MonoBehaviour
{
    public Material maskMaterial;
    public float radius = 0.2f;
    public Camera uiCamera; // Ссылка на камеру UI (для Screen Space - Camera)
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (maskMaterial == null || uiCamera == null) return;

        Vector2 localPos;

        // Получаем экранные координаты курсора
        Vector2 screenPos = Input.mousePosition;

        // Переводим экранные координаты в мировые координаты относительно камеры UI
        Ray ray = uiCamera.ScreenPointToRay(screenPos);
        Plane plane = new Plane(uiCamera.transform.forward, rectTransform.position); // Плоскость для расчета на основе RectTransform

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            localPos = rectTransform.InverseTransformPoint(worldPos); // Преобразуем мировые координаты в локальные координаты RectTransform
            Debug.Log($"World position: {worldPos}, Local position: {localPos}");
        }
        else
        {
            return; // Если луч не пересекает плоскость, ничего не делаем
        }

        // Нормализуем координаты относительно размеров изображения
        float normX = (localPos.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width;
        float normY = (localPos.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height;

        // Логируем нормализованные координаты
        Debug.Log($"Normalized cursor position (UV): ({normX}, {normY})");

        // Проверяем, что нормализованные координаты лежат в пределах от 0 до 1
        if (normX < 0 || normX > 1 || normY < 0 || normY > 1)
        {
            Debug.LogError($"Warning: UV coordinates out of bounds! (X: {normX}, Y: {normY})");
        }

        // Отправляем координаты курсора в шейдер
        maskMaterial.SetVector("_CursorPos", new Vector4(normX, normY, 0, 0));
        maskMaterial.SetFloat("_Radius", radius);
    }
}