using BeeState;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class Bee : MonoBehaviour
{
    [Header("Bee's Current State")]
    [SerializeField] private StateBase<Bee>[] states;
    [SerializeField] public STATE curState; 

    [Header("Bee's properties")]
    [SerializeField] public float detectRange;
    [SerializeField] public float moveSpeed;
    [SerializeField] public float attackRange;
    [SerializeField] public Transform[] patrolPoints;

    [SerializeField] public Vector3 returnPosition;
    [SerializeField] public Transform player;
    public int patrolIndex = 0; // debuggin purposes, to show which patrolpoint target is searching 

    // 기존의 상태끼리 캡슐화가 되지않은 상황에 대해서는, 상태끼리 값에 대해서 간섭이 가능하다. (e.g. Trace 에서 Idle의 값을 간섭할수 있게된다.) 
    // 따라서, 각 상태에 따라 새로운 클래스로 캡슐화를 하게 된다면, 각 상태에 포함되는 맴버변수들끼리는 간섭이 불가능하게 된다. 
    // 캡슐화 이후에는, 각 상태별로 배열로 지니게 할수 있겠다. 

    // 캡슐화 할때 주의사항 : 추상화 객체에 대해서, State를 지니고 있는 객체를 참조할수 있도록, 생성자에서 참조대상을 설정할수 있도록 하는것이 정배다. 

    private void Awake()
    {
        states = new StateBase<Bee>[(int)STATE.Size]; // Size 값을 이용해서, 열거형의 값대로의 배열생성응용이 가능하여지겠다.
        states[(int)STATE.Idle] = new IdleState(this);
        states[(int)STATE.Trace] = new TraceState(this);
        states[(int)STATE.Return] = new ReturnState(this);
        states[(int)STATE.Attack] = new AttackState(this);
        states[(int)STATE.Patrol] = new PatrolState(this);
    }

    private void Update()
    {
        states[(int)curState].Update(); // where 캡슐화된 객체들끼리 알아서 현재 상태에 대해서 evaluate 해주기 때문에, frame 별 업데이트할때에는 그저 현재상태에 따른 update function만 진행해주면 되겠다. 

    }

    private void Start()
    {
        detectRange = 4;
        moveSpeed = 1; 
        attackRange = 1;
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
    }
}

namespace BeeState
{
    public enum STATE
    {
        Idle, Trace, Return, Attack, Patrol, Size
    }
    public class IdleState : StateBase<Bee>
    {
        private float idleTime; 
        public IdleState(Bee owner) : base(owner)
        {}
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
            idleTime += Time.deltaTime;
            if (idleTime > 2)
            {
                idleTime = 0; 
                owner.ChangeState(STATE.Patrol);
            }
            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.detectRange)
            {
                owner.ChangeState(STATE.Idle);
            }
        }
    }

    public class TraceState : StateBase<Bee>
    {
        public TraceState(Bee owner) : base(owner)
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
            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.attackRange)
            {
                owner.ChangeState(STATE.Attack);
            }
        }
    }

    public class ReturnState : StateBase<Bee>
    {
        public ReturnState(Bee owner) : base(owner)
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

    public class AttackState : StateBase<Bee>
    {
        private float lastAttackTime = 0;
        public AttackState(Bee owner) : base(owner)
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
        }
    }

    public class PatrolState : StateBase<Bee>
    {
        public PatrolState(Bee owner) : base(owner)
        {
        }

        public override void Enter()
        {
            owner.patrolIndex = (owner.patrolIndex + 1) % owner.patrolPoints.Length; //이렇게 Modulus 를 이용해서 list에 있는 값들을 순차적으로, count를 넘지 않는 수순에서 반복확인이 가능하다.
            Debug.Log("Patrol Enter");
        }
        public override void Exit()
        {
            Debug.Log("Patrol Exit");
        }
        public override void SetUp()
        {
            throw new System.NotImplementedException();
        }
        public override void Update()
        {
            // 순찰 진행: 패트롤 포인트지정 


            // Patrol point로 순찰 
            Vector2 dir = (owner.patrolPoints[owner.patrolIndex].position - owner.transform.position).normalized;
            owner.transform.Translate(dir * owner.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(owner.transform.position, owner.patrolPoints[owner.patrolIndex].position) < 0.02f)
            {
                owner.ChangeState(STATE.Idle); 
            }

            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.detectRange)
            {
                owner.ChangeState(STATE.Trace);
            }
        }
    }

}
