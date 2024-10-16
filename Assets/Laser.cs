using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    // public Material laserMaterial; // 레이저에 적용할 머티리얼
    public float laserLength = 10f; // 레이저 길이
    public Color laserColor = Color.red; // 레이저 색상 너무 어둡다.
    public float brightness = 2f;
    public float laserWidth = 0.04f;
    public float duration = 0.1f; // 레이저가 보이는 시간

    private LineRenderer lineRenderer;

    void Start()
    {
        // LineRenderer 컴포넌트 추가
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth =laserWidth; // 시작 폭
        lineRenderer.endWidth = laserWidth; // 끝 폭
        lineRenderer.positionCount = 2; // 두 점으로 선을 그리기 위해 포지션 카운트 설정
        lineRenderer.startColor = laserColor; // 시작 색상
        lineRenderer.endColor = laserColor; // 끝 색상
        lineRenderer.useWorldSpace = true; // 월드 스페이스를 사용

        // // 레이저 머티리얼 설정 (Inspector에서 설정된 경우 사용)
        // if (laserMaterial != null)
        // {
        //     lineRenderer.material = laserMaterial;
        // }
        // else
        // {
        //     laserMaterial = new Material(Shader.Find("Unlit/Color")); // Unlit/Color 사용
        //     laserMaterial.color = laserColor;
        //     //Emission 설정
        //     laserMaterial.SetColor("_EmissionColor", laserColor * brightness);
        //     lineRenderer.material = laserMaterial;
        // }
    }

    void Update()
    {
        // 레이저를 발사
        ShootLaser();
    }

    void ShootLaser()
    {
        // 레이저의 시작점은 무기의 위치
        Vector3 startPosition = transform.position;
        // startPosition.y += 0.2f; 좀더 높게 잡았다가 다시 원래 대로
        
        // 레이저의 끝점은 무기 방향으로 레이저 길이만큼 이동한 위치
        Vector3 endPosition = startPosition + transform.forward * laserLength;

        // LineRenderer의 포지션 설정
        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);

        // 레이저 비활성화
        StartCoroutine(DeactivateLaser());
    }

    IEnumerator DeactivateLaser()
    {
        // 지정된 시간만큼 대기 후 레이저를 비활성화
        yield return new WaitForSeconds(duration);
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, Vector3.zero);
    }
}
