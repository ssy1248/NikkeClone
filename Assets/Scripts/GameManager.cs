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

public enum GameState
{
    GamePlay,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public List<GameObject> PlayerObjects = new List<GameObject>();
    public List<Player> OneBurstPlayer = new List<Player>();
    public List<Player> TwoBurstPlayer = new List<Player>();
    public List<Player> ThreeBurstPlayer = new List<Player>();
    public Button[] buttons; // 버튼 배열

    public List<Player> players = new List<Player>();

    //현재 활성화된 플레이어 오브젝트
    [SerializeField]
    private Player activePlayer;

    private RectTransform previousButtonRectTransform = null; // 이전 버튼의 RectTransform
    private float originalHeight = 0f; // 원래 높이 저장
    private Vector2 originalPos; // 원래 위치 저장
    private Dictionary<int, Coroutine> increaseHeightCoroutines = new Dictionary<int, Coroutine>(); // 각 버튼 높이 증가 코루틴
    private Dictionary<int, Coroutine> decreaseHeightCoroutines = new Dictionary<int, Coroutine>(); // 각 버튼 높이 축소 코루틴
    private int lastActiveButtonIndex = -1; // 이전에 눌린 버튼의 인덱스
    private Dictionary<int, bool> isButtonIncreased = new Dictionary<int, bool>(); // 각 버튼의 증가 상태를 확인하는 변수

    //캐릭터의 최대 탄창을 저장하기 위한 List
    [SerializeField]
    public List<float> MaxAmmos = new List<float>();
    //현재 활성화된 Player를 알기 위한 Index값
    public int PlayerIndex;

    //1버스트까지 가기 위한 값
    public float TotalBurst = 0;

    public List<Enemy> EnemyList = new List<Enemy>();

    //private List<GameObject> exceptionChildObjects = new List<GameObject>();

    //버튼 Auto 를 만들면 모든 플레이어 Player함수를 비활성화 시키고 AutoPlayer함수를 활성화 SelPlayer함수가 실행되더라도 Player함수가 활성화되는것을 예외처리
    //버튼 일시정지 모드
    //버튼 AutoBurst를 만들어서 클릭을 할 시 생성되는 Burst1Image의 리스트의 제일 앞을 실행시키기 만약 제일 앞이 쿨타임이 걸려있다면 ++ 을 진행하여 발동 만약 모든 리스트가 쿨타임이 진행중이라면 아무행동도 안함
    //크기 줄이는 코루틴이 발생하여서 크기가 커져도 hpbar와 defencebar, code SetActive 비활성화 예외 처리는 완료 그러나 위치가 이상하고 처음 시작하는 플레이어는 싹다 setActive가 false로 되어잇음

    void Start()
    {
        //초기화
        PlayerIndex = 0;

        for (int i = 0; i < buttons.Length; i++)
        {
            isButtonIncreased[i] = false;
        }

        players.AddRange(FindObjectsOfType<Player>());
        if (players.Count > 0)
        {
            SetActivePlayer(PlayerObjects[0].GetComponent<Player>());

            if (PlayerIndex != -1)
            {
                BtnSelPlayer(PlayerIndex);
            }
        }

        for (int i = 0; i < PlayerObjects.Count; i++)
        {
            PlayerStat playerStat = PlayerObjects[i].GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                MaxAmmos.Add(playerStat.currentAmmo);
                //임의 값 
                TotalBurst = 100;
                //TotalBurst += (playerStat.currentDamage * 5);

                if (playerStat.currentBurst == BurstType.Burst1)
                {
                    OneBurstPlayer.Add(PlayerObjects[i].GetComponent<Player>());
                }
                else if(playerStat.currentBurst == BurstType.Burst2)
                {
                    TwoBurstPlayer.Add(PlayerObjects[i].GetComponent<Player>());
                }
                else
                    ThreeBurstPlayer.Add(PlayerObjects[i].GetComponent<Player>());
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
        foreach (var p in players)
        {
            //p.enabled = false;
            if (p == player)
            {
                p.enabled = true;  // 활성화할 플레이어는 Player 스크립트를 활성화하고
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null)
                {
                    autoPlayer.enabled = false;  // AutoPlayer 스크립트를 비활성화
                }
            }
            else
            {
                p.enabled = false;  // 다른 플레이어는 Player 스크립트를 비활성화하고
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null)
                {
                    autoPlayer.enabled = true;  // AutoPlayer 스크립트를 활성화
                }
            }
        }

