using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    private RectTransform bar;
    private Image barImage;
    private float maxWidth;

    public Health health;

    void Start()
    {
        bar = GetComponent<RectTransform>();
        barImage = GetComponent<Image>();
        maxWidth = bar.sizeDelta.x;
    }

    void Update()
    {
        float value = health.currentHealth / health.maxHealth;

        bar.sizeDelta = new Vector2(maxWidth * value, bar.sizeDelta.y);
        barImage.color = value < 0.3f ? Color.red : Color.green;
    }
}