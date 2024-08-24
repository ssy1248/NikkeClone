using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseToScope : MonoBehaviour
{
    public GameObject Scope;

    void Start()
    {
        if (Scope == null)
        {
            Debug.LogError("Scope ������Ʈ�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    void Update()
    {
        // ���콺 �̵� �� ��ǥ ��������
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        // ���콺 Ŭ�� ���� �� Scope ������Ʈ ��ġ ������Ʈ
        if (Input.GetMouseButton(0)) // ���� ���콺 ��ư Ŭ�� ���� ��
        {
            Scope.SetActive(true);
            Scope.transform.position = mousePosition;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Scope.SetActive(false);
        }
    }
}
