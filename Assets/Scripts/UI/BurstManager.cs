using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BurstManager : MonoBehaviour
{
    GameManager gm;

    public BurstCooldownManager burstCooldownManager;

    // 버스트 바를 넣을 리스트
    public List<Image> BurstBarImage = new List<Image>();
    public int BurstIndex;
    private int currentIndex = 0;

    // 생성될 버스트 이미지
    public Button Burst1Image;
    public Button Burst2Image;
    public Button Burst3Image;

    public bool OneBurstIsOn = false;
    public bool TwoBurstIsOn = false;
    public bool ThreeBurstIsOn = false;
    public bool FullBurstOn = false;

    // 생성된 버튼들을 저장할 리스트
    private List<Button> burst1Buttons = new List<Button>();
    private List<Button> burst2Buttons = new List<Button>();
    private List<Button> burst3Buttons = new List<Button>();

    //버튼이 눌렸는지 체크할 bool
    [SerializeField]
    private bool isButtonClicked = false;

    public void Awake()
    {
        gm = FindObjectOfType<GameManager>();
        BurstIndex = 0;

        // 0번 제외 모든 BurstBarImage 비활성화
        foreach (var burstBar in BurstBarImage)
        {
            burstBar.gameObject.SetActive(false);
        }
        BurstBarImage[0].gameObject.SetActive(true);
    }

    public void Update()
    {
        if (FullBurstChk(currentIndex) == false)
        {
            Burst();
        }
        else
        {
            BurstIndex = 0;
        }
    }

    public void BtnBurstClick(Button clickedButton)
    {
        isButtonClicked = true;
        ButtonBase btb = clickedButton.GetComponentInChildren<ButtonBase>();

        if (btb.isCoolTime)
            return;

        burstCooldownManager.BurstCoolTime(clickedButton);

        BurstIndex++;
    }

    public void UpdateBurstBar(float attackPower)
    {
        if (BurstBarImage.Count > 0)
        {
            // 0 -> 1버스트로 진행할때
            if (BurstIndex == 0)
            {
                // 현재 fillAmount에 attackPower에 비례한 양을 더함
                BurstBarImage[0].fillAmount += attackPower / gm.TotalBurst;

                // fillAmount가 1을 초과하지 않도록 제한
                if (BurstBarImage[0].fillAmount >= 1f)
                {
                    OneBurstIsOn = true;
                    BurstIndex += 1;
                    BurstBarImage[0].fillAmount = 1f;

                    // BurstIndex에 따라 BurstBarImage 활성화
                    BurstBarImage[BurstIndex - 1].gameObject.SetActive(false);
                    BurstBarImage[BurstIndex].gameObject.SetActive(true);
                }
            }
        }
    }

    void Burst()
    {
        GameObject spawn = GameObject.Find("Burst").gameObject;
        Vector3 spawnPosition = spawn.transform.position;

        if (BurstIndex == 1 && OneBurstIsOn)
        {
            currentIndex = BurstIndex;
            HandleBurst(gm.OneBurstPlayer, burst1Buttons, Burst1Image, spawn, spawnPosition);
            OneBurstIsOn = false;
            TwoBurstIsOn = true;
        }
        else if (BurstIndex == 2 && TwoBurstIsOn)
        {
            currentIndex = BurstIndex;
            HandleBurstTransition(burst1Buttons, gm.TwoBurstPlayer, burst2Buttons, Burst2Image, spawn, spawnPosition, 10f);
            TwoBurstIsOn = false;
            ThreeBurstIsOn = true;
        }
        else if (BurstIndex == 3 && ThreeBurstIsOn)
        {
            currentIndex = BurstIndex;
            HandleBurstTransition(burst2Buttons, gm.ThreeBurstPlayer, burst3Buttons, Burst3Image, spawn, spawnPosition, 10f);
            ThreeBurstIsOn = false;
            FullBurstOn = true;
        }
        else if (BurstIndex == 4 && FullBurstOn)
        {
            currentIndex = BurstIndex;
            HandleFullBurst(burst3Buttons, 5f);
            FullBurstOn = false;
        }

        isButtonClicked = false;
    }

    void HandleBurst(List<Player> players, List<Button> burstButtons, Button burstImage, GameObject spawn, Vector3 spawnPosition)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (i > 0) spawnPosition.y -= 110 * i;

            if (burstButtons.Count <= i)
            {
                Button newBurstIButton = Instantiate(burstImage, spawnPosition, Quaternion.identity, spawn.transform);
                newBurstIButton.onClick.AddListener(() => BtnBurstClick(newBurstIButton));
                newBurstIButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = players[i].name;
                burstButtons.Add(newBurstIButton);
            }
            else
            {
                burstButtons[i].gameObject.SetActive(true);
            }
        }
    }

    void HandleBurstTransition(List<Button> previousBurstButtons, List<Player> currentBurstPlayers, List<Button> currentBurstButtons,
        Button currentBurstImage, GameObject spawn, Vector3 spawnPosition, float fillDuration)
    {
        BurstBarImage[BurstIndex - 1].gameObject.SetActive(false);
        BurstBarImage[BurstIndex].gameObject.SetActive(true);

        foreach (Button button in previousBurstButtons)
        {
            button.gameObject.SetActive(false);
        }

        HandleBurst(currentBurstPlayers, currentBurstButtons, currentBurstImage, spawn, spawnPosition);
        StartCoroutine(ChangeFillAmountOverTime(BurstIndex, fillDuration));
    }

    void HandleFullBurst(List<Button> previousBurstButtons, float fillDuration)
    {
        BurstBarImage[BurstIndex - 1].gameObject.SetActive(false);
        BurstBarImage[BurstIndex].gameObject.SetActive(true);

        foreach (Button button in previousBurstButtons)
        {
            button.gameObject.SetActive(false);
        }

        StartCoroutine(ChangeFillAmountOverTime(BurstIndex, fillDuration));
    }

    public bool FullBurstChk(int index)
    {
        if (BurstIndex > 4)
        {
            BurstBarImage[index].gameObject.SetActive(false);

            if (index == 2)
            {
                for (int i = 0; i < burst2Buttons.Count; i++)
                {
                    burst2Buttons[i].gameObject.SetActive(false);
                }
            }
            else if (index == 3)
            {
                for (int i = 0; i < burst3Buttons.Count; i++)
                {
                    burst3Buttons[i].gameObject.SetActive(false);
                }
            }

            for (int i = 1; i < BurstBarImage.Count; i++)
            {
                BurstBarImage[i].fillAmount = 1f;
            }

            // 여기서 fullburst 없애고 0으로 초기화 하면서 실행
            BurstBarImage[0].gameObject.SetActive(true);
            BurstBarImage[0].fillAmount = 0;
            return true;
        }
        return false;
    }

    IEnumerator ChangeFillAmountOverTime(int index, float time)
    {
        float elapsedTime = 0f;
        float startValue = 1f;
        float endValue = 0f;

        isButtonClicked = false;

        while (elapsedTime < time)
        {
            if (isButtonClicked) yield break;

            elapsedTime += Time.deltaTime;
            float newFillAmount = Mathf.Lerp(startValue, endValue, elapsedTime / time);
            BurstBarImage[index].fillAmount = newFillAmount;
            yield return null;
        }

        // fillAmount를 0으로 설정
        BurstBarImage[index].fillAmount = endValue;
        BurstIndex = 5;
    }
}
