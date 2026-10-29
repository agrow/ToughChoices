
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressDotsUI : MonoBehaviour
{
    [Header("Dot Prefab")]
    [SerializeField] private Image dotPrefab;

    [Header("Dot Colors")]
    [SerializeField] private Color activeColor = Color.white;

    [SerializeField] private Color inactiveColor =
        new Color(0.4f, 0.4f, 0.45f, 1f);

    private readonly List<Image> dots = new List<Image>();

    public void Initialize(int totalDots)
    {
        // Remove any existing dots
        foreach (Image dot in dots)
        {
            if (dot != null)
            {
                Destroy(dot.gameObject);
            }
        }

        dots.Clear();

        // Create one dot per dialogue line
        for (int i = 0; i < totalDots; i++)
        {
            Image newDot = Instantiate(dotPrefab, transform);

            newDot.gameObject.SetActive(true);
            newDot.raycastTarget = false;

            dots.Add(newDot);
        }

        UpdateProgress(0);
    }

    public void UpdateProgress(int currentIndex)
    {
        for (int i = 0; i < dots.Count; i++)
        {
            dots[i].color =
                i == currentIndex
                    ? activeColor
                    : inactiveColor;
        }
    }
}
