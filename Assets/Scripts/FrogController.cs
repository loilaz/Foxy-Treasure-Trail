using UnityEngine;
using System.Collections;

public class FrogController : EnemyBase
{
    public float jumpForce = 7f;
    private bool isGrounded = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(JumpLoop());
    }

   
    protected override void Move() { }

    IEnumerator JumpLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(1f);
            if (isGrounded)
                Jump();
            yield return new WaitForSeconds(1f);
        }
    }

    void Jump()
    {
        if (isDead) return;

        
        SetFacing(movingRight);

        float dir = movingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2(dir * speed, jumpForce);
        anim.SetTrigger("Jump");

        isGrounded = false;

       
        FlipDirection();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

       
        if (collision.contacts[0].normal.y > 0.5f)
        {
            rb.linearVelocity = Vector2.zero;
            isGrounded = true;
            return;
        }

        
        if (isGrounded && collision.gameObject.CompareTag("EnemyBlock"))
        {
            FlipDirection();
        }
    }
}