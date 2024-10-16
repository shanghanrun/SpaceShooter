using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20f;
    public float force = 6000f;

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // rb.AddForce(transform.forward * force); 월드좌표기준
        rb.AddRelativeForce(Vector3.forward * force); // 로컬좌표 기준
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
