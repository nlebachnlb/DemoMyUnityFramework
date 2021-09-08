using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoPlayBgm : MonoBehaviour
{   
    public string bgmId;
    public float delay;
    public bool loop;

    IEnumerator Start() 
    {
        yield return new WaitForSeconds(delay);
        var bgm = AppRoot.Instance.GetService<BgmController>();
        var clip = AppRoot.Instance.GetService<AudioManager>().GetBgm(bgmId);
        bgm.Play(clip, true, 0.2f, loop);
    }
}