        player.enabled = true;
        activePlayer = player;
    }

    public void BtnSelPlayer(int num)
    {
        SetActivePlayer(PlayerObjects[num].GetComponent<Player>());

        // 이전 버튼의 높이를 원래대로 줄이기
        if (previousButtonRectTransform != null && previousButtonRectTransform != buttons[num].GetComponent<RectTransform>())
        {
            if (decreaseHeightCoroutines.ContainsKey(lastActiveButtonIndex) && decreaseHeightCoroutines[lastActiveButtonIndex] != null)
            {
                // 기존 코루틴이 끝날 때까지 기다리기
                StartCoroutine(WaitForCoroutineToEnd(decreaseHeightCoroutines[lastActiveButtonIndex], () =>
                {
                    decreaseHeightCoroutines[lastActiveButtonIndex] = StartCoroutine(DecreaseButtonHeight(previousButtonRectTransform, originalHeight, originalPos, 0.5f));
                }));
            }
            else
            {
                // 새로 줄이기 코루틴 시작
                decreaseHeightCoroutines[lastActiveButtonIndex] = StartCoroutine(DecreaseButtonHeight(previousButtonRectTransform, originalHeight, originalPos, 0.5f));
            }
            isButtonIncreased[lastActiveButtonIndex] = false;
        }

        // 새로운 버튼의 RectTransform 가져오기
        RectTransform currentButtonRectTransform = buttons[num].GetComponent<RectTransform>();

        // 이미 증가된 버튼이면 크기를 증가시키지 않음
        if (isButtonIncreased[num])
        {
            Debug.Log("이미 증가된 버튼입니다.");
            return;
        }

        originalHeight = currentButtonRectTransform.sizeDelta.y; // 현재 높이 저장
        originalPos = currentButtonRectTransform.anchoredPosition; // 현재 위치 저장

        // 카메라 활성화
        ActivateCamera(num);

        // 버튼 높이 증가 코루틴 시작
        if (increaseHeightCoroutines.ContainsKey(num) && increaseHeightCoroutines[num] != null)
        {
            // 기존 코루틴이 끝날 때까지 기다리기
            StartCoroutine(WaitForCoroutineToEnd(increaseHeightCoroutines[num], () =>
            {
                increaseHeightCoroutines[num] = StartCoroutine(IncreaseButtonHeightAndMove(currentButtonRectTransform, 100f, new Vector2(originalPos.x, 0), 0.5f));
            }));
        }
        else
        {
            increaseHeightCoroutines[num] = StartCoroutine(IncreaseButtonHeightAndMove(currentButtonRectTransform, 100f, new Vector2(originalPos.x, 0), 0.5f));
        }
        isButtonIncreased[num] = true; // 버튼이 증가되었음을 표시

        // 현재 버튼을 이전 버튼으로 설정
        previousButtonRectTransform = currentButtonRectTransform;

        // 이미지와 텍스트 활성화/비활성화
        SetActiveImageText(false, num);
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
        if (lastActiveButtonIndex != -1 && lastActiveButtonIndex != num)
        {
            // 이전에 비활성화된 이미지와 텍스트를 다시 활성화
            SetChildActive(lastActiveButtonIndex, true);
        }

        // 현재 버튼의 이미지와 텍스트를 활성화/비활성화
        SetChildActive(num, active);

        // 마지막으로 활성화한 버튼 인덱스를 갱신
        if (!active)
        {
            lastActiveButtonIndex = num;
        }
    }

    private void SetChildActive(int num, bool active)
    {
        RectTransform currentButtonRectTransform = buttons[num].GetComponent<RectTransform>();
        foreach (Transform child in currentButtonRectTransform)
        {
            child.gameObject.SetActive(active);
        }
    }

    //버튼크기 키우는 코루틴
    IEnumerator IncreaseButtonHeightAndMove(RectTransform rectTransform, float heightIncrease,
        Vector2 targetPos, float duration)
    {
        float elapsedTime = 0f;
        float initialHeight = rectTransform.sizeDelta.y;
        float targetHeight = initialHeight + heightIncrease;
        Vector2 initialPos = rectTransform.anchoredPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newHeight = Mathf.Lerp(initialHeight, targetHeight, elapsedTime / duration);
            Vector2 newPos = Vector2.Lerp(initialPos, targetPos, elapsedTime / duration);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            rectTransform.anchoredPosition = newPos;
            yield return null;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight); // 최종 크기 설정
        rectTransform.anchoredPosition = targetPos; // 최종 위치 설정
    }

    //버튼크기 줄이는 코루틴
    IEnumerator DecreaseButtonHeight(RectTransform rectTransform, float targetHeight, 
        Vector2 targetPos, float duration)
    {
        float elapsedTime = 0f;
        float initialHeight = rectTransform.sizeDelta.y;
        Vector2 initialPos = rectTransform.anchoredPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newHeight = Mathf.Lerp(initialHeight, targetHeight, elapsedTime / duration);
            Vector2 newPos = Vector2.Lerp(initialPos, targetPos, elapsedTime / duration);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            rectTransform.anchoredPosition = newPos;
            yield return null;
        }

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight); // 최종 크기 설정
        rectTransform.anchoredPosition = targetPos; // 최종 위치 설정
    }

    //버튼이 줄어들거나 커지는 시간을 기다리게 하는 코루틴
    IEnumerator WaitForCoroutineToEnd(Coroutine coroutine, System.Action onComplete)
    {
        yield return coroutine;
        onComplete();
    }
}
