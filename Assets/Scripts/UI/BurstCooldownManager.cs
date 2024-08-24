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
            Debug.Log("이미 쿨타임이 돌아가고 있습니다.");
            return;
        }

        StartCoroutine(StartCooldown(coolTime, btb));
    }

    private IEnumerator StartCooldown(float coolTime, ButtonBase btb)
    {
        Debug.Log("쿨타임 코루틴 시작");
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
