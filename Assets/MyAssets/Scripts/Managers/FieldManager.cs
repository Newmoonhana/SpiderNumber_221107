using Newmoonhana.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Newmoonhana.HADEngine
{
    public struct HADNodeEvent
    {
        public string event_Name;
        public Point event_Position;
        public Point event_SubPosition;
        public HADNodeEvent(string newName)
        {
            event_Name = newName;
            event_Position = new Point();
            event_SubPosition = new Point();
        }
        static HADNodeEvent e;
        public static void Trigger(string newName)
        {
            e.event_Name = newName;
            HADEventManager.TriggerEvent(e);
        }
    }

    public class FieldManager : HADSingleton<FieldManager>, HADEventListener<HADNodeEvent>, IState
    {
        HADNodeEvent nodeMoving_event;
        HADNodeEvent nodeDroping_event;

        [Header("�ΰ��� ������Ʈ")]
        [SerializeField] Transform nodeLine_parent;
        [SerializeField] GameObject line_pre;
        [SerializeField] GameObject node_pre;

        [Header("�ΰ��� ����")]
        [SerializeField] uint row;
        [SerializeField] uint column;
        uint row_min = 2, row_max = 10, column_min = 2, column_max = 10;
        float width, height; //���� ������ ���� ����, ����� ���� ����(������ ���� ���̴� ����)
        [SerializeField] List<NumberLine> line_lst;
        [SerializeField] List<NumberNode> node_lst;
        int i, j;

        protected override void Awake()
        {
            HADInputEventManager.Init();
            nodeMoving_event = new HADNodeEvent("Node Moving");
            nodeDroping_event = new HADNodeEvent("Node Droping");
            line_lst = new List<NumberLine>();
            node_lst = new List<NumberNode>();

            Init();
            base.Awake();
        }

        void Start()
        {
            HADEventManager.AddListener(this);
            
            SettingField();
        }

        void Init()
        {
            //���� ����
            int temp_childCount = nodeLine_parent.childCount;
            for (i = 0; i < row_max; i++)
            {
                //���� ���� or ���
                GameObject line_obj;
                if (i < temp_childCount)
                    line_obj = nodeLine_parent.GetChild(i).gameObject;
                else
                    line_obj = Instantiate(line_pre, nodeLine_parent);
                Transform line_tns = line_obj.transform;
                line_obj.name = "NumberLine" + i;
                line_lst.Add(line_obj.GetComponent<NumberLine>());

                //��� ���� or ���
                int temp_nodeChildCount = line_tns.GetChild(1).childCount;
                for (j = 0; j < column_max; j++)
                {
                    GameObject node_obj;
                    if (j < temp_nodeChildCount)
                        node_obj = line_tns.GetChild(1).GetChild(j).gameObject;
                    else
                        node_obj = Instantiate(node_pre, line_tns.GetChild(1));
                    node_obj.name = "NumberNode" + j;
                    node_lst.Add(node_obj.GetComponent<NumberNode>());
                }
            }
                
        }

        private void OnEnable()
        {
            HADInputEventManager.OnStartTouch += NodeTouchStarted;
            HADInputEventManager.OnDragTouch += NodeDrag;
            HADInputEventManager.OnEndTouch += NodeTouchEnded;
            HADInputEventManager.Enable();
        }
        private void OnDisable()
        {
            HADInputEventManager.OnEndTouch -= NodeTouchStarted;
            HADInputEventManager.OnEndTouch -= NodeDrag;
            HADInputEventManager.OnEndTouch -= NodeTouchEnded;
            HADInputEventManager.Disable();
        }

        float size = 1.25f;
        void SettingField()
        {
            // ���� ���
            width = row <= 4 ? size : 1000 / row * 0.005f; //row�� 4 ������ �� ������ �⺻ ������ = 1.25f, �� �̻� �� ������ ���
            height = (size * 5 - 0.25f) / column;
            //���� ����
            for (i = 0; i < row_max; i++)
            {
                NumberLine line_tmp = line_lst[i];
                GameObject line_obj = line_tmp.gameObject;
                if (i < row)
                {
                    Transform line_tns = line_obj.transform;
                    LineSetting();
                    NodeSetting();

                    void LineSetting()
                    {
                        line_obj.SetActive(true);
                        //������ �� ���
                        Vector2 line_pos = Vector2.zero;
                        float linepos_cal = (-row * 0.5f + i + 0.5f) * width;
                        line_pos.x = linepos_cal;
                        line_tns.position = line_pos;
                        //ũ�� ����
                        Vector2 temp_size = line_tmp.sr.size;
                        temp_size.x = width;
                        line_tmp.ChangeSize(temp_size);
                    }

                    void NodeSetting()
                    {
                        for (j = 0; j < column_max; j++)
                        {
                            NumberNode node_tmp = node_lst[i * (int)column_max + j];
                            GameObject node_obj = node_tmp.gameObject;
                            if (j < column)
                            {
                                node_obj.SetActive(true);
                                Transform node_tns = node_tmp.transform;
                                //������ �� ���
                                Vector2 node_pos = Vector2.zero;
                                float nodepos_cal = (-column * 0.5f + j + 0.5f) * height;
                                node_pos.y = nodepos_cal;
                                node_tns.localPosition = node_pos;
                                //ũ�� ����
                                Vector2 temp_size = line_tmp.sr.size;
                                temp_size.x = width - 0.25f;
                                temp_size.y = height;
                                node_tmp.ChangeSize(temp_size);
                            }
                            else
                            {
                                node_obj.SetActive(false);
                            }
                        }
                    }
                }
                else
                {
                    line_obj.SetActive(false);
                }
            }
        }

        NumberNode hit_node;
        public void NodeTouchStarted(Vector2 _screen_pos, float time)
        {
            RaycastHit2D []hitInfo = Physics2D.RaycastAll(HADInputEventManager.worldCoordinates, Vector2.zero);
            for (i = 0; i < hitInfo.Length; i++)
            {
                if (hitInfo[i].transform != null)
                    if (hitInfo[i].collider.gameObject.layer == LayerMask.NameToLayer("NumberNode"))
                    {
                        //������ ��� ������Ʈ ID �� ��������(��Ȱ�� ������Ʈ�� node_lst�� �������Ƿ� column_max ������ line id�� ������)
                        int id = int.Parse((hitInfo[i].transform.parent.parent.name).Split("NumberLine")[1]) * (int)column_max
                            + int.Parse((hitInfo[i].transform.name).Split("NumberNode")[1]);
                        hit_node = node_lst[id];
                        hit_node.TouchedDropEffect();
                        break;
                    }
            }
        }

        public void NodeDrag(Vector2 _screen_pos, float time)
        {
            if (hit_node != null)
                hit_node.transform.position = HADInputEventManager.worldCoordinates;
        }

        public void NodeTouchEnded(Vector2 _screen_pos, float time)
        {
            if (hit_node != null)
            {
                hit_node.UnTouchedDropEffect();
                RaycastHit2D[] hitInfo = Physics2D.RaycastAll(HADInputEventManager.worldCoordinates, Vector2.zero);
                for (i = 0; i < hitInfo.Length; i++)
                {
                    if (hitInfo[i].transform != null)
                        if (hitInfo[i].collider.gameObject.layer == LayerMask.NameToLayer("NumberLine"))
                        {
                            //������ ���� ������Ʈ ID �� ��������
                            int line_id = int.Parse((hitInfo[i].transform.parent.parent.name).Split("NumberLine")[1]);
                            //������ ���ο� ������ ��尡 �ִ��� Ȯ��
                            int line_lastnode_id = line_id * (int)column_max + (int)column - 1;
                            NumberNode lastnode = node_lst[line_lastnode_id];
                            break;
                        }
                }

                hit_node = null;
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
        public void OnHADEvent(HADNodeEvent eventType)
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