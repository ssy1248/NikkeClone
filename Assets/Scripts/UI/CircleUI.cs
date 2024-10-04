using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CircleUI : MonoBehaviour
{
    public RectTransform uiElement1;
    public RectTransform uiElement2;
    public Image uiElement1Image;  // 첫 번째 UI의 Image 컴포넌트
    public Image uiElement2Image;  // 두 번째 UI의 Image 컴포넌트
    public Transform playerTransform;  // 플레이어의 Transform (머리 위치를 기준으로 함)
    public float radius = 0.2f;  // 반지름 (UI 요소 간 Z축 거리)
    public float duration = 2f;  // 애니메이션 지속 시간
    public Vector3 initialPos1 = new Vector3(0, 1.13f, -0.108f);  // 첫 번째 UI의 초기 위치
    public Vector3 initialPos2 = new Vector3(0, 1.039f, 0.292f);  // 두 번째 UI의 초기 위치

    private bool isSwapped = false;  // 위치가 바뀌었는지 여부를 저장하는 플래그

    void Start()
    {
        // UI의 초기 위치 설정 (플레이어 머리 위)
        ResetUIPositions();
    }

    void Update()
    {
        // Space 키를 누르면 애니메이션 시작
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSwapAnimation();
        }
    }

    // 두 UI 이미지를 반원 경로로 애니메이션하고, 동시에 알파값 교환
    void StartSwapAnimation()
    {
        // 애니메이션 중복 실행 방지
        if (DOTween.IsTweening(uiElement1) || DOTween.IsTweening(uiElement2))
            return;

        // 위치 교환 애니메이션 시작
        if (isSwapped)
        {
            // 두 번째 UI 요소의 위치를 원래 위치로 애니메이션
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement2, x, initialPos2, 1),
                180f,
                duration
            );

            // 첫 번째 UI 요소의 위치를 원래 위치로 애니메이션
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement1, x, initialPos1, -1),
                180f,
                duration
            );

            // 알파값도 원래대로 되돌리기
            uiElement1Image.DOFade(1f, duration);
            uiElement2Image.DOFade(0.196f, duration);
        }
        else
        {
            // 첫 번째 UI 요소의 위치를 두 번째 위치로 애니메이션
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement1, x, initialPos1, 1),
                180f,
                duration
            );

            // 두 번째 UI 요소의 위치를 첫 번째 위치로 애니메이션
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement2, x, initialPos2, -1),
                180f,
                duration
            );

            // 알파값도 교환
            uiElement1Image.DOFade(0.196f, duration);
            uiElement2Image.DOFade(1f, duration);
        }

        // 애니메이션 완료 후 상태를 반전시킴
        isSwapped = !isSwapped;
    }

    // 각도에 따라 UI 요소를 반원 경로로 이동시키는 함수 (플레이어 머리 기준)
    void MoveAlongSemiCircle(RectTransform uiElement, float angle, Vector3 initialPosition, int direction)
    {
        float radian = Mathf.Deg2Rad * angle;

        // 플레이어 위치를 기준으로 Z축을 따라 반원 경로 이동
        Vector3 offset = new Vector3(
            radius * Mathf.Sin(radian) * direction,  // X축 경로
            0,  // Y 좌표는 고정 (머리 위에 고정)
            radius * Mathf.Cos(radian)  // Z축 경로
        );

        // UI 요소를 플레이어 머리 위에서 이동시키기 (플레이어의 현재 위치를 중심으로 이동)
        uiElement.position = playerTransform.position + initialPosition + offset;
    }

    // UI 위치 초기화 함수
    void ResetUIPositions()
    {
        uiElement1.position = playerTransform.position + initialPos1;
        uiElement2.position = playerTransform.position + initialPos2;

        // 이미지 알파값 초기 설정
        SetAlpha(uiElement1Image, 1f);  // 첫 번째 UI의 알파값 255
        SetAlpha(uiElement2Image, 0.196f);  // 두 번째 UI의 알파값 50
    }

    // Image의 알파값을 설정하는 함수
    void SetAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
