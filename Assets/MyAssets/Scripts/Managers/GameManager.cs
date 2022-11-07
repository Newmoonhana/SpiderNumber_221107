using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newmoonhana.Tools;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace Newmoonhana.HADEngine
{
    /// <summary>
    /// 가능한 기본 이벤트 목록
    /// LevelStart : 레벨 시작 시 LevelManager에 의해 트리거
    ///	LevelComplete : 레벨 클리어 시 트리거
    /// LevelEnd : (*동일)
    ///	Pause : 일시정지 시작 시 트리거
    ///	UnPause : 일시정지 중단 이후 정상으로 돌아갈 시 트리거
    ///	PlayerDeath : 플레이어 캐릭터 사망 시 트리거
    ///	RespawnStarted : 플레이어 캐릭터 스왑 시 트리거
    ///	RespawnComplete : 플레이어 캐릭터 스왑 종료 시 트리거
    ///	GameOver : 모든 생명을 잃었을 때 LevelManager에 의해 트리거
    /// Repaint : UI 리프레시 요청 시 트리거
    /// TogglePause : 일시중지(또는 해제)를 위한 요청 시 트리거
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
	/// 시작 및 종료 신호를 보내는 데 사용되는 이벤트 유형(현재)
	/// </summary>
	public struct HADEngineEvent
    {
        public HADEngineEventTypes event_type;
        //public Character OriginCharacter;   //원본에선 엔터더건전 식 탑뷰라 캐릭터 함수로 연결되있는데 수정 들어가야할거임. 수정할때 주석 해제 및 코드 변경하면 됨

        /// <summary>
		/// 생성자. <see cref="Newmoonhana.HADEngine.HADEngineEvent"/> struct의 새 인스턴트를 초기화
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
	/// GameManager는 변수와 시간을 처리하는 지속적인 싱글톤(PersistentSingleton)
	/// </summary>
	[AddComponentMenu("HAD Engine/Managers/Game Manager")]
    public class GameManager : HADPersistentSingleton<GameManager>, HADEventListener<HADGameEvent>, HADEventListener<HADEngineEvent>
    {
        [Tooltip("게임의 프레임률")]
        public int targetFrameRate = 300;

        [Header("게임 오버 씬")]
        [Tooltip("게임 오버 시 이동될 씬")]
        public string gameoverScene;

        /// 게임이 일시정지 될 시 true
        public bool Paused { get; set; }

        //메뉴창
        protected bool _menuOpen = false;

        //해상도
        public EventSystem eventSystem;
        int screen_width, screen_height;
        public int ScreenW { set { screen_width = value; } }
        public int ScreenH { set { screen_height = value; } }
        FullScreenMode screen_mode;
        const float inchToCm = 2.54f;   //인치->센치미터
        readonly float dragThresholdCM = 0.5f;  //터치한 시점으로부터 변수 값 정도는 밀려도 눌린 상태 유지

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
        /// [모바일 한정] 최적화 - 프레임 제약(높은 프레임이 필요하지 않는 경우 프레임 수 낮추기)
        /// </summary>
        /// <param name="_isdown">프레임 수 낮추기</param>
        void Setting_Frame(bool _isdown)
        {
            Application.targetFrameRate = targetFrameRate;   //기본 fps
            int _interval = 1;  // targetFrameRate / 1 = 기본 fps
            if (_isdown)
                _interval = 2;  //낮출 fps
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

            //해상도에 따른 드래그 클릭 구별할 민감도 조절
            eventSystem.pixelDragThreshold = (int)(dragThresholdCM * Screen.dpi / inchToCm);
        }

        /// <summary>
        /// 게임 일시정지
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
            Debug.Log("일시정지");
            Time.timeScale = 0f;
        }
        /// <summary>
        /// 게임 일시정지 해제
        /// </summary>
        public virtual void UnPause()
        {
#if (UNITY_ANDROID || UNITY_IPHONE)
            isFrameDown = false;
            HADEngineEvent.Trigger(HADEngineEventTypes.Setting_Frame);
#endif
            Debug.Log("일시정지 해제");
            Time.timeScale = 1;
        }

        public void UnSelectableObject()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// HADGameEvent를 포착하여 대처
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
        /// HADEngineEvent 포착하여 대처
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

