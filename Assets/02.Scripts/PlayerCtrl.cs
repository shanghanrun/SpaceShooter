using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    private Transform tr;
    public Transform gunTr;
    private Animation anim;
    public float moveSpeed = 5f;
    public float turnSpeed = 2f;

    public float initHp = 300f;
    public float currHp;

    //Hp_bar 연결할 변수
    private Image hpBar;

    //델리게이트 선언
    public delegate void PlayerDieHandler();
    // 이벤트 선언
    public static event PlayerDieHandler OnPlayerDie;

    // //! 마우스 고정 상태를 관리하는 변수
    // private bool isMouseLocked = false;

    IEnumerator Start()
    {
        //Hpbar 연결
        hpBar = GameObject.FindGameObjectWithTag("Hp_bar")?.GetComponent<Image>();

        //HP 초기화
        currHp = initHp;
        DisplayHealth();

        tr = GetComponent<Transform>();
        // 간단히 tr = transform; 해도 된다.
        anim = GetComponent<Animation>();

        anim.Play("Idle");
        turnSpeed = 0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 40f; // 80에서 40으로

        // //! 마우스 커서를 고정
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false; // 커서 숨기기
        // isMouseLocked = true; // 커서가 고정됨을 표시

    }

    void Update()
    {

        // 마우스 왼쪽 버튼을 클릭했을 때
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치에서 레이캐스트 발사
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                // 적 태그가 있는 오브젝트에 클릭했는지 확인
                if (hit.transform.CompareTag("Monster"))
                {
                    // 적의 위치로 회전
                    RotatePlayerTowards(hit.point);
                    // 총구가 적을 정확히 향하게 회전
                    RotateGunTowards(hit.point);
                }
            }
        }
        // 마우스 커서를 화면 중앙에 고정
        // Cursor.lockState = CursorLockMode.Locked;
        // 마우스 커서를 게임 윈도우 내에 제한
        Cursor.lockState = CursorLockMode.Confined;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 마우스의 좌우 이동량
        float r = Input.GetAxis("Mouse X");

        // 전후좌우 이동 방향 벡터 계산
        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        // 이동 처리
        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);

        // 마우스 이동량을 기반으로 캐릭터 회전 처리
        tr.Rotate(Vector3.up * Time.deltaTime * turnSpeed * r);

        
        PlayerAnim(h,v);

    }

    void RotatePlayerTowards(Vector3 targetPosition)
    {
        // 타겟 위치와 플레이어의 현재 위치를 기반으로 방향 벡터 계산
        Vector3 direction = (targetPosition - tr.position).normalized;
        direction.y = 0; // 플레이어가 수평으로만 회전하게 하기 위해 Y축은 고정

        // 타겟 방향으로의 회전 계산
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 부드럽게 회전 (Lerp 사용)
        tr.rotation = Quaternion.Lerp(tr.rotation, targetRotation, Time.deltaTime * turnSpeed);
        // 즉시 회전
        // tr.rotation = targetRotation;
    }

    void RotateGunTowards(Vector3 targetPosition)
    {
        // 총구가 적을 향하도록 회전
        Vector3 direction = (targetPosition - gunTr.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 부드럽게 회전 (Lerp 사용)
        gunTr.rotation = Quaternion.Lerp(gunTr.rotation, targetRotation, Time.deltaTime * turnSpeed);

        // 총구는 바로 회전
        // gunTr.rotation = targetRotation;
    }

    void PlayerAnim(float h, float v){
        if(v >=0.1f){
            anim.CrossFade("RunF", 0.25f); //전진 
        }
        else if(v <= -0.1f){
            anim.CrossFade("RunB", 0.25f);
        }
        else if( h>= 0.1f){
            anim.CrossFade("RunR", 0.25f);
        }
        else if( h <= -0.1f){
            anim.CrossFade("RunL", 0.25f);
        }
        else{
            anim.CrossFade("Idle", 0.25f);
        }
    }

    void OnTriggerEnter(Collider coll){
        if(currHp >= 0f && coll.CompareTag("Punch")){
            currHp -= 5f;  // 플레이어가 받는 데미지 5
            DisplayHealth();
            // print($"Player hp = {currHp/initHp}");

            if(currHp <=0f){
                PlayerDie();
            }
        }
    }
    void PlayerDie(){
        print("Player Die !");

        // GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");
        // foreach(GameObject monster in monsters){
        //     monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);        
        // }

        // 주인공 사망 이벤트 호출(발생)
        OnPlayerDie();

        //GameManager 스크립트의 IsGameOver 프로퍼티를 통해 isGameOver 값을 변경
        // GameObject.Find("GameMgr").GetComponent<GameManager>().IsGameOver = true;
        GameManager.instance.IsGameOver = true; // 싱글턴 접근
    }
    void DisplayHealth(){
        hpBar.fillAmount = currHp/initHp;
    }

    // void Shoot()
    // {
    //     // 총 발사 시 마우스를 플레이어가 바라보는 방향으로 이동
    //     Vector3 shootDirection = tr.forward; // 현재 바라보는 방향
    //     RaycastHit hit;

    //     // Ray를 쏴서 해당 방향에 무엇이 있는지 확인
    //     if (Physics.Raycast(tr.position, shootDirection, out hit))
    //     {
    //         // hit.point를 사용해 커서 위치를 변경할 수 있지만, Cursor.lockState를 사용하고 있으므로 
    //         // 직접적으로 커서를 이동시킬 수는 없다. 대신 플레이어의 방향을 기준으로 커서를 중앙에 고정시키고, 
    //         // 이를 통해 마우스 커서를 플레이어가 바라보는 방향으로 시뮬레이션할 수 있습니다.

    //         // 플레이어가 바라보는 방향으로 마우스 이동
    //         Vector3 screenPoint = Camera.main.WorldToScreenPoint(hit.point);
    //         Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // 커서를 원래 상태로 변경
    //         Cursor.lockState = CursorLockMode.Locked; // 다시 고정

    //         // 실제적으로 커서를 업데이트하는 방법이 없으므로, 대안으로 플레이어가 바라보는 방향으로 회전하게 두고
    //         // 마우스 좌클릭 후에 쏘는 로직을 추가하는 것이 좋습니다.
    //     }
    // }
}
