using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 1f;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }
}