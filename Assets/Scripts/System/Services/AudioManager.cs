using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.ServiceLocator;

public class AudioManager : Service
{
    [System.Serializable]
    public class AudioInfo
    {
        public string Name;
        public AudioClip Clip;
    }

    public AudioClip GetSfx(string name)
    {
        AudioClip audio;
        var result = sfxMap.TryGetValue(name, out audio);
        return result ? audio : null;
    }

    public AudioClip GetBgm(string name)
    {
        AudioClip audio;
        var result = bgmMap.TryGetValue(name, out audio);
        return result ? audio : null;
    }

    private void Awake()
    {
        foreach (var info in sfx)
        {
            sfxMap.Add(info.Name, info.Clip);
        }

        foreach (var info in bgm)
        {
            bgmMap.Add(info.Name, info.Clip);
        }
    }

    [SerializeField]
    private List<AudioInfo> sfx = new List<AudioInfo>();
    [SerializeField]
    private List<AudioInfo> bgm = new List<AudioInfo>();
    private Dictionary<string, AudioClip> sfxMap = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> bgmMap = new Dictionary<string, AudioClip>();
}
