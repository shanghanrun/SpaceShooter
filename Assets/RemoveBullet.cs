using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    //스파크 파티클 프리팹을 연결할 변수
    public GameObject sparkEffect;
    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Bullet")){

            // 첫번째 충돌지멍의 정보추출
            ContactPoint cp = other.GetContact(0);
            // 충돌 총알의 법선 벡터를 쿼터니언 타입으로 변환
            Quaternion rot = Quaternion.LookRotation(-cp.normal);

            GameObject spark = Instantiate(sparkEffect, cp.point, rot);
            Destroy(spark, 0.5f);
            Destroy(other.gameObject);
        }

    }
}
