using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VanishingPlatform : MonoBehaviour
{
    Material mat;
    Collider2D col;

    public float shatterTime;

    bool isDisapearing;


    private void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        col = GetComponent<Collider2D>();
        mat.SetFloat("_Fade", 1f);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.enabled)
            return;

        if (collision.gameObject.CompareTag("Player") && !isDisapearing)
        {
            //AudioManager.instance.PlaySFX("Shatter", 1f);
            StartCoroutine(Vanish());

        }
    }

    IEnumerator Vanish()
    {
        isDisapearing = true;
        float percent = 0f;

        while (percent < 1)
        {
            percent += Time.deltaTime / shatterTime;

            float fade = Mathf.Lerp(1, 0, percent);
            mat.SetFloat("_Fade", fade);
            if (percent >= 0.5)
            {
                col.enabled = false;
            }
            yield return null;
        }

        isDisapearing = false;
        StartCoroutine(Restore());
    }

    IEnumerator Restore()
    {
        yield return new WaitForSeconds(0.5f);
        float percent = 0f;
        while (percent < 1)
        {
            percent += Time.deltaTime / shatterTime;

            float fade = Mathf.Lerp(0, 1, percent);
            mat.SetFloat("_Fade", fade);
            if (percent >= 0.5)
            {
                col.enabled = true;
            }
            yield return null;
        }
    }
}
