using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect;

    Transform tr;
    Rigidbody rb;
    Rigidbody rb2;
    //무작위로 적용할 텍스처 배열
    public Texture[] textures;
    // 하위에 있는 Mesh Renderer 컴포넌트를 저장할 변수
    MeshRenderer  meshRenderer;
    // new MeshRenderer renderer;//기존 MonoBehaviour의  renderer와 구분하기 위해 new를 사용
    int hitCount =0;

    //폭발 반경
    public float radius = 10f;
    Collider[] colls = new Collider[10];

    //! 폭발을 간접적으로 영향받을 레이어들
    public LayerMask affectedLayers;
    
    
    // Start is called before the first frame update
    void Start()
    {
        //! 예시: Layer 3과 Layer 5를 포함하도록 설정
        affectedLayers = (1 << 3) | (1 << 6);  // 3번 Barrel, 6번 Monster_body

        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        
        if (meshRenderer != null)
        {
            // 난수 발생
            int idx = Random.Range(0, textures.Length);
            // 새로운 머티리얼을 생성
            
            // 텍스처 지정
            meshRenderer.material.mainTexture = textures[idx];

            // 확인을 위한 출력
            // print("Applied Texture: " + meshRenderer.material.mainTexture);
        } else{
            // print("Mesh renderer가 없음");
        }
    }

    void OnCollisionEnter(Collision other){
        if(other.gameObject.CompareTag("Bullet")){
            // c총알 맞은 횟수를 증가시키고 3회이상이면 폭발처리
            if( ++hitCount ==3){
                ExpBarrel();
            }
        }
    }

    void ExpBarrel(){
        // 폭발 효과 파티클 생성
        GameObject exp = Instantiate(expEffect, tr.position, Quaternion.identity);

        Destroy(exp, 1.5f);

        //rigidbody 컴포넌트의 mass 를 1로 해서 무게를 가볍게
        // rb.mass = 1f;
        //위로 솟구치는 힘을 가함
        // rb.AddForce(Vector3.up * 1500f);

        // 간접 폭발력 전달
        IndirectDamage(tr.position);

        // 3초후에 드럼통 제거
        Destroy(gameObject, 1f);
    }

    void IndirectDamage(Vector3 pos){
        
        //주변에 있는 드럼통을 모두 추출
        Physics.OverlapSphereNonAlloc(pos, radius, colls, affectedLayers);

        foreach(var coll in colls){
            if(coll != null){
                rb2 = coll.GetComponent<Rigidbody>();
                if(rb2 !=null){
                    rb2.mass = 1f;
                    rb2.constraints = RigidbodyConstraints.None;
                    rb2.AddExplosionForce(1500f, pos, radius, 1200f);
                }
            }
            
        }
    }
}
