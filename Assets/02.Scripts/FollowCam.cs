using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform targetTr;
    private Transform camTr;

    //따라갈 대상으로부터 떨어질 거리
    [Range(2f, 20f)]
    public float distance = 10f;

    //Y축으로 이동할 높이
    [Range(0, 10f)]
    public float height =2f;

    // 반응속도 (도달시간)
    public float damping =10f;

    //SmoothDamp 에서 사용할 변수
    private Vector3 velocity = Vector3.zero;
    // 카메라 LookAt의 Offset값
    public float targetOffset =2f;

    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        // 추적해야할 대상의 뒤쪽으로 distance만큼 이동
        //높이를 height만큼 이동
        Vector3 pos = targetTr.position 
                        + (-targetTr.forward *distance)
                        + (Vector3.up *height);

        //구면 선형보간
        // camTr.position = Vector3.Slerp(camTr.position, pos, Time.deltaTime *damping);

        //SmoothDamp 이용한 위치 보간
        camTr.position = Vector3.SmoothDamp(camTr.position,
                                            pos,
                                            ref velocity,
                                            damping);

        // Camera를 피벗 좌표를 향해 회전
        camTr.LookAt(targetTr.position + (targetTr.up * targetOffset));
    }
}
