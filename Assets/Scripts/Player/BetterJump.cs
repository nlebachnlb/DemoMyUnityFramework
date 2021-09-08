using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.AMVC;

public class BetterJump : View<GameplayApp>
{
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Vector2.Dot(rb.velocity, transform.up) == -1) //Falling
        {
            rb.velocity += (Vector2)transform.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (!Input.GetButton("Jump")) //If jumping up and not holding jump button
        {
            rb.velocity += (Vector2)transform.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
