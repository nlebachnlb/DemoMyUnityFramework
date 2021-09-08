using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    Rigidbody2D rb;

    private bool isOnPlanet;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isOnPlanet)
        {
            transform.up = Vector3.MoveTowards(transform.up, Vector3.up, rb.gravityScale * Time.deltaTime * 1.5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (!collision.enabled)
        //    return;

        //if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        //{
        //    AudioManager.instance.PlaySFX("Impact", 0.3f);
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            isOnPlanet = true;
            rb.gravityScale = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Planet"))
        {
            isOnPlanet = false;
            rb.gravityScale = 4f;
        }
    }
}
