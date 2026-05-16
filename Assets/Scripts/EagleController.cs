using UnityEngine;

public class EagleController : EnemyBase
{
    public float verticalSpeed = 2f;
    public float moveDistance = 3f;

    private Vector3 topPoint;
    private Vector3 bottomPoint;
    private bool movingUp = true;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        rb.bodyType = RigidbodyType2D.Kinematic;

        bottomPoint = transform.position;
        topPoint = bottomPoint + Vector3.up * moveDistance;
    }

    protected override void Update()
    {
        if (isDead) return;

        Move();
    }

    protected override void Move()
    {
        Vector3 target = movingUp ? topPoint : bottomPoint;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            verticalSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, topPoint) < 0.05f)
            movingUp = false;

        if (Vector3.Distance(transform.position, bottomPoint) < 0.05f)
            movingUp = true;
    }
}