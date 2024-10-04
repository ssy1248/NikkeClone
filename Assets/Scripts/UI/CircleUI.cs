using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CircleUI : MonoBehaviour
{
    public RectTransform uiElement1;
    public RectTransform uiElement2;
    public Image uiElement1Image;  // ù ��° UI�� Image ������Ʈ
    public Image uiElement2Image;  // �� ��° UI�� Image ������Ʈ
    public Transform playerTransform;  // �÷��̾��� Transform (�Ӹ� ��ġ�� �������� ��)
    public float radius = 0.2f;  // ������ (UI ��� �� Z�� �Ÿ�)
    public float duration = 2f;  // �ִϸ��̼� ���� �ð�
    public Vector3 initialPos1 = new Vector3(0, 1.13f, -0.108f);  // ù ��° UI�� �ʱ� ��ġ
    public Vector3 initialPos2 = new Vector3(0, 1.039f, 0.292f);  // �� ��° UI�� �ʱ� ��ġ

    private bool isSwapped = false;  // ��ġ�� �ٲ������ ���θ� �����ϴ� �÷���

    void Start()
    {
        // UI�� �ʱ� ��ġ ���� (�÷��̾� �Ӹ� ��)
        ResetUIPositions();
    }

    void Update()
    {
        // Space Ű�� ������ �ִϸ��̼� ����
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSwapAnimation();
        }
    }

    // �� UI �̹����� �ݿ� ��η� �ִϸ��̼��ϰ�, ���ÿ� ���İ� ��ȯ
    void StartSwapAnimation()
    {
        // �ִϸ��̼� �ߺ� ���� ����
        if (DOTween.IsTweening(uiElement1) || DOTween.IsTweening(uiElement2))
            return;

        // ��ġ ��ȯ �ִϸ��̼� ����
        if (isSwapped)
        {
            // �� ��° UI ����� ��ġ�� ���� ��ġ�� �ִϸ��̼�
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement2, x, initialPos2, 1),
                180f,
                duration
            );

            // ù ��° UI ����� ��ġ�� ���� ��ġ�� �ִϸ��̼�
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement1, x, initialPos1, -1),
                180f,
                duration
            );

            // ���İ��� ������� �ǵ�����
            uiElement1Image.DOFade(1f, duration);
            uiElement2Image.DOFade(0.196f, duration);
        }
        else
        {
            // ù ��° UI ����� ��ġ�� �� ��° ��ġ�� �ִϸ��̼�
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement1, x, initialPos1, 1),
                180f,
                duration
            );

            // �� ��° UI ����� ��ġ�� ù ��° ��ġ�� �ִϸ��̼�
            DOTween.To(
                () => 0f,
                x => MoveAlongSemiCircle(uiElement2, x, initialPos2, -1),
                180f,
                duration
            );

            // ���İ��� ��ȯ
            uiElement1Image.DOFade(0.196f, duration);
            uiElement2Image.DOFade(1f, duration);
        }

        // �ִϸ��̼� �Ϸ� �� ���¸� ������Ŵ
        isSwapped = !isSwapped;
    }

    // ������ ���� UI ��Ҹ� �ݿ� ��η� �̵���Ű�� �Լ� (�÷��̾� �Ӹ� ����)
    void MoveAlongSemiCircle(RectTransform uiElement, float angle, Vector3 initialPosition, int direction)
    {
        float radian = Mathf.Deg2Rad * angle;

        // �÷��̾� ��ġ�� �������� Z���� ���� �ݿ� ��� �̵�
        Vector3 offset = new Vector3(
            radius * Mathf.Sin(radian) * direction,  // X�� ���
            0,  // Y ��ǥ�� ���� (�Ӹ� ���� ����)
            radius * Mathf.Cos(radian)  // Z�� ���
        );

        // UI ��Ҹ� �÷��̾� �Ӹ� ������ �̵���Ű�� (�÷��̾��� ���� ��ġ�� �߽����� �̵�)
        uiElement.position = playerTransform.position + initialPosition + offset;
    }

    // UI ��ġ �ʱ�ȭ �Լ�
    void ResetUIPositions()
    {
        uiElement1.position = playerTransform.position + initialPos1;
        uiElement2.position = playerTransform.position + initialPos2;

        // �̹��� ���İ� �ʱ� ����
        SetAlpha(uiElement1Image, 1f);  // ù ��° UI�� ���İ� 255
        SetAlpha(uiElement2Image, 0.196f);  // �� ��° UI�� ���İ� 50
    }

    // Image�� ���İ��� �����ϴ� �Լ�
    void SetAlpha(Image img, float alpha)
    {
        Color color = img.color;
        color.a = alpha;
        img.color = color;
    }
}
