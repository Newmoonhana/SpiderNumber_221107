using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newmoonhana.Tools;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Newmoonhana.HADEngine
{
    /// <summary>
    /// ������ �⺻ �̺�Ʈ ���
    /// LevelStart : ���� ���� �� LevelManager�� ���� Ʈ����
    ///	LevelComplete : ���� Ŭ���� �� Ʈ����
    /// LevelEnd : (*����)
    ///	Pause : �Ͻ����� ���� �� Ʈ����
    ///	UnPause : �Ͻ����� �ߴ� ���� �������� ���ư� �� Ʈ����
    ///	PlayerDeath : �÷��̾� ĳ���� ��� �� Ʈ����
    ///	RespawnStarted : �÷��̾� ĳ���� ���� �� Ʈ����
    ///	RespawnComplete : �÷��̾� ĳ���� ���� ���� �� Ʈ����
    ///	GameOver : ��� ������ �Ҿ��� �� LevelManager�� ���� Ʈ����
    /// Repaint : UI �������� ��û �� Ʈ����
    /// TogglePause : �Ͻ�����(�Ǵ� ����)�� ���� ��û �� Ʈ����
    /// </summary>
    public enum HADEngineEventTypes
    {
        Setting_Frame,
        Setting_Screen,
        Setting_OrthographicSize,
        Pause,
        UnPause,
        GameOver
    }

    /// <summary>
	/// ���� �� ���� ��ȣ�� ������ �� ���Ǵ� �̺�Ʈ ����(����)
	/// </summary>
	public struct HADEngineEvent
    {
        public HADEngineEventTypes event_type;
        //public Character OriginCharacter;   //�������� ���ʹ����� �� ž��� ĳ���� �Լ��� ������ִµ� ���� �����Ұ���. �����Ҷ� �ּ� ���� �� �ڵ� �����ϸ� ��

        /// <summary>
		/// ������. <see cref="Newmoonhana.HADEngine.HADEngineEvent"/> struct�� �� �ν���Ʈ�� �ʱ�ȭ
		/// </summary>
		/// <param name="eventType">Event type.</param>
		//public HADEngineEvent(HADEngineEventTypes eventType, Character originCharacter)
        public HADEngineEvent(HADEngineEventTypes eventType)
        {
            event_type = eventType;
            //OriginCharacter = originCharacter;
        }

        static HADEngineEvent e;
        //public static void Trigger(HADEngineEventTypes eventType, Character originCharacter)
        public static void Trigger(HADEngineEventTypes eventType)
        {
            e.event_type = eventType;
            //OriginCharacter = originCharacter;
            HADEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
	/// GameManager�� ������ �ð��� ó���ϴ� �������� �̱���(PersistentSingleton)
	/// </summary>
	[AddComponentMenu("HAD Engine/Managers/Game Manager")]
    public class GameManager : HADPersistentSingleton<GameManager>, HADEventListener<HADGameEvent>, HADEventListener<HADEngineEvent>
    {
        [Tooltip("������ �����ӷ�")]
        public int targetFrameRate = 300;

        [Header("���� ���� ��")]
        [Tooltip("���� ���� �� �̵��� ��")]
        public string gameoverScene;

        /// ������ �Ͻ����� �� �� true
        public bool Paused { get; set; }

        //�޴�â
        protected bool _menuOpen = false;

        //�ػ�
        public EventSystem eventSystem;
        int screen_width, screen_height;
        public int ScreenW { set { screen_width = value; } }
        public int ScreenH { set { screen_height = value; } }
        FullScreenMode screen_mode;
        const float inchToCm = 2.54f;   //��ġ->��ġ����
        readonly float dragThresholdCM = 0.5f;  //��ġ�� �������κ��� ���� �� ������ �з��� ���� ���� ����

        HADGameEvent pause_event = new HADGameEvent("Input_Pause");
#if (UNITY_ANDROID || UNITY_IPHONE)
        bool isFrameDown = false;
#endif

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            screen_width = Screen.width;
            screen_height = Screen.height;
            screen_mode = Screen.fullScreenMode;
            HADEventManager.AddListener((HADEventListener<HADGameEvent>)this);
            HADEventManager.AddListener((HADEventListener<HADEngineEvent>)this);
            HADEngineEvent.Trigger(HADEngineEventTypes.Setting_Frame);
            HADEngineEvent.Trigger(HADEngineEventTypes.Setting_Screen);
        }

        /// <summary>
        /// [����� ����] ����ȭ - ������ ����(���� �������� �ʿ����� �ʴ� ��� ������ �� ���߱�)
        /// </summary>
        /// <param name="_isdown">������ �� ���߱�</param>
        void Setting_Frame(bool _isdown)
        {
            Application.targetFrameRate = targetFrameRate;   //�⺻ fps
            int _interval = 1;  // targetFrameRate / 1 = �⺻ fps
            if (_isdown)
                _interval = 2;  //���� fps
            OnDemandRendering.renderFrameInterval = _interval;  // Application.targetFrameRate / _interval fps
        }

        void Setting_OrthographicSize()
        {
            Camera.main.orthographicSize = Screen.width / 24f / 10;
            //Camera.main.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = Screen.height / 200f;
        }
        public void Input_Setting_Screen(FullScreenMode isFullScreen)
        {
            screen_mode = isFullScreen;
            HADEngineEvent.Trigger(HADEngineEventTypes.Setting_Screen);
        }
        void Setting_Screen()
        {
            Screen.SetResolution(screen_width, screen_height, screen_mode);
            Setting_OrthographicSize();

            //�ػ󵵿� ���� �巡�� Ŭ�� ������ �ΰ��� ����
            eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
        }

        /// <summary>
        /// ���� �Ͻ�����
        /// </summary>
        public void IsInputPause()
        {
            HADEventManager.TriggerEvent(pause_event);
        }
        public virtual void Pause()
        {
#if (UNITY_ANDROID || UNITY_IPHONE)
            isFrameDown = true;
            HADEngineEvent.Trigger(HADEngineEventTypes.Setting_Frame);
#endif
            Debug.Log("�Ͻ�����");
            Time.timeScale = 0f;
        }
        /// <summary>
        /// ���� �Ͻ����� ����
        /// </summary>
        public virtual void UnPause()
        {
#if (UNITY_ANDROID || UNITY_IPHONE)
            isFrameDown = false;
            HADEngineEvent.Trigger(HADEngineEventTypes.Setting_Frame);
#endif
            Debug.Log("�Ͻ����� ����");
            Time.timeScale = 1;
        }

        public void UnSelectableObject()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// HADGameEvent�� �����Ͽ� ��ó
        /// </summary>
        /// <param name="eventType">HADGameEvent event.</param>
        public virtual void OnHADEvent(HADGameEvent eventType)
        {
            switch (eventType.event_Name)
            {
                case "Input_Pause":
                    if (Paused)
                    {
                        Paused = false;
                        HADEngineEvent.Trigger(HADEngineEventTypes.UnPause);
                    }
                    else
                    {
                        Paused = true;
                        HADEngineEvent.Trigger(HADEngineEventTypes.Pause);
                    }
                    break;
            }
        }
        /// <summary>
        /// HADEngineEvent �����Ͽ� ��ó
        /// </summary>
        /// <param name="eventType">HADEngineEvent event.</param>
        public virtual void OnHADEvent(HADEngineEvent eventType)
        {
            switch (eventType.event_type)
            {
                case HADEngineEventTypes.Setting_Frame:
#if (UNITY_ANDROID || UNITY_IPHONE)
                    Setting_Frame(isFrameDown);
#endif
                    break;
                case HADEngineEventTypes.Setting_Screen:
                    Setting_Screen();
                    break;
                case HADEngineEventTypes.Setting_OrthographicSize:
                    Setting_OrthographicSize();
                    break;
                case HADEngineEventTypes.Pause:
                    Pause();
                    break;
                case HADEngineEventTypes.UnPause:
                    UnPause();
                    break;
            }
        }
    }
}

