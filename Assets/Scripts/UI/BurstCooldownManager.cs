using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BurstCooldownManager : MonoBehaviour
{
    public void BurstCoolTime(Button clickedButton)
    {
        GameObject obj = GameObject.Find(clickedButton.transform.GetComponentInChildren<TextMeshProUGUI>().text);
        float coolTime = obj.GetComponent<PlayerStat>().currentBurstCoolTime;
        ButtonBase btb = clickedButton.GetComponentInChildren<ButtonBase>();

        if (btb.isCoolTime)
        {
            Debug.Log("�̹� ��Ÿ���� ���ư��� �ֽ��ϴ�.");
            return;
        }

        StartCoroutine(StartCooldown(coolTime, btb));
    }

    private IEnumerator StartCooldown(float coolTime, ButtonBase btb)
    {
        Debug.Log("��Ÿ�� �ڷ�ƾ ����");
        btb.isCoolTime = true;
        btb.cooltimeBase.gameObject.SetActive(true);

        while (coolTime > 0f)
        {
            coolTime -= Time.deltaTime;
            btb.cooltimeText.text = coolTime.ToString("F0");
            yield return null;
        }

        btb.cooltimeBase.gameObject.SetActive(false);
        btb.isCoolTime = false;
    }
}
