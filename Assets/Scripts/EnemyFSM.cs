using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class EnemyFSM : MonoBehaviour
{
    enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Return,
        Damaged,
        Die
    }

    EnemyState m_state;

    public float findDistance = 8f;

    public float attackDistance = 2f;

    public float moveSpeed = 5f;

    float currentTime = 0;

    float attackDelay = 2f;

    public int attackPower = 3;

    Vector3 originPos;

    Quaternion originRot;

    public float moveDistance = 20f;

    public int hp = 15;

    int maxHp = 15;

    public Slider hpSlider;
    
    Transform player;

    CharacterController cc;

    Animator anim;

    NavMeshAgent smith;

    // Start is called before the first frame update
    void Start()
    {
        m_state = EnemyState.Idle;

        player = GameObject.Find("Player").transform;

        cc = GetComponent<CharacterController>();

        originPos = transform.position;

        anim = transform.GetComponentInChildren<Animator>();

        originRot = transform.rotation;

        smith = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        switch(m_state)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Move:
                Move();
                break;
            case EnemyState.Attack:
                Attack();
                break;
            case EnemyState.Return:
                Return();
                break;
            case EnemyState.Damaged:
               // Damaged();
                break;
            case EnemyState.Die:
               // Die();
                break;

        }

        hpSlider.value = (float)hp / (float)maxHp;

    }

    void Idle()
    {
        if(Vector3.Distance(transform.position, player.position)<findDistance)
        {
            m_state = EnemyState.Move;
            print("상태 전환: Idle -> Move");

            anim.SetTrigger("IdleToMove");
        }
    }

    void Move()
    {        
        if(Vector3.Distance(transform.position, originPos) > moveDistance)
        {
            m_state = EnemyState.Return;
            print("상태 전환: Move -> Return");
        }
        else if(Vector3.Distance(transform.position, player.position) > attackDistance)
        {
            //Vector3 dir = (player.position - transform.position).normalized;

            //cc.Move(dir * moveSpeed * Time.deltaTime);

            //transform.forward = dir;

            smith.isStopped = true;
            smith.ResetPath();

            smith.stoppingDistance = attackDistance;
            smith.destination = player.position;
        }
        else
        {
            m_state = EnemyState.Attack;
            print("상태 전환: Move-> Attack");
            currentTime = attackDelay;
            anim.SetTrigger("MoveToAttackDelay");
        }
    }

    void Attack()
    {
        if(Vector3.Distance(transform.position,player.position)<attackDistance)
        {
            currentTime += Time.deltaTime;
            if(currentTime>attackDelay)
            {
                //player.GetComponent<PlayerMove>().DamageAction(attackPower);
                print("공격");
                currentTime = 0;
                anim.SetTrigger("StartAttack");
            }
           
        }
        else
        {
            m_state = EnemyState.Move;
            print("상태 전환: Attack -> Move");
            currentTime = 0;

            anim.SetTrigger("AttackToMove");
        }
    }

    public void AttackAction()
    {
        player.GetComponent<PlayerMove>().DamageAction(attackPower);
    }

    void Return()
    {
        if(Vector3.Distance(transform.position, originPos)>0.1f)
        {
            //Vector3 dir = (originPos - transform.position).normalized;
            //cc.Move(dir * moveSpeed * Time.deltaTime);

            //transform.forward = dir;
            smith.destination = originPos;
            smith.stoppingDistance = 0;
        }
        else
        {
            smith.isStopped = true;
            smith.ResetPath();

            transform.position = originPos;
            transform.rotation = originRot;

            hp = maxHp;
            m_state = EnemyState.Idle;
            print("상태 전환: Return -> Idle");

            anim.SetTrigger("MoveToIdle");
        }
    }

    

    void Damaged()
    {
        StartCoroutine(DamagedProcess());
    }

    void Die()
    {
        StopAllCoroutines();

        StartCoroutine(DieProcess());
    }

    IEnumerator DamagedProcess()
    {
        yield return new WaitForSeconds(1.0f);

        m_state = EnemyState.Move;
        print("상태 전환: Damaged -> Move");
    }

    IEnumerator DieProcess()
    {
        cc.enabled = false;

        yield return new WaitForSeconds(2f);

        print("소멸!");
    }

    public void HitEnemy(int hitPower)
    {

        if( m_state == EnemyState.Damaged || m_state == EnemyState.Die || m_state == EnemyState.Return)
        {
            return;
        }

        hp -= hitPower;

        smith.isStopped = true;
        smith.ResetPath();

        if(hp>0)
        {
            m_state = EnemyState.Damaged;
            print("상태 전환: Any state -> Damaged");
            print(hp);

            anim.SetTrigger("Damaged");
            Damaged();
        }
        else
        {
            m_state = EnemyState.Die;
            print("상태 전환: Any state -> Die");
            print(hp);
            anim.SetTrigger("Die");
            Die();
        }
    }
}
