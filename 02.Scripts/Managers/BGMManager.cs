using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : Singleton<BGMManager>
{
    public AudioSource audioSource;
    public Dictionary<string, AudioClip> bgmDic = new Dictionary<string, AudioClip>();
    public BgmData[] bgmData;

    [System.Serializable]
    public struct BgmData
    {
        public string name;
        public AudioClip clip;
    }

    private void Start()
    {
        for(int i = 0; i < bgmData.Length; i++)
        {
            bgmDic[bgmData[i].name] = bgmData[i].clip;
        }
    }

    public void PlayBgm(string name)
    {
        audioSource.clip = bgmDic[name];
        audioSource.Play();
    }

    public void StopBgm()
    {
        audioSource.Stop();
    }
}
