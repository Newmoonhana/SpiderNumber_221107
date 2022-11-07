using Newmoonhana.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Newmoonhana.HADEngine
{
    public struct HADFieldEvent
    {
        public string event_Name;
        public Point event_Position;
        public Point event_SubPosition;
        public HADFieldEvent(string newName)
        {
            event_Name = newName;
            event_Position = new Point();
            event_SubPosition = new Point();
        }
        static HADFieldEvent e;
        public static void Trigger(string newName)
        {
            e.event_Name = newName;
            HADEventManager.TriggerEvent(e);
        }
    }

    public class FieldManager : HADSingleton<FieldManager>, HADEventListener<HADFieldEvent>, IState
    {
        HADFieldEvent nodeMoving_event = new HADFieldEvent("Node Moving");
        HADFieldEvent nodeDroping_event = new HADFieldEvent("Node Droping");

        [Header("인게임 변수")]
        [SerializeField] Dictionary<int, NumberNode> node_lst = new Dictionary<int, NumberNode>();

        protected override void Awake()
        {
            base.Awake();
        }

        protected virtual void Start()
        {
            HADEventManager.AddListener(this);
        }

        public void NodeMoving(Point position)
        {
            
        }
        public bool NodeDroping(Point prevpos)
        {
            Point movepos = prevpos;
            movepos.PositionArray[1] -= 1;
            if (node_lst.ContainsKey(movepos.Position))
                return false;

            node_lst[movepos.Position] = node_lst[prevpos.Position];
            node_lst.Remove(prevpos.Position);
            return true;
        }

        /// <summary>
        /// HADGameEvent를 포착하여 대처
        /// </summary>
        /// <param name="eventType">HADGameEvent event.</param>
        public void OnHADEvent(HADFieldEvent eventType)
        {
            switch (eventType.event_Name)
            {
                case "Node Moving":
                    NodeMoving(eventType.event_Position);
                    break;
                case "Node Droping":
                    NodeDroping(eventType.event_Position);
                    break;
            }
        }

        public void UpdateState(float elapsedTime)
        {

        }

        public void Render()
        {

        }

        public void OnEnter(object p_data)
        {

        }

        public void OnExit()
        {

        }
    }
}