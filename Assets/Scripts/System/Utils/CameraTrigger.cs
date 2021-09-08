using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class CameraInfo
{
    public float fieldOfView;
}

public enum TriggerActivities
{
    Activate,
    Deactivate
}

public class CameraTrigger : Trigger
{
    public CameraInfo targetAction;
    public float actionDuration;
    public AnimationCurve curve;

    [Header("Other triggers affect")]
    public List<Collider2D> activated = new List<Collider2D>();
    public List<Collider2D> deactivated = new List<Collider2D>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // StopAllCoroutines();
        StartCoroutine(ECameraAnim(actionDuration));

        foreach (var trigger in activated) trigger.enabled = true;
        foreach (var trigger in deactivated) trigger.enabled = false;
    }

    private IEnumerator ECameraAnim(float duration)
    {
        if (cameraController == null) cameraController = GameObject.FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        var src = cameraController.m_Lens.FieldOfView;
        var dst = targetAction.fieldOfView;
        // Debug.LogError("Start tweening: " + this.gameObject.name + ": " + src + ":" + dst);

        float elapsed = 0f;
        // yield return null;

        while (true)
        {
            elapsed += Time.deltaTime;
            float percent = Mathf.Clamp01(elapsed / duration);

            float curvePercent = curve.Evaluate(percent);
            var val = Mathf.LerpUnclamped(src, dst, curvePercent);
            // transform.position = new Vector3(pos.x, pos.y, transform.position.z);
            cameraController.m_Lens.FieldOfView = val;
            // Debug.Log(gameObject.name + ":" + val + ":" + elapsed + ":" + duration);

            if (elapsed > duration)
                break;

            yield return null;
        }

        // Debug.LogError("Done tweening: " + this.gameObject.name);
    }

    [SerializeField]
    private Cinemachine.CinemachineVirtualCamera cameraController;
}
