using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//우월코드
public class TypeChart
{
    private static Dictionary<AttributeCodes, Dictionary<AttributeCodes, float>> typeChart;

    static TypeChart()
    {
        typeChart = new Dictionary<AttributeCodes, Dictionary<AttributeCodes, float>>
        {
            {AttributeCodes.Fire, new Dictionary<AttributeCodes, float> //작열 코드
                {
                    {AttributeCodes.Wind, 10f },
                    {AttributeCodes.Water, -10f },
                } 
            },
            {AttributeCodes.Wind, new Dictionary<AttributeCodes, float> //풍압 코드
                {
                    {AttributeCodes.Metal, 10f },
                    {AttributeCodes.Fire, -10f },
                }               
            },
            {AttributeCodes.Metal, new Dictionary<AttributeCodes, float> //철갑 코드
                {
                    {AttributeCodes.Electro, 10f },
                    {AttributeCodes.Wind, -10f },
                }
            },
            {AttributeCodes.Electro, new Dictionary<AttributeCodes, float> //전격 코드
                {
                    {AttributeCodes.Water, 10f },
                    {AttributeCodes.Metal, -10f },
                }
            },
            {AttributeCodes.Water, new Dictionary<AttributeCodes, float> //수냉 코드
                {
                    {AttributeCodes.Fire, 10f },
                    {AttributeCodes.Electro, -10f },
                }
            }
        };
    } 
    
    public static float GetEffectiveness(AttributeCodes attackType, AttributeCodes defenceType)
    {
        if(typeChart.ContainsKey(attackType) && typeChart[attackType].ContainsKey(defenceType))
        {
            return typeChart[attackType][defenceType];
        }
        return 0f;
    }
}

public enum PlayerState
{
    None,
    Attack,
    Defence
}

public class GameManager : MonoBehaviour
{
    public List<GameObject> PlayerObjects = new List<GameObject>();
    public List<Player> OneBurstPlayer = new List<Player>();
    public List<Player> TwoBurstPlayer = new List<Player>();
    public List<Player> ThreeBurstPlayer = new List<Player>();
    public Button[] buttons; // 버튼 배열

    public List<Player> players = new List<Player>();

    //현재 활성화된 플레이어
    [SerializeField]
    public Player activePlayer;

    // ▼ 기존 코드에서는 단일 originalHeight, originalPos를 사용했지만
    //   여러 버튼을 연속으로 누를 때 서로 꼬이지 않도록
    //   버튼마다 '초기 크기/위치'를 저장하는 Dictionary를 사용합니다.
    private Dictionary<int, float> defaultHeights = new Dictionary<int, float>();
    private Dictionary<int, Vector2> defaultPositions = new Dictionary<int, Vector2>();

    // 코루틴 추적
    private Dictionary<int, Coroutine> increaseHeightCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, Coroutine> decreaseHeightCoroutines = new Dictionary<int, Coroutine>();

    // 각 버튼의 확장 상태 체크
    private Dictionary<int, bool> isButtonIncreased = new Dictionary<int, bool>();

    // 이전에 활성화된 버튼/RectTransform
    private int lastActiveButtonIndex = -1;
    private RectTransform previousButtonRectTransform = null;

    //캐릭터의 최대 탄창을 저장하기 위한 List
    [SerializeField]
    public List<float> MaxAmmos = new List<float>();

    //현재 활성화된 Player의 Index
    public int PlayerIndex;

    //1버스트까지 가기 위한 값
    public float TotalBurst = 0;

    public List<Enemy> EnemyList = new List<Enemy>();

    //오토 관련 bool
    public bool isAutoBurst;
    public bool isAutoAttack;

    [Header("UI")]
    public GameObject pauseScreen;

    void Start()
    {
        DisableScreens();

        isAutoBurst = false;
        isAutoAttack = false;
        PlayerIndex = 0;

        // 모든 버튼의 초기 크기/위치를 저장하고, isButtonIncreased를 false로 초기화
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform rt = buttons[i].GetComponent<RectTransform>();
            defaultHeights[i] = rt.sizeDelta.y;
            defaultPositions[i] = rt.anchoredPosition;
            isButtonIncreased[i] = false;

            // 코루틴 딕셔너리도 초기화
            increaseHeightCoroutines[i] = null;
            decreaseHeightCoroutines[i] = null;
        }

