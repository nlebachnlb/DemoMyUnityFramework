using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.ServiceLocator;
using DG.Tweening;

public class BgmController : Service
{
    public void Init()
    {
        audioLayers = GetComponentsInChildren<AudioSource>();
        layerAvailability = new bool[audioLayers.Length];
        for (int i = 0; i < audioLayers.Length; ++i) layerAvailability[i] = true;
        mainSwap = 0;
    }

    public void Play(AudioClip clip, bool crossFade = true, float duration = 1f, bool loop = true)
    {
        int availableLayer = 0;
        for (int i = 0; i < audioLayers.Length; ++i) 
            if (!audioLayers[i].isPlaying) 
            {
                availableLayer = i;
                break;
            }

        StopAllCoroutines();
        StartCoroutine(ECrossFade(clip, availableLayer, duration, loop));
    }

    public void Stop(bool crossFade = true, float duration = 1f)
    {
        StopAllCoroutines();
        StartCoroutine(EStop(duration));
    }

    private IEnumerator EStop(float duration)
    {
        DOTweenModuleAudio.DOFade(audioLayers[mainSwap], 0f, duration);
        yield return new WaitForSeconds(duration);
        audioLayers[mainSwap].Stop();
    }

    private IEnumerator ECrossFade(AudioClip clip, int availableLayer, float duration, bool loop) 
    {
        audioLayers[availableLayer].volume = 0f;
        audioLayers[availableLayer].Stop();
        audioLayers[availableLayer].loop = loop;
        audioLayers[availableLayer].clip = clip;
        audioLayers[availableLayer].Play();
        DOTweenModuleAudio.DOFade(audioLayers[mainSwap], 0f, duration);
        DOTweenModuleAudio.DOFade(audioLayers[availableLayer], 1f, duration);
        yield return new WaitForSeconds(duration);
        // audioLayers[mainSwap].Stop();
        mainSwap = availableLayer;
    }

    private void Awake()
    {
        Init();
    }

    // Main layer has index 0
    private AudioSource[] audioLayers;
    private bool[] layerAvailability;
    private AudioManager audioManager;
    private int mainSwap;
}
