using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newmoonhana.Tools;
using UnityEngine.Audio;
using System;

namespace Newmoonhana.HADEngine
{
    //public class AudioClipManager_Data : SaveToDictionary_Data<string>
    //{
    //    AudioClip clip;
    //    public AudioClip Clip { get { return clip; } }
    //    public override string Key
    //    {
    //        set
    //        {
    //            clip = Resources.Load<AudioClip>("Audio/" + value);
    //        }
    //    }
    //}
    //public class AudioClipManager_Dictionary : SaveToDictionary<string, AudioClipManager_Data>
    //{
    //    public AudioClip GetClip(string _key)
    //    {
    //        AudioClipManager_Data data = base.GetValue(_key);
    //        return data.Clip;
    //    }
    //}
    //public static class AudioClipManager
    //{
    //    static AudioClipManager_Dictionary data = new AudioClipManager_Dictionary();
    //    public static AudioClip GetClip(string _key)
    //    {
    //        return data.GetClip(_key);
    //    }
    //}

    /// <summary>
    /// 사운드 세팅을 저장 (music on or off, sfx on or off)
    /// </summary>
    [Serializable]
    public class SoundSettings
    {
        public bool AudioOn = true;
        public bool MusicOn = true;
        public bool SfxOn = true;
    }

    /// <summary>
    /// SoundManager는 변수와 시간을 처리하는 지속적인 싱글톤(PersistentSingleton)
    /// </summary>
    [AddComponentMenu("HAD Engine/Managers/Sound Manager")]
    public class SoundManager : HADPersistentSingleton<SoundManager>, HADEventListener<HADGameEvent>
    {
        [Header("Setting")]
        public SoundSettings setting;

        [Header("Master")] [Range(0, 100)] public int masterVolume = 100;
        [Header("BGM")] [Range(0, 100)] public int bgmVolume = 100;
        [Header("SFX")] [Range(0, 100)] public int sfxVolume = 100;
        public int SfxVolume { set { sfxVolume = value; } }
        [Header("Pause")] [SerializeField] bool muteSfxOnPause;

        [Header("Audio Mixer")] [SerializeField] AudioMixer mixer;

        [SerializeField] AudioSource DefalutSfxSource;
        HADGameEvent playsfx_event;
        HADGameEvent setsoundsetting_event;
        AudioSource bgmSource, sfxSource;
        AudioClip clip;

        protected override void Awake()
        {
            setting = new SoundSettings();
            playsfx_event = new HADGameEvent("Play_SFX");
            setsoundsetting_event = new HADGameEvent("Setting_Sound");
            base.Awake();
        }

        protected virtual void Start()
        {
            //bgmSource = FieldManager.Instance.GetComponent<AudioSource>();    //[FieldManager 역할 스크립트 작성 필요]
            HADEventManager.AddListener(this);
            InputSetSoundSetting(setting);
        }

        public void InputSetSoundSetting(SoundSettings _setting)
        {
            setting = _setting;
            HADEventManager.TriggerEvent(setsoundsetting_event);
        }
        void SetSoundSetting()
        {
            float master = setting.AudioOn ? -((100 - masterVolume) / 1.25f) : -80;
            float bgm = setting.MusicOn ? -((100 - bgmVolume) / 1.25f) : -80;
            float sfx = setting.SfxOn ? -((100 - sfxVolume) / 1.25f) : -80;

            mixer.SetFloat("Master_Volume", master);
            mixer.SetFloat("BGM_Volume", bgm);
            mixer.SetFloat("SFX_Volume", sfx);
        }

        void BGMPlay(AudioClip _clip = null)
        {
            if (_clip != null)
                bgmSource.clip = _clip;
            bgmSource.Play();
        }
        public void InputSFXPlay(AudioClip _clip = null)
        {
            InputSFXPlay(DefalutSfxSource, _clip);
        }
        public void InputSFXPlay(AudioSource _source = null, AudioClip _clip = null)
        {
            if (_clip == null)
                return;

            sfxSource = _source;
            clip = _clip;
            HADEventManager.TriggerEvent(playsfx_event);
        }
        void SFXPlay(AudioSource _source)
        {
            if (_source == null)
                _source = DefalutSfxSource; //비활성화 되는 오브젝트 등으로 인해 AudioSource를 받아오면 재생이 안되는 경우 null로 넣고 대신 clip을 가져옴
            _source.clip = clip;

            _source.PlayOneShot(_source.clip);
        }

        /// <summary>
        /// HADGameEvent를 포착하여 대처
        /// </summary>
        /// <param name="eventType">HADGameEvent event.</param>
        public void OnHADEvent(HADGameEvent eventType)
        {
            switch (eventType.event_Name)
            {
                case "Setting_Sound":
                    SetSoundSetting();
                    break;
                case "Play_BGM":
                    SFXPlay(bgmSource);
                    break;
                case "Play_SFX":
                    SFXPlay(sfxSource);
                    break;
            }
        }
    }
}

