using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // public Transform[] points;
    public List<Transform> points = new List<Transform>();

    //몬스터를 미리 생성해 저장할 리스트 자료형
    public List<GameObject> monsterPool = new List<GameObject>();
    // 오브젝트 풀에 생성할 몬스터의 최대 갯수
    public int maxMonsters = 10;

    // 몬스터 프리팹을 연결할 변수
    public GameObject monster;

    //몬스터 생성 간격
    public float createTime =10f;
    //게임종료 여부를 저장할 맴버변수
    bool isGameOver;

    //게임종료 여부를 저장할 프로퍼티
    public bool IsGameOver{
        get{return isGameOver;}
        set{
            isGameOver = value;
            if(isGameOver){
                CancelInvoke("CreateMonster");
            }
        }
    }

    //! 싱글텅 인스턴스 선언
    public static GameManager instance = null;

    //스코어 변수
    public TMP_Text scoreText;
    // 누적점수 변수
    int totalScore =0;

    void Awake(){
        if(instance == null){
            instance = this;
        } else if( instance != this){
            Destroy(this.gameObject);
        }

        //다른 씬으로 넘어가도 삭제하지 않고 유지
        DontDestroyOnLoad(this.gameObject);
    }
    
    void Start()
    {
        // 스코어 점수출력
        DisplayScore(0);  // 처음에는 0점 출력
        // PlayerPrefs에서 불러오기
        totalScore = PlayerPrefs.GetInt("Total_Score", 0);

        // 몬스터 오브젝트 풀 생성
        CreateMonsterPool();

        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        // points = spawnPointGroup?.GetComponentsInChildren<Transform>();
        // spawnPointGroup?.GetComponentsInChildren<Transform>(points);
        foreach(Transform point in spawnPointGroup){
            points.Add(point);  // points가 빈 List이며 Transform타입이므로 Add
        }

        //일정시간 간격으로 함수 호출
        InvokeRepeating("CreateMonster", 2f, createTime);

    }

    void CreateMonster(){
        int idx = Random.Range(0, points.Count); // 리스트의 요수갟수 Count
        // Instantiate(monster, points[idx].position, points[idx].rotation);

        //오브젝트풀에서 몬스터 추출
        GameObject _monster = GetMonsterInPool();
        //추출한 몬스터 위치와 회전을 설정
        _monster?.transform.SetPositionAndRotation(points[idx].position, points[idx].rotation);

        //추출한 몬스터 활성화
        _monster?.SetActive(true);
    }

    void CreateMonsterPool(){
        for(int i=0; i< maxMonsters; i++){
            //몬스터 생성
            var _monster = Instantiate<GameObject>(monster);
            // GameObject _monster = Instantiate(monster); 해도 된다. 반환타입을 알려주었으니
            // 몬스터 이름지정
            _monster.name = $"Monster_{i:00}"; 
            // 몬스터 비활성화
            _monster.SetActive(false);

            // 몬스터 풀에 추가
            monsterPool.Add(_monster);
        }
    }
    //오브젝트 풀에서 사용가능한 몬스터를 추출해 반환하는 함수
    public GameObject GetMonsterInPool(){
        foreach( var _monster in monsterPool){
            //비활성화 여부로 사용가능한 몬스터를 판단
            if(_monster.activeSelf == false){
                //비활성화한 것, 한개만 반환.  return하는 순간 함수 빠져나간다.
                return _monster;
            }
        }
        return null;  // 없을 경우 빈객체라도 반환해야 된다.
    }

    public void DisplayScore(int score){ // 추가하는 스코어를 넣고, 결과 출력
        totalScore += score;
        scoreText.text = $"<color=#00ff00>SCORE :</color> <color=#ff0000>{totalScore:#,##0}</color>";

        //스코어 저장
        PlayerPrefs.SetInt("Total_Score", totalScore);
    }
}
