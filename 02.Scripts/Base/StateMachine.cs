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

    //default 상태를 생성시 설정
    public StateMachine(IState defaultState)
    {
        CurrentState = defaultState;
        CurrentState.OperateEnter();
    }

    public void SetState(IState state)
    {
        if(CurrentState == state)
        {
            Debug.Log("현재 이미 해당 상태입니다.");
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
