using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;
    public float speed = 2f;
    protected bool movingRight = false;
    public bool isDead = false;

   
    public bool spriteDefaultFacingRight = false;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        SetFacing(movingRight);
    }

    protected virtual void Update()
    {
        if (isDead) return;
        Move();
    }

    protected virtual void Move()
    {
        float dir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        SetFacing(movingRight);
    }

    public virtual void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
        anim.SetTrigger("EnemyDeath");
        Destroy(gameObject, 0.7f);
    }

    protected void FlipDirection()
    {
        movingRight = !movingRight;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("EnemyBlock"))
        {
            FlipDirection();
        }
    }

   
    protected void SetFacing(bool facingRight)
    {
        Vector3 scale = transform.localScale;
        bool shouldFlip = facingRight != spriteDefaultFacingRight;
        scale.x = shouldFlip ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}