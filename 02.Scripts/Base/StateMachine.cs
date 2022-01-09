using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void OperateEnter();
    public void OperateUpdate();
    public void OperateExit();
}

public class StateMachine
{
    public IState CurrentState { get; private set; }

    //default ���¸� ������ ����
    public StateMachine(IState defaultState)
    {
        CurrentState = defaultState;
        CurrentState.OperateEnter();
    }

    public void SetState(IState state)
    {
        if(CurrentState == state)
        {
            Debug.Log("���� �̹� �ش� �����Դϴ�.");
            return;
        }

        CurrentState.OperateExit();

        CurrentState = state;
        CurrentState.OperateEnter();
    }
    public void DoOperateUpdate()
    {
        CurrentState.OperateUpdate();
    }
}
