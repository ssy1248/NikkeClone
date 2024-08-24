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
            Debug.LogError("Scope 오브젝트가 할당되지 않았습니다.");
        }
    }

    void Update()
    {
        // 마우스 이동 시 좌표 가져오기
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

        // 마우스 클릭 중일 때 Scope 오브젝트 위치 업데이트
        if (Input.GetMouseButton(0)) // 왼쪽 마우스 버튼 클릭 중일 때
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
