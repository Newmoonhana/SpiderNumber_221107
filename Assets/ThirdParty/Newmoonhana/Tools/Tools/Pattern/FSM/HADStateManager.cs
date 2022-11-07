using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Newmoonhana.Tools
{
    public interface IState //state interface
    {
        void UpdateState(float elapsedTime);    //실행 중
        void Render();  //렌더 출력(유니티 특성 상 update에 통합되는 기능이 많아 사용 가능성은 좀 적을듯?)
        void OnEnter(object p_data);    //시작
        void OnExit();  //종료
    }

    [System.Serializable]
    public class EmptyState : IState //empty에는 함수 안에 기능을 넣지 않음
    {
        public virtual void UpdateState(float elapsedTime) { }
        public virtual void Render() { }
        public virtual void OnEnter(object p_data) { }
        public virtual void OnExit() { }
    }

    public class HADStateMachine
    {
        Dictionary<string, IState> states = new Dictionary<string, IState>();
        IState currentState = new EmptyState();
        public IState current
        {
            get
            {
                if (currentState != null)
                    return currentState;
                else
                    return null;
            }
        }
        public string currentToString;

        public void UpdateState(float elapsedTime)  //실행 중(지속 호출)
        {
            currentState.UpdateState(elapsedTime);
        }

        public void Render()    //렌더(지속 호출)
        {
            currentState.Render();
        }

        public void Change(string stateName, object p_data) //state 변경(enter와 exit를 처리하므로 변수를 직접 바꾸지 않고 함수 호출)
        {
            currentState.OnExit();
            currentState = states[stateName];
            currentToString = stateName;
            currentState.OnEnter(p_data);
        }

        public void Add(string name, IState state)  //state 추가(보통 awake start에서 함)
        {
            states[name] = state;
        }
    }

    public class HADStateStackMachine
    {
        Dictionary<string, IState> states = new Dictionary<string, IState>();
        Stack<IState> stack = new Stack<IState>();
        public IState current
        {
            get
            {
                if (stack.Count > 0)
                    return stack.Peek();
                else
                    return null;
            }
        }

        public void Update(float elapsedTime)  //실행 중(지속 호출)
        {
            IState top = stack.Peek();
            top.UpdateState(elapsedTime);
        }

        public void Render()    //렌더(지속 호출)
        {
            IState top = stack.Peek();
            top.Render();
        }

        public IState Push(string name) //state 변경(enter와 exit를 처리하므로 변수를 직접 바꾸지 않고 함수 호출)
        {
            if (stack.Count > 0)
            {
                IState top = stack.Peek();
                top.OnExit();
            }
            
            IState state = states[name];
            stack.Push(state);
            state.OnEnter(null);

            return state;
        }

        public IState Pop()  //state 추가(보통 awake start에서 함)
        {
            if (stack.Count <= 1)
            {
                return null;
            }

            IState pop = stack.Pop();
            pop.OnExit();

            IState top = stack.Peek();
            top.OnEnter(null);

            return pop;
        }

        public void Add(string p_name, IState p_state) //state 추가(보통 awake start에서 함)
        {
            if (states.ContainsKey(p_name)) //키 값이 이미 존재 시 value 변경
            {
                states[p_name] = p_state;
            }
            else    //없으면 추가
            {
                states.Add(p_name, p_state);
            }
        }

        public void DebugStack()
        {
            string str = "[Debug Stack]";

            int i = 0;
            foreach (IState item in stack)
            {
                str = str + "\n" + i++ + "= " + item.ToString();
            }

            Debug.Log(str);
        }
    }
}