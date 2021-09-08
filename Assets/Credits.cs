using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            canvasAnim.SetTrigger("HideCredits");
            canvasAnim.GetComponent<MenuController>().freeze = false;
            cameraAnim.SetTrigger("HideCredits");
        }
    }

    [SerializeField]
    private Animator cameraAnim, canvasAnim;
}
