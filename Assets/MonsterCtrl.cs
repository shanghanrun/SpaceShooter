using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//네비게이션 기능
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public enum State{
        idle,
        trace,
        attack,
        die
    }

    public State state = State.idle;

    // 추적 사정거리
    public float traceDist = 10f;
    // 공격 사정거리
    public float attackDist = 2f;
    // 몬스터 사망여부
    public bool isDead = false;
    //! 몬스터 hp도 챙김
    int hp =30;  // 100에서 30으로 줄임

    Transform monsterTr;
    Transform playerTr;
    NavMeshAgent agent;
    Animator anim;

    //Animator 파라미터의 해시값 추출
    readonly int hashTrace = Animator.StringToHash("IsTrace");
    readonly int hashAttack = Animator.StringToHash("IsAttack");
    readonly int hashHit = Animator.StringToHash("Hit");
    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die");

    GameObject bloodEffect;

    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect"); //프리팹 로드

        // agent.destination = playerTr.position;

        
    }

    void OnEnable()
    { //  스크립트 활성화될 때(게임오브젝트 생성될 때) 호출되는 함수
        //이벤트 발생시 수행할 함수 연결
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        //! 몬스터제반 상태 초기화
        isDead = false;
        hp = 30;
        state = State.idle;  // 특히 State.die 상태를 바꾸지 않았으니 이것 반드시 필요

        // 몬스터의 상태를 체크하는 코루틴함수 호출
        StartCoroutine(CheckMonsterState());

        // 몬스터 상태에 따라 행동 수행하는 코루틴
        StartCoroutine(MonsterAction());
    }
    void OnDisable() // 스크립트 비활성화될 때(게임오브젝트 사라질 때) 호출되는 함수
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

   

    IEnumerator CheckMonsterState(){
        while(! isDead){
            // 0.3초동안 중지(대기)하는 동안 제어권을 메시지 루프에 양보
            yield return new WaitForSeconds(0.3f);

            //! 몬스터의 상태가 die일 경우 코루틴을 종료한다.
            if(state == State.die) yield break;

            // 몬스터와 주인공 캐릭터 사이의 거리를 측정
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            //공격 사정거리 범위로 들어왔는지 확인
            if(distance <= attackDist){
                state = State.attack;
            } else if(distance <= traceDist){
                state = State.trace;
            } else {
                state = State.idle;
            }
        }
    }
    IEnumerator MonsterAction(){
        while(! isDead){
            switch(state){
                case State.idle:
                    agent.isStopped = true;
                    anim.SetBool(hashTrace, false);
                    break;

                case State.trace:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    anim.SetBool(hashTrace, true);
                    anim.SetBool(hashAttack, false);
                    break;
                case State.attack:
                    anim.SetBool(hashAttack, true);
                    break;
                case State.die:
                    isDead = true;
                    //추격중지
                    agent.isStopped = true;
                    // 사망 에니메이션 실행
                    anim.SetTrigger(hashDie);
                    //몬스터의 콜라이더 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;

                    //! 일정시간 대기 후 오브젝트 풀로 환원시킴(비활성화시킴)
                    yield return new WaitForSeconds(3f);
                    // 다시 사용하기 위해 초기화세팅
                    // hp =60;
                    // isDead = false;
                    GetComponent<CapsuleCollider>().enabled = true;
                    // 몬스터 비활성화 = 오브젝트풀로 복귀
                    this.gameObject.SetActive(false);

                    break;
            }
            yield return new WaitForSeconds(0.3f);
        }
    }

    void OnCollisionEnter(Collision coll){
        if(coll.collider.CompareTag("Bullet")){
            Destroy(coll.gameObject);

            //=======RayCast로 인해 주석 처리 함 ===============
            anim.SetTrigger(hashHit);

            //총알 충돌 지점 찾기
            Vector3 pos = coll.GetContact(0).point;

            // 총알의 충돌 지점 법선 벡터
            Quaternion rot = Quaternion.LookRotation(-coll.GetContact(0).normal);
            //  혈흔효과 생성하는 함수호출
            ShowBloodEffect(pos, rot);

            //! 몬스터의 hp도 차감
            hp -= 10;
            if(hp <=0){
                state = State.die;

                //몬스터 사망시 score 50점 추가하며 화면표시
                GameManager.instance.DisplayScore(50);
            }
            //===========================================

        }
    }

    //레이캐스트에 충돌했을 경우
    // public void OnDamage(Vector3 pos, Vector3 normal){
    //     //피격 리액션 에니메이션 실행
    //     anim.SetTrigger(hashHit);
    //     Quaternion rot = Quaternion.LookRotation(normal);

    //     //혈흔효과
    //     ShowBloodEffect(pos,rot);
    //     //몬스터  hp차감
    //     hp -=10;
    //     if(hp <=0){
    //         state = State.die;
    //         GameManager.instance.DisplayScore(50);
    //     }
    // }

    void ShowBloodEffect(Vector3 pos, Quaternion rot){
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1f);
    }

    void OnDrawGizmos(){
        // 추적 사정거리 표시
        if(state == State.trace){
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }
        if(state == State.attack){
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }
    
    void OnPlayerDie(){
        // 몬스터의 상태를 체그하는 코루틴 함수를 모두 정지시킴
        StopAllCoroutines(); // StopAllCoroutine 단수가 아니라 복수

        //추적을 정지하고 애니메이션을 수행
        agent.isStopped = true;
        anim.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        anim.SetTrigger(hashPlayerDie);
    }
    

    
}
