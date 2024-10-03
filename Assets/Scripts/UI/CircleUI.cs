using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleUI : MonoBehaviour
{
    public RectTransform uiElement1; // 첫 번째 UI 요소
    public RectTransform uiElement2; // 두 번째 UI 요소
    public float radius = 100f;  // 원의 반지름
    public float speed = 180f;  // 초당 회전 각도
    private bool isAnimating = false;  // 애니메이션이 진행 중인지 확인하는 변수

    void Update()
    {
        // Space 키를 누를 때 코루틴을 실행
        if (Input.GetKeyDown(KeyCode.Space) && !isAnimating)
        {
            StartCoroutine(SwapPositions());
        }
    }

    // 두 UI 이미지의 위치를 반원 경로를 그리며 교체하는 코루틴
    IEnumerator SwapPositions()
    {
        isAnimating = true;
        float duration = 1f;  // 애니메이션 지속 시간
        float elapsed = 0f;   // 경과 시간
        float currentAngle = 0f;

        Vector2 initialPos1 = uiElement1.anchoredPosition;  // UI 1의 초기 위치
        Vector2 initialPos2 = uiElement2.anchoredPosition;  // UI 2의 초기 위치

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentAngle = Mathf.Lerp(0f, 180f, elapsed / duration);  // 시간에 따라 각도 증가

            // UIElement1을 반원의 경로로 이동
            MoveAlongSemiCircle(uiElement1, currentAngle, initialPos1);

            // UIElement2를 반원의 경로로 이동 (반대방향)
            MoveAlongSemiCircle(uiElement2, 180f - currentAngle, initialPos2);

            yield return null;
        }

        // 위치를 정확히 반대편으로 이동 (혹시나 정확한 위치에 놓지 않았을 때를 대비)
        uiElement1.anchoredPosition = initialPos2;
        uiElement2.anchoredPosition = initialPos1;

        isAnimating = false;  // 애니메이션 완료
    }

    // 각도에 따라 UI 요소를 반원 경로로 이동시키는 함수
    void MoveAlongSemiCircle(RectTransform uiElement, float angle, Vector2 initialPos)
    {
        float radian = Mathf.Deg2Rad * angle;
        float x = radius * Mathf.Cos(radian);
        float y = radius * Mathf.Sin(radian);

        // 원래 위치에서의 변화를 적용한 좌표
        uiElement.anchoredPosition = initialPos + new Vector2(x, y);
    }
}
