using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Framework.AMVC;

public class Collision : View<GameplayApp>
{
    PlayerController player;

    public LayerMask Ground;
    public LayerMask Wall;

    public Vector2 bottomOffset;

    public float collisionRadius;
    public float wallCheckDistance;

    public bool onGround;
    public bool isTouchingWall;
    public bool onLeftWall;
    public bool onRightWall;

    //public bool isInvincible = false;

    //public float knockBackForceX;
    //public float knockBackForceY;
    //public float knockBackTime;

    //private bool knockFromRight;

    public int wallSide; //1: Right || -1: Left
    // Start is called before the first frame update
    private void Start()
    {
        //player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckCollision();
    }

    private void CheckCollision()
    {
        onGround = Physics2D.OverlapCircle(transform.position + transform.TransformDirection(bottomOffset), collisionRadius, Ground);

        isTouchingWall = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, Wall)
                        || Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, Wall);

        onRightWall = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, Wall);
        onLeftWall = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, Wall);

        wallSide = onRightWall ? 1 : -1;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawRay(transform.position, Vector2.left * wallCheckDistance);
        Gizmos.DrawRay(transform.position, Vector2.right * wallCheckDistance);
       
    }
}
