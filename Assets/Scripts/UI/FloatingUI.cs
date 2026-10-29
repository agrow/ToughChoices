using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [Header("Floating")]
    [SerializeField] private float floatDistance = 8f;
    [SerializeField] private float floatSpeed = 2f;

    private RectTransform rectTransform;
    private Vector2 startPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * floatSpeed) * floatDistance;

        rectTransform.anchoredPosition =
            startPosition + Vector2.up * yOffset;
    }
}