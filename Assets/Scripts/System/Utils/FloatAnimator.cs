using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatAnimator : MonoBehaviour
{
    public float Ambience;
    public float RoundTime;

    private void Update()
    {
        var pos = transform.localPosition;
        pos.y = Ambience * Mathf.Sin(2f * Mathf.PI / RoundTime * Time.time);
        transform.localPosition = pos;
    }
}