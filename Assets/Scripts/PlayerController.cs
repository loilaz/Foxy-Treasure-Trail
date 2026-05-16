using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    private float direction = 0f;

    private Rigidbody2D player;
    private Animator playerAnimation;

    public float JumpSpeed = 8f;
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    private bool isTouchingGround;

    private Vector3 respawnPoint;
    public GameObject fallDetector;

    public TextMeshProUGUI scoreText;

    public Health health;

    private float damageCooldown = 0f;
    private bool fallTriggered = false;

    
    public int maxLives = 3;
    private int currentLives;
    public Image[] hearts;

    private bool isDead = false;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerAnimation = GetComponent<Animator>();

        respawnPoint = transform.position;

        currentLives = maxLives;
        UpdateHearts();

        scoreText.text = "Score: " + Scoring.totalScore;
    }

    void Update()
    {

        if (isDead) return;
        // 🟢 Ground check
        isTouchingGround = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        
        direction = Input.GetAxis("Horizontal");

        if (direction > 0f)
        {
            player.linearVelocity = new Vector2(direction * speed, player.linearVelocity.y);
            transform.localScale = new Vector2(5, 5);
        }
        else if (direction < 0f)
        {
            player.linearVelocity = new Vector2(direction * speed, player.linearVelocity.y);
            transform.localScale = new Vector2(-5, 5);
        }
        else
        {
            player.linearVelocity = new Vector2(0, player.linearVelocity.y);
        }

      
        if (Input.GetButtonDown("Jump") && isTouchingGround)
        {
            player.linearVelocity = new Vector2(player.linearVelocity.x, JumpSpeed);
        }

       
        playerAnimation.SetFloat("Speed", Mathf.Abs(player.linearVelocity.x));
        playerAnimation.SetBool("OnGround", isTouchingGround);

        
        fallDetector.transform.position =
            new Vector2(transform.position.x, fallDetector.transform.position.y);

    
        if (damageCooldown > 0)
            damageCooldown -= Time.deltaTime;

        
        if (health.currentHealth <= 0f && !isDead)
        {
            isDead = true;
            HandleDeath();
        }
    }

    
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = i < currentLives;
        }
    }

    
    void HandleDeath()
    {
        currentLives--;
        UpdateHearts();

        player.linearVelocity = Vector2.zero;
        playerAnimation.SetTrigger("Die");

        if (currentLives > 0)
        {
            StartCoroutine(RespawnDelay());
        }
        else
        {
            GameOver();
        }
    }

    IEnumerator RespawnDelay()
    {
        yield return new WaitForSeconds(1f);
        Respawn();
    }

    
    void Respawn()
    {
        transform.position = respawnPoint;
        fallTriggered = false;
        health.ResetHealth();
        UpdateHearts();

        player.linearVelocity = Vector2.zero;

        playerAnimation.Play("PlayerIdle");
        playerAnimation.ResetTrigger("Die");

        isDead = false;
    }

   
    void GameOver()
    {
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(1f);

        Scoring.totalScore = 0;

        SceneManager.LoadScene(0);
    }

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            respawnPoint = transform.position;
        }
        else if (collision.CompareTag("NextLevel"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (collision.CompareTag("PreviousLevel"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
        else if (collision.CompareTag("Gem"))
        {
            Scoring.totalScore += 10;
            scoreText.text = "Score: " + Scoring.totalScore;
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Cherry"))
        {
            Scoring.totalScore += 5;
            scoreText.text = "Score: " + Scoring.totalScore;
            collision.gameObject.SetActive(false);
        }
        else if (collision.CompareTag("Finish"))
        {
            StartCoroutine(WinGame());
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Spike"))
        {
            if (damageCooldown <= 0f && !isDead)
            {
                health.TakeDamage(0.2f);

                playerAnimation.ResetTrigger("isHurt");
                playerAnimation.SetTrigger("isHurt");

                damageCooldown = 0.5f;
            }
        }

        
        if (collision.CompareTag("FallDetector"))
        {
            if (!isDead && !fallTriggered)
            {
                fallTriggered = true;
                HandleDeath();
            }
        }
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyBase enemy = collision.collider.GetComponent<EnemyBase>();
        if (enemy == null || enemy.isDead) return;

        ContactPoint2D contact = collision.contacts[0];

        if (contact.normal.y > 0.5f)
        {
            enemy.Die();
            player.linearVelocity = new Vector2(player.linearVelocity.x, JumpSpeed);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EnemyBase enemy = collision.collider.GetComponent<EnemyBase>();
        if (enemy == null || enemy.isDead) return;

        ContactPoint2D contact = collision.contacts[0];

        if (contact.normal.y <= 0.5f)
        {
            if (damageCooldown <= 0f && !isDead)
            {
                health.TakeDamage(0.2f);

                playerAnimation.ResetTrigger("isHurt");
                playerAnimation.SetTrigger("isHurt");

                damageCooldown = 0.5f;
            }
        }
    }

    IEnumerator WinGame()
    {
        isDead = true; 

        player.linearVelocity = Vector2.zero;

        playerAnimation.SetTrigger("Win");

        yield return new WaitForSeconds(5f);

        Scoring.totalScore = 0;

        SceneManager.LoadScene(0);
    }
}