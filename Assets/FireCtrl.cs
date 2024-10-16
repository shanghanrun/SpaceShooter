using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 반드시 필요한 컴포넌트를 명시해 해당 컴포넌트가 삭제되는 것을 방지하는 어트리뷰트
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    //총알 프리팹
    public GameObject bullet;
    public Transform firePos;
    // public Transform riflePos;
    public AudioClip fireSfx;

    //AudioSource 컴포넌트를 저장할 변수
    new AudioSource audio;
    // Muzzle Flash의 MeshRenderer  컴포넌트
    MeshRenderer muzzleFlash;

    //RayCast 결과값을 저장하기 위한 구조체 선언
    RaycastHit hit;

    

    void Start()
    {
        audio = GetComponent<AudioSource>();
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        if(muzzleFlash != null){
            // print("muzzleFlash 있음");
        }
        muzzleFlash.enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        // Ray를 시각적으로 표시
        Debug.DrawRay(firePos.position, firePos.forward * 10f, Color.green);

        if(Input.GetMouseButtonDown(0)){
            Fire();

            // 발사할 때, Ray도 발사
            // if(Physics.Raycast(firePos.position, //발사원점
            //                     firePos.forward,// 발사방향
            //                     out hit, // 광선에 맞은 결과 데이터
            //                     10f, // 광선거리
            //                     1<<6)){  // 레이어마스크 
                                    
            //     // Debug.Log($"Hit = {hit.transform.name}");
            //     hit.transform.GetComponent<MonsterCtrl>()?.OnDamage(hit.point, hit.normal);

            // }
        }
    }
    void Fire(){
        Instantiate(bullet, firePos.position, firePos.rotation);

        audio.PlayOneShot(fireSfx, 1f);

        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash(){


        muzzleFlash.enabled = true;
        // print("muzzleFlash Enabled");

        yield return new WaitForSeconds(0.2f);

        muzzleFlash.enabled = false;
        // print("muzzleFlash Enabled false");
    }
}
