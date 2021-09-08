using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PromptTrigger : MonoBehaviour
{
    public GameObject buttonPrompt;
    public UnityEvent triggerEvent;

    bool canInteract;
    public bool isEmpty = false;

    private void Update()
    {
        if (isEmpty)
            return;

        if (canInteract && Input.GetButtonDown("Interact"))
        {
            triggerEvent?.Invoke();
            buttonPrompt.GetComponent<Animator>().SetTrigger("disappear");
            //isEmpty = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isEmpty)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            buttonPrompt.GetComponent<Animator>().SetTrigger("appear");
            canInteract = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isEmpty)
            return;

        if (collision.gameObject.CompareTag("Player"))
        {
            if (buttonPrompt.GetComponent<CanvasGroup>().alpha == 1)
                buttonPrompt.GetComponent<Animator>().SetTrigger("disappear");
            canInteract = false;
        }
    }

    public void DisableTrigger()
    {
        isEmpty = true;
    }
}
