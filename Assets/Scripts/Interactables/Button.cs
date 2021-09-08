using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour
{
    SpriteRenderer button;

    private bool active;

    public Sprite inactivePad;
    public Sprite activePad;

    public LayerMask interactable;
    public Vector2 size;
    public Vector2 offset;

    public UnityEvent OnPressEvent;
    public UnityEvent OnReleaseEvent;

    Collider2D hit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        button = GetComponent<SpriteRenderer>();
        var angle = transform.eulerAngles.z;

        hit = Physics2D.OverlapBox((transform.position + transform.TransformDirection(offset)), size, angle, interactable);
        if (hit != null && !active)
        {
            //AudioManager.instance.PlaySFX("Activate", 0.5f);
            OnPressEvent?.Invoke();
            active = true;
            button.sprite = activePad;
        }
        else if (hit == null && active)
        {
            OnReleaseEvent?.Invoke();
            active = false;
            button.sprite = inactivePad;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector2.zero + offset, size);
    }
}
