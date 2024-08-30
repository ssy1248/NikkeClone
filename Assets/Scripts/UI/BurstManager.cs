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

    // 각 버스트 상태를 나타내는 변수
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
        // 전체 버스트 체크 후 버스트 실행
        if (FullBurstChk(currentIndex) == false)
        {
            Burst();
        }
        else
        {
            // 전체 버스트가 완료되면 인덱스 초기화
            BurstIndex = 0;
        }
    }

    public void BtnBurstClick(Button clickedButton)
    {
        isButtonClicked = true; // 버튼 클릭 상태 설정
        ButtonBase btb = clickedButton.GetComponentInChildren<ButtonBase>();

        if (btb.isCoolTime) // 쿨타임 중이면 리턴
            return;

        burstCooldownManager.BurstCoolTime(clickedButton); // 쿨타임 실행

        BurstIndex++; // 버스트 인덱스 증가
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
        GameObject spawn = GameObject.Find("Burst").gameObject; // 버스트 오브젝트 찾기
        Vector3 spawnPosition = spawn.transform.position; // 스폰 위치

        // 각 버스트 상태에 따라 처리
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

        // 버튼 클릭 상태 초기화
        isButtonClicked = false;
    }

    void HandleBurst(List<Player> players, List<Button> burstButtons, Button burstImage, GameObject spawn, Vector3 spawnPosition)
    {
        for (int i = 0; i < players.Count; i++)
        {
            // 버튼 간격 조정
            if (i > 0) spawnPosition.y -= 110 * i; 

            // 버튼이 생성이 안되있다면
            if (burstButtons.Count <= i)
            {
                // 새로운 버스트 버튼 생성
                Button newBurstIButton = Instantiate(burstImage, spawnPosition, Quaternion.identity, spawn.transform);
                newBurstIButton.onClick.AddListener(() => BtnBurstClick(newBurstIButton)); // 클릭 이벤트 등록
                newBurstIButton.transform.GetComponentInChildren<TextMeshProUGUI>().text = players[i].name; // 버튼 텍스트 설정
                burstButtons.Add(newBurstIButton); // 버튼 리스트에 추가
            }
            // 버튼이 생성이 되있다면 기존 버튼 활성화
            else
            {
                burstButtons[i].gameObject.SetActive(true); 
            }
        }
    }

    void HandleBurstTransition(List<Button> previousBurstButtons, List<Player> currentBurstPlayers, List<Button> currentBurstButtons,
        Button currentBurstImage, GameObject spawn, Vector3 spawnPosition, float fillDuration)
    {
        BurstBarImage[BurstIndex - 1].gameObject.SetActive(false); // 이전 버스트 바 비활성화
        BurstBarImage[BurstIndex].gameObject.SetActive(true); // 현재 버스트 바 활성화

        foreach (Button button in previousBurstButtons)
        {
            button.gameObject.SetActive(false); // 이전 버튼 비활성화
        }

        HandleBurst(currentBurstPlayers, currentBurstButtons, currentBurstImage, spawn, spawnPosition); // 현재 버스트 처리
        StartCoroutine(ChangeFillAmountOverTime(BurstIndex, fillDuration)); // fillAmount 변화 코루틴 시작
    }

    void HandleFullBurst(List<Button> previousBurstButtons, float fillDuration)
    {
        BurstBarImage[BurstIndex - 1].gameObject.SetActive(false); // 이전 버스트 바 비활성화
        BurstBarImage[BurstIndex].gameObject.SetActive(true); // 현재 버스트 바 활성화

        foreach (Button button in previousBurstButtons)
        {
            button.gameObject.SetActive(false); // 이전 버튼 비활성화
        }

        StartCoroutine(ChangeFillAmountOverTime(BurstIndex, fillDuration)); // fillAmount 변화 코루틴 시작
    }

    public bool FullBurstChk(int index)
    {
        if (BurstIndex > 4) // 전체 버스트 체크
        {
            BurstBarImage[index].gameObject.SetActive(false); // 현재 버스트 바 비활성화

            // 각 버튼 리스트 비활성화
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

            // 모든 버스트 바 fillAmount를 1로 설정
            for (int i = 1; i < BurstBarImage.Count; i++)
            {
                BurstBarImage[i].fillAmount = 1f;
            }

            // 전체 버스트 초기화
            BurstBarImage[0].gameObject.SetActive(true);
            BurstBarImage[0].fillAmount = 0;
            return true; // 전체 버스트 완료
        }
        return false; // 전체 버스트 미완료
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
