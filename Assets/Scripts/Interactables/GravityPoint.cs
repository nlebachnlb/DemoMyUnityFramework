using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class GravityPoint : MonoBehaviour
{
    public float gravityScale = 12f, planetRadius = 2f;
    public float gravityMinRange = 1f, gravityMaxRange = 2f;


    // Start is called before the first frame update
    
    void OnTriggerStay2D(Collider2D obj)
    {     

        if (obj.CompareTag("Player") || obj.CompareTag("Box"))
        {
            float gravitationalPower = gravityScale;
            float dist = Vector2.Distance(obj.transform.position, transform.position);

            if (dist > (planetRadius + gravityMinRange))
            {
                float min = planetRadius + gravityMinRange;
                gravitationalPower = gravitationalPower * (((min + gravityMaxRange) - dist) / gravityMaxRange);
            }

            Vector3 dir = (transform.position - obj.transform.position) * gravitationalPower;
            var rb = obj.GetComponent<Rigidbody2D>();
            rb.velocity += (Vector2)dir * Time.deltaTime;
            obj.transform.up = Vector3.MoveTowards(obj.transform.up, -dir, gravityScale * Time.deltaTime);
        }
    }

}
