using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    public void Disappear()
    {
        anim.SetTrigger("disappear");
    }

    public void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
