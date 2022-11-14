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
    /// ���� ������ ���� (music on or off, sfx on or off)
    /// </summary>
    [Serializable]
    public class SoundSettings
    {
        public bool AudioOn = true;
        public bool MusicOn = true;
        public bool SfxOn = true;
    }

    /// <summary>
    /// SoundManager�� ������ �ð��� ó���ϴ� �������� �̱���(PersistentSingleton)
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
            //bgmSource = FieldManager.Instance.GetComponent<AudioSource>();    //[FieldManager ���� ��ũ��Ʈ �ۼ� �ʿ�]
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
                _source = DefalutSfxSource; //��Ȱ��ȭ �Ǵ� ������Ʈ ������ ���� AudioSource�� �޾ƿ��� ����� �ȵǴ� ��� null�� �ְ� ��� clip�� ������
            _source.clip = clip;

            _source.PlayOneShot(_source.clip);
        }

        /// <summary>
        /// HADGameEvent�� �����Ͽ� ��ó
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

