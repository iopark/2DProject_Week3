using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class StateBase<TOwner> where TOwner : MonoBehaviour
{
    protected TOwner owner; // 어떤 component가 state를 지니고 있는지 포함하게 된다면, 이 State를 지닌 객체에 대해서 접근, 상호작용이 가능하여진다. 
    public StateBase(TOwner owner)
    {
        this.owner = owner;
    }
    // 이렇게 어떠한 State에 대해서 꼭 지니게될 상태들에대해서 원활한 캡슐화 작업을 위해서, 추상화 작업을 선진행해줄수도 있겠다.
    public abstract void SetUp(); 
    public abstract void Enter(); 
    public abstract void Update();
    public abstract void Exit(); 
}
