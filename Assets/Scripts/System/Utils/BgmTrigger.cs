using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmTrigger : Trigger
{
    public string bgmName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var bgm = AppRoot.Instance.GetService<BgmController>();
        var clip = AppRoot.Instance.GetService<AudioManager>().GetBgm(bgmName);
        switch (type)
        {
            case TriggerTypes.Entrance: bgm.Play(clip); break;
            case TriggerTypes.Exit: bgm.Stop(); break;
        }
    }
}
