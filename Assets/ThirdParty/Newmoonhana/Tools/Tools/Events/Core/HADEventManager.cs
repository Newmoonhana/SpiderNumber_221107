//#define EVENTROUTER_THROWEXCEPTIONS 
#if EVENTROUTER_THROWEXCEPTIONS
    #define EVENTROUTER_REQUIRELISTENER // listeners가 이벤트 전송을 하길 원한다면 주석을 지우세요(디버깅). 사용 안할때는 주석 처리.
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Newmoonhana.Tools
{
    /// <summary>
	/// HADGameEvents는 이벤트는 일반 게임 이벤트에 대해 게임 전반에 걸쳐 사용(게임 시작, 게임 종료, 라이프 로스트 등)
	/// </summary>
	public struct HADGameEvent
    {
        public string event_Name;
        public HADGameEvent(string newName)
        {
            event_Name = newName;
        }
        static HADGameEvent e;
        public static void Trigger(string newName)
        {
            e.event_Name = newName;
            HADEventManager.TriggerEvent(e);
        }
    }

    /// <summary>
    /// 이 클래스는 이벤트 관리를 처리하며, 한 클래스(또는 여러 클래스)에 문제가 발생했음을 알리기 위해 게임 전체에 이벤트를 브로드캐스트하는 데 사용할 수 있음
    /// 이벤트는 struct이므로 원하는 모든 종류의 이벤트를 정의할 수 있습니다. 이 Manager에는 기본적으로 문자열로만 구성된 HADGameEvents가 함께 제공되지만,
    /// 원하는 경우 더 복잡한 이벤트로 작업할 수 있음
    /// 
    /// 어디서든 새 이벤트를 트리거하려면, YOUR_EVENT.Trigger(YOUR_PARAMETERS) 라 한다.(YOUR_EVENT & YOUR_PARAMETERS는 변수 값)
    /// 예) HADGameEvent.Trigger("Save"); -> Save HADGameEvent를 트리거
    /// 
    /// HADEventManager를 호출할 수도 있음. TriggerEvent(YOUR_EVENT);
    /// 예) HADEventManager.TriggerEvent(new HADGameEvent("GameStart")); -> GameStart라는 이름의 HAD Game Event를 모든 listener에게 브로드캐스트
    ///
    /// 어떤 클래스에서든 이벤트 듣기를 시작하려면 다음 세 가지 작업을 수행해야함 : 
    ///
    /// 1 - 클래스가 HADEventListener interface를 선언
    /// 예) public class GUIManager : Singleton<GUIManager>, HADEventListener<HADGameEvent>
    /// 이 중 하나 이상을 가질 수 있음(이벤트 유형당 하나씩)
    ///
    /// 2 - On Enable and Disable 에서 이벤트 수신을 각각 시작하고 중지
    /// void OnEnable()
    /// {
    /// 	this.HADEventStartListening<HADGameEvent>();
    /// }
    /// void OnDisable()
    /// {
    /// 	this.HADEventStopListening<HADGameEvent>();
    /// }
    /// 
    /// 3 - 해당 이벤트에 대해 HADEventListener interface를 구현해야함. 예)
    /// public void OnHADEvent(HADGameEvent gameEvent)
    /// {
    /// 	if (gameEvent.EventName == "GameOver")
    ///		{
    ///			// DO SOMETHING
    ///		}
    /// } 
    /// 게임 내 어디에서나 방출되는 HADGameEvent 유형의 모든 이벤트를 포착하고 GameOver라는 이름이 있으면 작업을 수행
    /// </summary>
    [ExecuteAlways]
    public static class HADEventManager
    {
        private static Dictionary<Type, List<HADEventListenerBase>> _subscribersList;   //등록 리스트

        static HADEventManager()    //생성자
        {
            _subscribersList = new Dictionary<Type, List<HADEventListenerBase>>();
        }

        /// <summary>
        /// 특정 이벤트로 새 _subscribersList를 등록
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="HADEvent">event type< T >.</typeparam>
        public static void AddListener<HADEvent>(HADEventListener<HADEvent> listener) where HADEvent : struct
        {
            Type eventType = typeof(HADEvent);

            if (!_subscribersList.ContainsKey(eventType))   //Key 비존재
                _subscribersList[eventType] = new List<HADEventListenerBase>();  //<T>값을 새로운 키로 등록

            if (!SubscriptionExists(eventType, listener))   //Value 비존재
                _subscribersList[eventType].Add(listener);  //<T>값을 키로 listener를 등록
        }

        /// <summary>
	    /// 특정 이벤트로 _subscribersList 항목을 제거
	    /// </summary>
		/// <param name="listener">listener</param>
	    /// <typeparam name="HADEvent">event type< T ></typeparam>
	    public static void RemoveListener<HADEvent>(HADEventListener<HADEvent> listener) where HADEvent : struct
        {
            Type eventType = typeof(HADEvent);

            if (!_subscribersList.ContainsKey(eventType))   //Key 비존재
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", event type \"{1}\"는 등록되지 않은 항목", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<HADEventListenerBase> subscriberList = _subscribersList[eventType];

#if EVENTROUTER_THROWEXCEPTIONS
	            bool listenerFound = false; //for문에서 디버깅 로그 출력을 위한 체킹 변수
#endif

            for (int i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] == listener)  //제거할 항목 찾을 시
                {
                    subscriberList.Remove(subscriberList[i]);   //제거
#if EVENTROUTER_THROWEXCEPTIONS
					    listenerFound = true;
#endif

                    if (subscriberList.Count == 0)  //제거 이후 항목이 하나도 없을 시
                    {
                        _subscribersList.Remove(eventType); //리스트 제거
                    }

                    return;
                }
            }

#if EVENTROUTER_THROWEXCEPTIONS //제거할 항목 찾기 실패
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener는 작동했으나 해당 receiver에 event type \"{0}\"가 없다.", eventType.ToString() ) );
		        }