        // Player 목록 세팅
        players.AddRange(FindObjectsOfType<Player>());
        if (players.Count > 0)
        {
            // 0번 플레이어 활성화
            SetActivePlayer(PlayerObjects[0].GetComponent<Player>());

            if (PlayerIndex != -1)
            {
                BtnSelPlayer(PlayerIndex);
            }
        }

        // PlayerStat 정보 확인
        for (int i = 0; i < PlayerObjects.Count; i++)
        {
            PlayerStat playerStat = PlayerObjects[i].GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                MaxAmmos.Add(playerStat.currentAmmo);
                TotalBurst = 100;

                if (playerStat.currentBurst == BurstType.Burst1)
                {
                    OneBurstPlayer.Add(PlayerObjects[i].GetComponent<Player>());
                }
                else if (playerStat.currentBurst == BurstType.Burst2)
                {
                    TwoBurstPlayer.Add(PlayerObjects[i].GetComponent<Player>());
                }
                else
                {
                    ThreeBurstPlayer.Add(PlayerObjects[i].GetComponent<Player>());
                }
            }
            else
            {
                Debug.LogError("PlayerStat 이 사라짐 " + (i + 1));
            }
        }
    }

    void Update()
    {
        SelPlayer();
    }

    void SelPlayer()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            BtnSelPlayer(0);
            SetActivePlayer(PlayerObjects[0].GetComponent<Player>());
            PlayerIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            BtnSelPlayer(1);
            SetActivePlayer(PlayerObjects[1].GetComponent<Player>());
            PlayerIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            BtnSelPlayer(2);
            SetActivePlayer(PlayerObjects[2].GetComponent<Player>());
            PlayerIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            BtnSelPlayer(3);
            SetActivePlayer(PlayerObjects[3].GetComponent<Player>());
            PlayerIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            BtnSelPlayer(4);
            SetActivePlayer(PlayerObjects[4].GetComponent<Player>());
            PlayerIndex = 4;
        }
    }

    public void SetActivePlayer(Player player)
    {
        // 오토/수동 플레이어 스크립트 활성/비활성 처리
        foreach (var p in players)
        {
            if (p == player)
            {
                p.enabled = true;  // 수동 플레이어
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null) autoPlayer.enabled = false;
            }
            else
            {
                p.enabled = false; // 수동 끄기
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null) autoPlayer.enabled = true; // 오토 켜기
            }
        }

        activePlayer = player;
    }

    public void BtnSelPlayer(int num)
    {
        // 1) 이전 버튼 코루틴 중단 & 즉시 원상복귀
        if (lastActiveButtonIndex != -1 && lastActiveButtonIndex != num)
        {
            // 기존 코루틴 중단
            if (increaseHeightCoroutines[lastActiveButtonIndex] != null)
            {
                StopCoroutine(increaseHeightCoroutines[lastActiveButtonIndex]);
                increaseHeightCoroutines[lastActiveButtonIndex] = null;
            }
            if (decreaseHeightCoroutines[lastActiveButtonIndex] != null)
            {
                StopCoroutine(decreaseHeightCoroutines[lastActiveButtonIndex]);
                decreaseHeightCoroutines[lastActiveButtonIndex] = null;
            }

            // 이전 버튼 RectTransform을 원래 크기/위치로 복구
            RectTransform oldRect = buttons[lastActiveButtonIndex].GetComponent<RectTransform>();
            oldRect.sizeDelta = new Vector2(oldRect.sizeDelta.x, defaultHeights[lastActiveButtonIndex]);
            oldRect.anchoredPosition = defaultPositions[lastActiveButtonIndex];

            isButtonIncreased[lastActiveButtonIndex] = false;
        }

        // 이미 확장된 버튼이면 무시
        if (isButtonIncreased[num])
        {
            Debug.Log("이미 증가된 버튼입니다.");
            return;
        }

        // 2) 새 버튼 코루틴 중단 & 원상복귀 (혹시라도 중첩이 있을 수 있으니)
        if (increaseHeightCoroutines[num] != null)
        {
            StopCoroutine(increaseHeightCoroutines[num]);
            increaseHeightCoroutines[num] = null;
        }
        if (decreaseHeightCoroutines[num] != null)
        {
            StopCoroutine(decreaseHeightCoroutines[num]);
            decreaseHeightCoroutines[num] = null;
        }

        RectTransform currentButtonRectTransform = buttons[num].GetComponent<RectTransform>();

        // 카메라 활성화
        ActivateCamera(num);

        // 3) 새 버튼 확장 코루틴 시작
        float currentHeight = currentButtonRectTransform.sizeDelta.y;
        float targetHeight = currentHeight + 100f; // 100f만큼 키우기
        Vector2 currentPos = currentButtonRectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(currentPos.x, 0f); // y=0으로 이동

        increaseHeightCoroutines[num] = StartCoroutine(
            IncreaseButtonHeightAndMove(currentButtonRectTransform, targetHeight, targetPos, 0.5f)
        );

        isButtonIncreased[num] = true;

        // 4) UI 이미지/텍스트 활성화/비활성 로직
        SetActiveImageText(false, num);

        // 5) 현재 버튼을 '이전 버튼'으로 설정
        previousButtonRectTransform = currentButtonRectTransform;
        lastActiveButtonIndex = num;
    }

    void ActivateCamera(int index)
    {
        for (int i = 0; i < PlayerObjects.Count; i++)
        {
            Transform childCamera = PlayerObjects[i].transform.Find("Camera");
            if (childCamera != null)
            {
                childCamera.gameObject.SetActive(i == index);
            }
        }
    }

    void SetActiveImageText(bool active, int num)
    {
        // 이전 버튼의 텍스트/이미지 되돌리기
        if (lastActiveButtonIndex != -1 && lastActiveButtonIndex != num)
        {
            SetChildActive(lastActiveButtonIndex, true);
        }

        // 새로 선택된 버튼 텍스트/이미지 비활성화
        SetChildActive(num, active);
    }

    private void SetChildActive(int num, bool active)
    {
        RectTransform currentButtonRectTransform = buttons[num].GetComponent<RectTransform>();
        foreach (Transform child in currentButtonRectTransform)
        {
            child.gameObject.SetActive(active);
        }
    }

    void DisableScreens()
    {
        pauseScreen.SetActive(false);
    }

    //버튼크기 키우는 코루틴
    IEnumerator IncreaseButtonHeightAndMove(RectTransform rectTransform,
                                            float targetHeight,
                                            Vector2 targetPos,
                                            float duration)
    {
        float elapsedTime = 0f;
        float initialHeight = rectTransform.sizeDelta.y;
        Vector2 initialPos = rectTransform.anchoredPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float newHeight = Mathf.Lerp(initialHeight, targetHeight, t);
            Vector2 newPos = Vector2.Lerp(initialPos, targetPos, t);

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            rectTransform.anchoredPosition = newPos;
            yield return null;
        }

        // 최종 보정
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight);
        rectTransform.anchoredPosition = targetPos;
    }

    //버튼크기 줄이는 코루틴 (필요하면 사용)
    IEnumerator DecreaseButtonHeight(RectTransform rectTransform,
                                     float targetHeight,
                                     Vector2 targetPos,
                                     float duration)
    {
        float elapsedTime = 0f;
        float initialHeight = rectTransform.sizeDelta.y;
        Vector2 initialPos = rectTransform.anchoredPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            float newHeight = Mathf.Lerp(initialHeight, targetHeight, t);
            Vector2 newPos = Vector2.Lerp(initialPos, targetPos, t);

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            rectTransform.anchoredPosition = newPos;
            yield return null;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight);
        rectTransform.anchoredPosition = targetPos;
    }

    // ▼ 두 번째 접근 방식에서는 이전 코루틴이 끝나길 기다리지 않으므로
    //   WaitForCoroutineToEnd는 사용하지 않아도 됩니다.
    //   필요 없다면 삭제하셔도 무방합니다.
    IEnumerator WaitForCoroutineToEnd(Coroutine coroutine, System.Action onComplete)
    {
        yield return coroutine;
        onComplete();
    }
}
