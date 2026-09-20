using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DragonCombatManager : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 5;
    private int currentHealth;

    // Allows the Receiver to check if the dragon is dead
    public bool IsDead { get; private set; } = false;

    [Header("UI Elements")]
    public Slider healthBar;
    public TextMeshProUGUI victoryText;

    [Header("Animator")]
    public Animator animator;
    private static readonly int DieHash = Animator.StringToHash("DieTrigger");

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
        }
    }

    // Returns TRUE if the dragon died from this hit, FALSE if it survived
    public bool TakeDamage(int damage)
    {
        if (IsDead) return true;

        currentHealth -= damage;

        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
            return true;
        }

        return false;
    }

    private void Die()
    {
        IsDead = true;

        if (animator != null)
        {
            animator.SetTrigger(DieHash);
        }

        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(true);
        }
    }

    public void ResetScene()
    {
        // Instantly reloads the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}