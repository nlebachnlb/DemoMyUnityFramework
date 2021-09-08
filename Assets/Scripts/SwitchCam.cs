using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCam : MonoBehaviour
{
    public GameObject virtualCam;
    public float blendTime = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !collision.isTrigger)
        {
            virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player" && !collision.isTrigger)
        {
            virtualCam.SetActive(false);
        }
    }
}
