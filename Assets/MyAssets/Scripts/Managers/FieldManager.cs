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

        [Header("인게임 오브젝트")]
        [SerializeField] Transform nodeLine_parent;
        [SerializeField] GameObject line_pre;
        [SerializeField] GameObject node_pre;

        [Header("인게임 변수")]
        [SerializeField] uint row;
        [SerializeField] uint column;
        uint row_min = 2, row_max = 10, column_min = 2, column_max = 10;
        float width, height; //노드와 라인의 가로 길이, 노드의 세로 길이(라인의 세로 길이는 고정)
        [SerializeField] List<NumberLine> line_lst = new List<NumberLine>();
        [SerializeField] List<NumberNode> node_lst = new List<NumberNode>();
        int i, j;

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
            //라인 세팅
            int temp_childCount = nodeLine_parent.childCount;
            for (i = 0; i < row_max; i++)
            {
                //라인 생성 or 등록
                GameObject line_obj;
                if (i < temp_childCount)
                    line_obj = nodeLine_parent.GetChild(i).gameObject;
                else
                    line_obj = Instantiate(line_pre, nodeLine_parent);
                Transform line_tns = line_obj.transform;
                line_obj.name = "NumberLine" + i;
                line_lst.Add(line_obj.GetComponent<NumberLine>());

                //노드 생성 or 등록
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

        float size = 1.25f;
        void SettingField()
        {
            // 넓이 계산
            width = row <= 4 ? size : 1000 / row * 0.005f; //row가 4 이하일 시 적용할 기본 사이즈 = 1.25f, 그 이상 시 비율로 계산
            height = (size * 5 - 0.25f) / column;
            //라인 세팅
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
                        //포지션 값 계산
                        Vector2 line_pos = Vector2.zero;
                        float linepos_cal = (-row * 0.5f + i + 0.5f) * width;
                        line_pos.x = linepos_cal;
                        line_tns.position = line_pos;
                        //크기 조정
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
                                //포지션 값 계산
                                Vector2 node_pos = Vector2.zero;
                                float nodepos_cal = (-column * 0.5f + j + 0.5f) * height;
                                node_pos.y = nodepos_cal;
                                node_tns.localPosition = node_pos;
                                //크기 조정
                                Vector2 temp_size = line_tmp.sr.size;
                                temp_size.x = width - 0.25f;
                                temp_size.y = height;
                                node_tmp.ChangeSize(temp_size);
                            }
                            else
                            {
                                node_obj.SetActive(false);
                                Debug.Log(node_obj.name);
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