//#define EVENTROUTER_THROWEXCEPTIONS 
#if EVENTROUTER_THROWEXCEPTIONS
    #define EVENTROUTER_REQUIRELISTENER // listeners�� �̺�Ʈ ������ �ϱ� ���Ѵٸ� �ּ��� ���켼��(�����). ��� ���Ҷ��� �ּ� ó��.
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Newmoonhana.Tools
{
    /// <summary>
	/// HADGameEvents�� �̺�Ʈ�� �Ϲ� ���� �̺�Ʈ�� ���� ���� ���ݿ� ���� ���(���� ����, ���� ����, ������ �ν�Ʈ ��)
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
    /// �� Ŭ������ �̺�Ʈ ������ ó���ϸ�, �� Ŭ����(�Ǵ� ���� Ŭ����)�� ������ �߻������� �˸��� ���� ���� ��ü�� �̺�Ʈ�� ��ε�ĳ��Ʈ�ϴ� �� ����� �� ����
    /// �̺�Ʈ�� struct�̹Ƿ� ���ϴ� ��� ������ �̺�Ʈ�� ������ �� �ֽ��ϴ�. �� Manager���� �⺻������ ���ڿ��θ� ������ HADGameEvents�� �Բ� ����������,
    /// ���ϴ� ��� �� ������ �̺�Ʈ�� �۾��� �� ����
    /// 
    /// ��𼭵� �� �̺�Ʈ�� Ʈ�����Ϸ���, YOUR_EVENT.Trigger(YOUR_PARAMETERS) �� �Ѵ�.(YOUR_EVENT & YOUR_PARAMETERS�� ���� ��)
    /// ��) HADGameEvent.Trigger("Save"); -> Save HADGameEvent�� Ʈ����
    /// 
    /// HADEventManager�� ȣ���� ���� ����. TriggerEvent(YOUR_EVENT);
    /// ��) HADEventManager.TriggerEvent(new HADGameEvent("GameStart")); -> GameStart��� �̸��� HAD Game Event�� ��� listener���� ��ε�ĳ��Ʈ
    ///
    /// � Ŭ���������� �̺�Ʈ ��⸦ �����Ϸ��� ���� �� ���� �۾��� �����ؾ��� : 
    ///
    /// 1 - Ŭ������ HADEventListener interface�� ����
    /// ��) public class GUIManager : Singleton<GUIManager>, HADEventListener<HADGameEvent>
    /// �� �� �ϳ� �̻��� ���� �� ����(�̺�Ʈ ������ �ϳ���)
    ///
    /// 2 - On Enable and Disable ���� �̺�Ʈ ������ ���� �����ϰ� ����
    /// void OnEnable()
    /// {
    /// 	this.HADEventStartListening<HADGameEvent>();
    /// }
    /// void OnDisable()
    /// {
    /// 	this.HADEventStopListening<HADGameEvent>();
    /// }
    /// 
    /// 3 - �ش� �̺�Ʈ�� ���� HADEventListener interface�� �����ؾ���. ��)
    /// public void OnHADEvent(HADGameEvent gameEvent)
    /// {
    /// 	if (gameEvent.EventName == "GameOver")
    ///		{
    ///			// DO SOMETHING
    ///		}
    /// } 
    /// ���� �� ��𿡼��� ����Ǵ� HADGameEvent ������ ��� �̺�Ʈ�� �����ϰ� GameOver��� �̸��� ������ �۾��� ����
    /// </summary>
    [ExecuteAlways]
    public static class HADEventManager
    {
        private static Dictionary<Type, List<HADEventListenerBase>> _subscribersList;   //��� ����Ʈ

        static HADEventManager()    //������
        {
            _subscribersList = new Dictionary<Type, List<HADEventListenerBase>>();
        }

        /// <summary>
        /// Ư�� �̺�Ʈ�� �� _subscribersList�� ���
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="HADEvent">event type< T >.</typeparam>
        public static void AddListener<HADEvent>(HADEventListener<HADEvent> listener) where HADEvent : struct
        {
            Type eventType = typeof(HADEvent);

            if (!_subscribersList.ContainsKey(eventType))   //Key ������
                _subscribersList[eventType] = new List<HADEventListenerBase>();  //<T>���� ���ο� Ű�� ���

            if (!SubscriptionExists(eventType, listener))   //Value ������
                _subscribersList[eventType].Add(listener);  //<T>���� Ű�� listener�� ���
        }

        /// <summary>
	    /// Ư�� �̺�Ʈ�� _subscribersList �׸��� ����
	    /// </summary>
		/// <param name="listener">listener</param>
	    /// <typeparam name="HADEvent">event type< T ></typeparam>
	    public static void RemoveListener<HADEvent>(HADEventListener<HADEvent> listener) where HADEvent : struct
        {
            Type eventType = typeof(HADEvent);

            if (!_subscribersList.ContainsKey(eventType))   //Key ������
            {
#if EVENTROUTER_THROWEXCEPTIONS
					throw new ArgumentException( string.Format( "Removing listener \"{0}\", event type \"{1}\"�� ��ϵ��� ���� �׸�", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<HADEventListenerBase> subscriberList = _subscribersList[eventType];

#if EVENTROUTER_THROWEXCEPTIONS
	            bool listenerFound = false; //for������ ����� �α� ����� ���� üŷ ����
#endif

            for (int i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] == listener)  //������ �׸� ã�� ��
                {
                    subscriberList.Remove(subscriberList[i]);   //����
#if EVENTROUTER_THROWEXCEPTIONS
					    listenerFound = true;
#endif

                    if (subscriberList.Count == 0)  //���� ���� �׸��� �ϳ��� ���� ��
                    {
                        _subscribersList.Remove(eventType); //����Ʈ ����
                    }

                    return;
                }
            }

#if EVENTROUTER_THROWEXCEPTIONS //������ �׸� ã�� ����
		        if( !listenerFound )
		        {
					throw new ArgumentException( string.Format( "Removing listener�� �۵������� �ش� receiver�� event type \"{0}\"�� ����.", eventType.ToString() ) );
		        }
#endif
        }

        /// <summary>
	    /// �̺�Ʈ�� Ʈ����. ��ϵ� ��� �ν�źƮ�� ����, (����������) �����.
	    /// </summary>
		/// <param name="newEvent">Ʈ������ �̺�Ʈ</param>
	    /// <typeparam name="HADEvent">The 1st type parameter.</typeparam>
	    public static void TriggerEvent<HADEvent>(HADEvent newEvent) where HADEvent : struct
        {
            List<HADEventListenerBase> list;
            if (!_subscribersList.TryGetValue(typeof(HADEvent), out list))   //Key�� �����ϴ� ���� ���ٸ� ����� �α� ���(receivers �� ����)
#if EVENTROUTER_REQUIRELISTENER
			            throw new ArgumentException( string.Format( "event type \"{0}\"�� ���ŵǾ�����, listener���� �� type�� ã�� �� ����.\nȮ��:Subscribe<{0}>(EventRouter)�� ���� X or �� �̺�Ʈ�� listeners�� ���", typeof( HADEvent ).ToString() ) );
#else
                return;
#endif
            
            for (int i = 0; i < list.Count; i++)    //����Ʈ�� ��� �̺�Ʈ ����
            {
                (list[i] as HADEventListener<HADEvent>).OnHADEvent(newEvent);
            }
        }

        /// <summary>
	    /// Ư�� �̺�Ʈ ������ ���� ������ �ִ��� Ȯ��
	    /// </summary>
	    /// <returns>���� �����Ѵٸ� <c>true</c>, �׷��� ������ <c>false</c>.</returns>
	    /// <param name="type">Type.</param>
	    /// <param name="receiver">Receiver.</param>
	    private static bool SubscriptionExists(Type type, HADEventListenerBase receiver)
        {
            List<HADEventListenerBase> receivers;

            if (!_subscribersList.TryGetValue(type, out receivers)) return false;   //Key�� �����ϴ� ���� ���ٸ� false ����(receivers �� ����)

            bool exists = false;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (receivers[i] == receiver)   //ã�� ���� ������ ���
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
    }

    /// <summary>
	/// (Event listener) �⺻ interface
	/// </summary>
	public interface HADEventListenerBase { };

    /// <summary>
    /// (Event listener) �� ������ ���� �����ؾ��ϴ� public interface
    /// </summary>
    public interface HADEventListener<T> : HADEventListenerBase
    {
        void OnHADEvent(T eventType);
    }
}
