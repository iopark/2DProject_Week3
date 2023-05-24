using batstate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour
{
    [Header("Bat's Current State")]
    [SerializeField] private StateBase<Bat>[] states;
    [SerializeField] public STATE curState;

    [Header("Bat's properties")]
    [SerializeField] public float detectRange;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float attackRange;
    [SerializeField] public float runRange; // 도망칠 타이밍을 선정하기 위한 맴버변수 

    [SerializeField] public Vector3 returnPosition;
    [SerializeField] public Transform player;

    private void Awake()
    {
        states = new StateBase<Bat>[(int)STATE.Size]; // Size 값을 이용해서, 열거형의 값대로의 배열생성응용이 가능하여지겠다.
        states[(int)STATE.Idle] = new IdleState(this);
        states[(int)STATE.Trace] = new TraceState(this);
        states[(int)STATE.Return] = new ReturnState(this);
        states[(int)STATE.Attack] = new AttackState(this);
        states[(int)STATE.Runaway] = new RunawayState(this); // 도망가는 녀석
    }

    private void Update()
    {
        states[(int)curState].Update(); // where 캡슐화된 객체들끼리 알아서 현재 상태에 대해서 evaluate 해주기 때문에, frame 별 업데이트할때에는 그저 현재상태에 따른 update function만 진행해주면 되겠다. 

    }

    private void Start()
    {
        detectRange = 5;
        moveSpeed = 0.5f;
        attackRange = 4;
        runRange = attackRange - 2; 
        curState = STATE.Idle;
        states[(int)curState].Enter(); // 이렇듯 상태머신을 만들때에, 최초 시작하는 상태를 트리거 하여 주며 객체의 유한상태머신을 구동을 할수 있겠다. 

        player = GameObject.FindGameObjectWithTag("Player").transform; // where some state of monster depend on player's current position. 
        returnPosition = transform.position;
    }

    public void ChangeState(STATE state)
    {
        states[(int)curState].Exit();
        curState = state;
        states[(int)curState].Enter();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, runRange);
    }
}

namespace batstate
{
    public enum STATE
    {
        Idle, Trace, Return, Attack, Runaway, Size
    }
    public class IdleState : StateBase<Bat>
    {
        private float idleTime;
        public IdleState(Bat owner) : base(owner)
        { }
        public override void Enter()
        {
            Debug.Log("Idle Enter");
        }
        public override void Exit()
        {
            Debug.Log("Idle Exit");
        }
        public override void SetUp()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.detectRange)
            {
                owner.ChangeState(STATE.Trace); // 1. 애초에 도망갈 범위보다 긴 조건문만 추가해주면 유한상태머신 유지된다. 
            }
        }
    }

    public class TraceState : StateBase<Bat>
    {
        public TraceState(Bat owner) : base(owner)
        {
        }
        public override void Enter()
        {
            Debug.Log("Trace Enter");
        }

        public override void Exit()
        {
            Debug.Log("Trace Exit");
        }

        public override void SetUp()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            Vector2 dir = (owner.player.position - owner.transform.position).normalized;
            // 플레이어 쫒아가기 
            owner.transform.Translate(dir * owner.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(owner.player.position, owner.transform.position) > owner.detectRange)
            {
                owner.ChangeState(STATE.Return);
            }
            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.attackRange) // 2. 마찬가지로 공격범위 가 도망범위보다 길기에 
            {
                owner.ChangeState(STATE.Attack);
            }
        }
    }

    public class ReturnState : StateBase<Bat>
    {
        public ReturnState(Bat owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Debug.Log("Return Enter");
        }

        public override void Exit()
        {
            Debug.Log("Return Exit");
        }

        public override void SetUp()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            // 원래 자리로 돌아가는 과정 
            Vector2 dir = (owner.returnPosition - owner.transform.position).normalized;
            owner.transform.Translate(dir * owner.moveSpeed * Time.deltaTime);

            //원래 자리에 도착했다면, 
            if (Vector2.Distance(owner.transform.position, owner.returnPosition) < 0.02f)
            {
                owner.ChangeState(STATE.Idle);
            }
            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.detectRange)
            {
                owner.ChangeState(STATE.Trace);
            }
        }
    }

    public class AttackState : StateBase<Bat>
    {
        private float lastAttackTime = 0;
        public AttackState(Bat owner) : base(owner)
        {
        }

        public override void Enter()
        {
            Debug.Log("Attack Enter");
        }
        public override void Exit()
        {
            Debug.Log("Attack Exit");
        }
        public override void SetUp()
        {
            throw new System.NotImplementedException();
        }
        public override void Update()
        {
            if (lastAttackTime > 1)
            {
                Debug.Log("공격");
                lastAttackTime = 0;
            }
            lastAttackTime += Time.deltaTime;

            //공격하던중 공격 사거리 밖에 있게된다면, 
            if (Vector2.Distance(owner.player.position, owner.transform.position) > owner.attackRange)
            {
                owner.ChangeState(STATE.Trace);
            }

            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.runRange) // 공격하다가 공격범위보다 더 가까워져, 맞을 위기에 놓인다면, 
            {
                owner.ChangeState(STATE.Runaway);
            }
        }
    }

    public class RunawayState : StateBase<Bat>
    {
        Vector2 runDir; 
        //RunAway 구현 
        public RunawayState(Bat owner) : base(owner)
        {
        }

        public override void Enter()
        {
            runDir = (owner.player.position - owner.transform.position).normalized * (-1); // 도망을 치는데, 도망치는 범위는 해당 상태 발동직후, 따라오는 용사의 반대방향으로 뛴다. 
            Debug.Log("Runaway Enter");
        }
        public override void Exit()
        {
            Debug.Log("Runaway Exit");
        }
        public override void SetUp()
        {
            throw new System.NotImplementedException();
        }
        public override void Update()
        {

            owner.transform.Translate(runDir * owner.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.runRange) // 충분히 도망쳤다면 그 다음범위에 할당되는 상태로 넘어가는것이 맞겠다. 
            {
                owner.ChangeState(STATE.Attack);
            }

            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.detectRange) // 이게 필요할까? 없어도 될듯하다. 
            {
                owner.ChangeState(STATE.Trace);
            }
        }
    }

}