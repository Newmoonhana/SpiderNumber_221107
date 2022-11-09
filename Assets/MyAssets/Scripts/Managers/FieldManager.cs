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

        [Header("�ΰ��� ������Ʈ")]
        [SerializeField] Transform nodeLine_parent;
        [SerializeField] GameObject line_pre;
        [SerializeField] GameObject node_pre;

        [Header("�ΰ��� ����")]
        [SerializeField] uint width, height;
        uint width_min = 2, width_max = 10;
        float wsize, hsize; //���� ������ ���� ����, ����� ���� ����(������ ���� ���̴� ����)
        [SerializeField] List<NumberLine> line_lst = new List<NumberLine>();
        [SerializeField] List<NumberNode> node_lst = new List<NumberNode>();
        int i;

        protected override void Awake()
        {
            Init();
            base.Awake();
        }

        protected virtual void Start()
        {
            HADEventManager.AddListener(this);
            
            SettingField();
        }

        void Init()
        {
            //���� ����
            int temp_childCount = nodeLine_parent.childCount;
            for (i = 0; i < width_max; i++)
            {
                GameObject line_obj;
                if (i < temp_childCount)
                    line_obj = nodeLine_parent.GetChild(i).gameObject;
                else
                    line_obj = Instantiate(line_pre, nodeLine_parent);
                line_obj.name = "NumberLine" + i;
                line_lst.Add(line_obj.GetComponent<NumberLine>());
            }
                
        }

        void SettingField()
        {
            //���� ����
            for (i = 0; i < width_max; i++)
            {
                if (i < width)
                {
                    NumberLine line_tmp = line_lst[i];
                    LineSetting();
                    NodeSetting();

                    void LineSetting()
                    {
                        Vector2 line_pos = Vector2.zero;
                        // ���� ���
                        wsize = width <= 4 ? 1.25f : 1000 / width * 0.005f; //width�� 4 ������ �� ������ �⺻ ������ = 1.25f, �� �̻� �� ������ ���
                        hsize = 1.25f * 5;
                        //������ �� ��� �� ������Ʈ ����
                        line_pos.x = (-width * 0.5f + i + 0.5f) * wsize;
                        GameObject line_obj = line_tmp.gameObject;
                        line_obj.SetActive(true);
                        line_obj.transform.position = line_pos;

                        //ũ�� ����
                        Vector2 temp_size = line_tmp.sr.size;
                        temp_size.x = wsize;
                        line_tmp.sr.size = temp_size;
                        line_tmp.bg_sr.size = temp_size;
                    }

                    void NodeSetting()
                    {

                    }
                }
                else
                {
                    nodeLine_parent.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public bool NodeMoving(Point prevpos)
        {
            Point movepos = Point.Zero;
            movepos.position = prevpos.position;
            movepos.PositionArray[0] -= prevpos.PositionArray[0];
            if (node_lst[movepos.position] != null)
                return false;

            return true;
        }
        public bool NodeDroping(Point prevpos)
        {
            Point movepos = Point.Zero;
            movepos.position = prevpos.position;
            movepos.PositionArray[1] -= 1;
            if (node_lst[movepos.position] != null)
                return false;

            node_lst[movepos.position] = node_lst[prevpos.position];
            node_lst[prevpos.position] = null;
            return true;
        }

        /// <summary>
        /// HADGameEvent�� �����Ͽ� ��ó
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