#endif
        }

        /// <summary>
	    /// 이벤트를 트리거. 등록된 모든 인스탄트가 수신, (잠재적으로) 실행됨.
	    /// </summary>
		/// <param name="newEvent">트리거할 이벤트</param>
	    /// <typeparam name="HADEvent">The 1st type parameter.</typeparam>
	    public static void TriggerEvent<HADEvent>(HADEvent newEvent) where HADEvent : struct
        {
            List<HADEventListenerBase> list;
            if (!_subscribersList.TryGetValue(typeof(HADEvent), out list))   //Key에 대응하는 값이 없다면 디버그 로그 출력(receivers 값 저장)
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "event type \"{0}\"가 수신되었지만, listener에서 이 type를 찾을 수 없다.\n확인:Subscribe<{0}>(EventRouter)가 선언 X or 이 이벤트의 listeners가 취소", typeof( HADEvent ).ToString() ) );
#else
                return;
#endif
            
            for (int i = 0; i < list.Count; i++)    //리스트의 모든 이벤트 실행
            {
                (list[i] as HADEventListener<HADEvent>).OnHADEvent(newEvent);
            }
        }

        /// <summary>
	    /// 특정 이벤트 유형에 대한 변수가 있는지 확인
	    /// </summary>
	    /// <returns>만약 존재한다면 <c>true</c>, 그렇지 않으면 <c>false</c>.</returns>
	    /// <param name="type">Type.</param>
	    /// <param name="receiver">Receiver.</param>
	    private static bool SubscriptionExists(Type type, HADEventListenerBase receiver)
        {
            List<HADEventListenerBase> receivers;

            if (!_subscribersList.TryGetValue(type, out receivers)) return false;   //Key에 대응하는 값이 없다면 false 리턴(receivers 값 저장)

            bool exists = false;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (receivers[i] == receiver)   //찾는 값이 존재할 경우
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
    }

    /// <summary>
	/// (Event listener) 기본 interface
	/// </summary>
	public interface HADEventListenerBase { };

    /// <summary>
    /// (Event listener) 각 유형에 대해 구현해야하는 public interface
    /// </summary>
    public interface HADEventListener<T> : HADEventListenerBase
    {
        void OnHADEvent(T eventType);
    }
}
