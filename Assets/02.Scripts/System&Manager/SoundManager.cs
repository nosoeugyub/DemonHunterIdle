using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using static Utility;

/// <summary>
/// 작성일자 : 2024-07-30
/// 작성자 : 민영 (gksalsdud1234@gmail.com)
/// 클래스 용도 : 사운드 관리
/// </summary>
public enum EAudioMixer { Bgm, Ui, Hero, Enemy } // 새로운 오디오 믹서 유형 추가

public class SoundManager : MonoSingleton<SoundManager>
{
    [Serializable]
    public struct AudioData
    {
        public AudioClip audioClip;
        [Range(0f, 1f)]
        public float volume;
        public EAudioMixer eAudioMixer; // 오디오 믹서 유형 추가
    }

    private AudioSource _originalPrefab;
    private Utility.ObjectPooling<AudioSource> audioPooling = null;

    [SerializeField] private AudioMixerGroup masterMixer = null;
    [SerializeField] private AudioMixerGroup[] audioMixers = null;

    [SerializeField] private List<AudioData> audioDataList = null;

    protected override void Awake()
    {
        base.Awake();

        _originalPrefab = new GameObject().AddComponent<AudioSource>();
        _originalPrefab.transform.parent = transform;
        _originalPrefab.outputAudioMixerGroup = masterMixer;
        _originalPrefab.volume = 1f;

        audioPooling = new ObjectPooling<AudioSource>(_originalPrefab, transform, 100);
    }

    public AudioSource PopObject(EAudioMixer eAudioMixer)
    {
        var returnObj = audioPooling.PopObject();
        returnObj.outputAudioMixerGroup = audioMixers[(int)eAudioMixer];
        returnObj.pitch = 1;
        returnObj.clip = null;

        return returnObj;
    }
    

    /// <summary>
    /// AudioData에 있는 스트링으로 소리 재생
    /// </summary>
    public void PlayAudio(string audioName)
    {
        if (IsAudioNameNullOrEmpty(audioName))
            return;

        var playAudio = audioDataList.Find(audio => audio.audioClip.name == audioName);

        var audioSource = PopObject(playAudio.eAudioMixer);
        audioSource.loop = false;
        audioSource.clip = null;
        audioSource.pitch = 1;
        
        audioSource.volume = playAudio.volume;

        var audioClip = playAudio.audioClip;

        if (audioClip == null)
            return;

        audioSource.PlayOneShot(audioClip);
        StartCoroutine(Disable(audioClip.length + 1, audioSource));
    }
    
    /// <summary>
    /// 매개변수 Clip으로 소리 재생
    /// </summary>
    public void PlayAudio(AudioClip audioClip)
    {
        if (audioClip == null)
            return;

        var playAudio = audioDataList.Find(audio => audio.audioClip == audioClip);

        if (playAudio.audioClip == null)
            return;

        var audioSource = PopObject(playAudio.eAudioMixer);
        audioSource.loop = false;
        audioSource.clip = null;
        audioSource.pitch = 1;

        audioSource.volume = playAudio.volume;

        audioSource.PlayOneShot(audioClip);
        StartCoroutine(Disable(audioClip.length + 1, audioSource));
    }

    /// <summary>
    /// Bgm 같이 루프하는 소리 재생
    /// </summary>
    /// <param name="audioName"></param>
    /// <returns></returns>
    public AudioSource PlayAudioLoop(string audioName)
    {
        if (IsAudioNameNullOrEmpty(audioName))
            return null;

        var playAudio = audioDataList.Find(audio => audio.audioClip.name == audioName);

        var audioSource = PopObject(playAudio.eAudioMixer);
        audioSource.gameObject.SetActive(true);
        audioSource.loop = true;
        audioSource.clip = playAudio.audioClip;
        audioSource.pitch = 1;
        audioSource.volume = playAudio.volume;
        audioSource.Play();

        return audioSource;
    }

    /// <summary>
    /// 루핑되는 오디오를 종료시키는 함수.
    /// PlayAudioLoop에서 반환되는 audioSource를 매개변수로 넣으면 됨
    /// </summary>
    public void StopAudioLoop(AudioSource audioSource)
    {
        audioSource.Stop();
        audioSource.gameObject.SetActive(false);
    }

    bool IsAudioNameNullOrEmpty(string name) => audioPooling == null || name == null || name == string.Empty;
    
}