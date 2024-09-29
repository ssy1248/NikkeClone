using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObjectBase : MonoBehaviour
{
    public int BreakableObjectHP;

    public void BreakableObjectAttack(float dmg)
    {
        //건물이 맞으면 체력을 깎으면서 건물 오브젝트의 알파값을 줄이면서 0이되면 Destroy
        //만약 건물 뒤에 적이있다면 적이 우선공격이 아닌 건물이 우선공격이 되는 로직 구현
    }
}
