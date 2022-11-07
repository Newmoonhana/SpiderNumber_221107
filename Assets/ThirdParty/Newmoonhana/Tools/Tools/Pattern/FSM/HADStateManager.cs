using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Newmoonhana.Tools
{
    public interface IState //state interface
    {
        void UpdateState(float elapsedTime);    //���� ��
        void Render();  //���� ���(����Ƽ Ư�� �� update�� ���յǴ� ����� ���� ��� ���ɼ��� �� ������?)
        void OnEnter(object p_data);    //����
        void OnExit();  //����
    }

    [System.Serializable]
    public class EmptyState : IState //empty���� �Լ� �ȿ� ����� ���� ����
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

        public void UpdateState(float elapsedTime)  //���� ��(���� ȣ��)
        {
            currentState.UpdateState(elapsedTime);
        }

        public void Render()    //����(���� ȣ��)
        {
            currentState.Render();
        }

        public void Change(string stateName, object p_data) //state ����(enter�� exit�� ó���ϹǷ� ������ ���� �ٲ��� �ʰ� �Լ� ȣ��)
        {
            currentState.OnExit();
            currentState = states[stateName];
            currentToString = stateName;
            currentState.OnEnter(p_data);
        }

        public void Add(string name, IState state)  //state �߰�(���� awake start���� ��)
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

        public void Update(float elapsedTime)  //���� ��(���� ȣ��)
        {
            IState top = stack.Peek();
            top.UpdateState(elapsedTime);
        }

        public void Render()    //����(���� ȣ��)
        {
            IState top = stack.Peek();
            top.Render();
        }

        public IState Push(string name) //state ����(enter�� exit�� ó���ϹǷ� ������ ���� �ٲ��� �ʰ� �Լ� ȣ��)
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

        public IState Pop()  //state �߰�(���� awake start���� ��)
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

        public void Add(string p_name, IState p_state) //state �߰�(���� awake start���� ��)
        {
            if (states.ContainsKey(p_name)) //Ű ���� �̹� ���� �� value ����
            {
                states[p_name] = p_state;
            }
            else    //������ �߰�
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