using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleUI : MonoBehaviour
{
    public RectTransform uiElement1; // ù ��° UI ���
    public RectTransform uiElement2; // �� ��° UI ���
    public float radius = 100f;  // ���� ������
    public float speed = 180f;  // �ʴ� ȸ�� ����
    private bool isAnimating = false;  // �ִϸ��̼��� ���� ������ Ȯ���ϴ� ����

    void Update()
    {
        // Space Ű�� ���� �� �ڷ�ƾ�� ����
        if (Input.GetKeyDown(KeyCode.Space) && !isAnimating)
        {
            StartCoroutine(SwapPositions());
        }
    }

    // �� UI �̹����� ��ġ�� �ݿ� ��θ� �׸��� ��ü�ϴ� �ڷ�ƾ
    IEnumerator SwapPositions()
    {
        isAnimating = true;
        float duration = 1f;  // �ִϸ��̼� ���� �ð�
        float elapsed = 0f;   // ��� �ð�
        float currentAngle = 0f;

        Vector2 initialPos1 = uiElement1.anchoredPosition;  // UI 1�� �ʱ� ��ġ
        Vector2 initialPos2 = uiElement2.anchoredPosition;  // UI 2�� �ʱ� ��ġ

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            currentAngle = Mathf.Lerp(0f, 180f, elapsed / duration);  // �ð��� ���� ���� ����

            // UIElement1�� �ݿ��� ��η� �̵�
            MoveAlongSemiCircle(uiElement1, currentAngle, initialPos1);

            // UIElement2�� �ݿ��� ��η� �̵� (�ݴ����)
            MoveAlongSemiCircle(uiElement2, 180f - currentAngle, initialPos2);

            yield return null;
        }

        // ��ġ�� ��Ȯ�� �ݴ������� �̵� (Ȥ�ó� ��Ȯ�� ��ġ�� ���� �ʾ��� ���� ���)
        uiElement1.anchoredPosition = initialPos2;
        uiElement2.anchoredPosition = initialPos1;

        isAnimating = false;  // �ִϸ��̼� �Ϸ�
    }

    // ������ ���� UI ��Ҹ� �ݿ� ��η� �̵���Ű�� �Լ�
    void MoveAlongSemiCircle(RectTransform uiElement, float angle, Vector2 initialPos)
    {
        float radian = Mathf.Deg2Rad * angle;
        float x = radius * Mathf.Cos(radian);
        float y = radius * Mathf.Sin(radian);

        // ���� ��ġ������ ��ȭ�� ������ ��ǥ
        uiElement.anchoredPosition = initialPos + new Vector2(x, y);
    }
}
