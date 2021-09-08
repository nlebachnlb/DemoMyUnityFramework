using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabController : MonoBehaviour
{
    public bool grabbed;
    public Vector2 offset;
    public float radius = 1f;
    public LayerMask grabbable;
    public LayerMask ground;
    public Transform holdPoint;
    public float throwForce;

    Collider2D hit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Grab");
            if (!grabbed)
            {
                //Grab
                //hit = Physics2D.Raycast(transform.position, Vector2.right * transform.localScale.x, distance, grabbable);
                hit = Physics2D.OverlapCircle((Vector2)transform.position + offset * transform.localScale.x, radius, grabbable);

                if (hit != null)
                {
                    //AudioManager.instance.PlaySFX("Throw", 0.5f);
                    grabbed = true;
                }

            }
            else if (!Physics2D.OverlapPoint(holdPoint.position, ground))
            {
                //Throw
                grabbed = false;

                if (hit.gameObject.GetComponent<Rigidbody2D>() != null)
                {
                    //AudioManager.instance.PlaySFX("Throw", 0.5f);
                    var hitRb = hit.gameObject.GetComponent<Rigidbody2D>();
                    hitRb.velocity = Vector2.zero;
                    hitRb.velocity = new Vector2(transform.localScale.x, 1.25f) * throwForce;
                }
            }
        }

        if (grabbed)
        {
            hit.gameObject.transform.position = holdPoint.position;
            hit.gameObject.transform.rotation = holdPoint.rotation;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, (Vector2)transform.position + Vector2.right * transform.localScale.x * radius);
        Gizmos.DrawWireSphere((Vector2)transform.position + offset * transform.localScale.x, radius);
    }
}
