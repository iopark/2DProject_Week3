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
    [SerializeField] public float runRange; // ����ĥ Ÿ�̹��� �����ϱ� ���� �ɹ����� 

    [SerializeField] public Vector3 returnPosition;
    [SerializeField] public Transform player;

    private void Awake()
    {
        states = new StateBase<Bat>[(int)STATE.Size]; // Size ���� �̿��ؼ�, �������� ������� �迭���������� �����Ͽ����ڴ�.
        states[(int)STATE.Idle] = new IdleState(this);
        states[(int)STATE.Trace] = new TraceState(this);
        states[(int)STATE.Return] = new ReturnState(this);
        states[(int)STATE.Attack] = new AttackState(this);
        states[(int)STATE.Runaway] = new RunawayState(this); // �������� �༮
    }

    private void Update()
    {
        states[(int)curState].Update(); // where ĸ��ȭ�� ��ü�鳢�� �˾Ƽ� ���� ���¿� ���ؼ� evaluate ���ֱ� ������, frame �� ������Ʈ�Ҷ����� ���� ������¿� ���� update function�� �������ָ� �ǰڴ�. 

    }

    private void Start()
    {
        detectRange = 5;
        moveSpeed = 0.5f;
        attackRange = 4;
        runRange = attackRange - 2; 
        curState = STATE.Idle;
        states[(int)curState].Enter(); // �̷��� ���¸ӽ��� ���鶧��, ���� �����ϴ� ���¸� Ʈ���� �Ͽ� �ָ� ��ü�� ���ѻ��¸ӽ��� ������ �Ҽ� �ְڴ�. 

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
                owner.ChangeState(STATE.Trace); // 1. ���ʿ� ������ �������� �� ���ǹ��� �߰����ָ� ���ѻ��¸ӽ� �����ȴ�. 
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
            // �÷��̾� �i�ư��� 
            owner.transform.Translate(dir * owner.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(owner.player.position, owner.transform.position) > owner.detectRange)
            {
                owner.ChangeState(STATE.Return);
            }
            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.attackRange) // 2. ���������� ���ݹ��� �� ������������ ��⿡ 
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
            // ���� �ڸ��� ���ư��� ���� 
            Vector2 dir = (owner.returnPosition - owner.transform.position).normalized;
            owner.transform.Translate(dir * owner.moveSpeed * Time.deltaTime);

            //���� �ڸ��� �����ߴٸ�, 
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
                Debug.Log("����");
                lastAttackTime = 0;
            }
            lastAttackTime += Time.deltaTime;

            //�����ϴ��� ���� ��Ÿ� �ۿ� �ְԵȴٸ�, 
            if (Vector2.Distance(owner.player.position, owner.transform.position) > owner.attackRange)
            {
                owner.ChangeState(STATE.Trace);
            }

            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.runRange) // �����ϴٰ� ���ݹ������� �� �������, ���� ���⿡ ���δٸ�, 
            {
                owner.ChangeState(STATE.Runaway);
            }
        }
    }

    public class RunawayState : StateBase<Bat>
    {
        Vector2 runDir; 
        //RunAway ���� 
        public RunawayState(Bat owner) : base(owner)
        {
        }

        public override void Enter()
        {
            runDir = (owner.player.position - owner.transform.position).normalized * (-1); // ������ ġ�µ�, ����ġ�� ������ �ش� ���� �ߵ�����, ������� ����� �ݴ�������� �ڴ�. 
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

            if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.runRange) // ����� �����ƴٸ� �� ���������� �Ҵ�Ǵ� ���·� �Ѿ�°��� �°ڴ�. 
            {
                owner.ChangeState(STATE.Attack);
            }

            else if (Vector2.Distance(owner.player.position, owner.transform.position) < owner.detectRange) // �̰� �ʿ��ұ�? ��� �ɵ��ϴ�. 
            {
                owner.ChangeState(STATE.Trace);
            }
        }
    }

}