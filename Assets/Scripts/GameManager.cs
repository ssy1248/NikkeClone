using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//����ڵ�
public class TypeChart
{
    private static Dictionary<AttributeCodes, Dictionary<AttributeCodes, float>> typeChart;

    static TypeChart()
    {
        typeChart = new Dictionary<AttributeCodes, Dictionary<AttributeCodes, float>>
        {
            {AttributeCodes.Fire, new Dictionary<AttributeCodes, float> //�ۿ� �ڵ�
                {
                    {AttributeCodes.Wind, 10f },
                    {AttributeCodes.Water, -10f },
                } 
            },
            {AttributeCodes.Wind, new Dictionary<AttributeCodes, float> //ǳ�� �ڵ�
                {
                    {AttributeCodes.Metal, 10f },
                    {AttributeCodes.Fire, -10f },
                }               
            },
            {AttributeCodes.Metal, new Dictionary<AttributeCodes, float> //ö�� �ڵ�
                {
                    {AttributeCodes.Electro, 10f },
                    {AttributeCodes.Wind, -10f },
                }
            },
            {AttributeCodes.Electro, new Dictionary<AttributeCodes, float> //���� �ڵ�
                {
                    {AttributeCodes.Water, 10f },
                    {AttributeCodes.Metal, -10f },
                }
            },
            {AttributeCodes.Water, new Dictionary<AttributeCodes, float> //���� �ڵ�
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
    public Button[] buttons; // ��ư �迭

    public List<Player> players = new List<Player>();

    //���� Ȱ��ȭ�� �÷��̾�
    [SerializeField]
    public Player activePlayer;

    // �� ���� �ڵ忡���� ���� originalHeight, originalPos�� ���������
    //   ���� ��ư�� �������� ���� �� ���� ������ �ʵ���
    //   ��ư���� '�ʱ� ũ��/��ġ'�� �����ϴ� Dictionary�� ����մϴ�.
    private Dictionary<int, float> defaultHeights = new Dictionary<int, float>();
    private Dictionary<int, Vector2> defaultPositions = new Dictionary<int, Vector2>();

    // �ڷ�ƾ ����
    private Dictionary<int, Coroutine> increaseHeightCoroutines = new Dictionary<int, Coroutine>();
    private Dictionary<int, Coroutine> decreaseHeightCoroutines = new Dictionary<int, Coroutine>();

    // �� ��ư�� Ȯ�� ���� üũ
    private Dictionary<int, bool> isButtonIncreased = new Dictionary<int, bool>();

    // ������ Ȱ��ȭ�� ��ư/RectTransform
    private int lastActiveButtonIndex = -1;
    private RectTransform previousButtonRectTransform = null;

    //ĳ������ �ִ� źâ�� �����ϱ� ���� List
    [SerializeField]
    public List<float> MaxAmmos = new List<float>();

    //���� Ȱ��ȭ�� Player�� Index
    public int PlayerIndex;

    //1����Ʈ���� ���� ���� ��
    public float TotalBurst = 0;

    public List<Enemy> EnemyList = new List<Enemy>();

    //���� ���� bool
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

        // ��� ��ư�� �ʱ� ũ��/��ġ�� �����ϰ�, isButtonIncreased�� false�� �ʱ�ȭ
        for (int i = 0; i < buttons.Length; i++)
        {
            RectTransform rt = buttons[i].GetComponent<RectTransform>();
            defaultHeights[i] = rt.sizeDelta.y;
            defaultPositions[i] = rt.anchoredPosition;
            isButtonIncreased[i] = false;

            // �ڷ�ƾ ��ųʸ��� �ʱ�ȭ
            increaseHeightCoroutines[i] = null;
            decreaseHeightCoroutines[i] = null;
        }

        // Player ��� ����
        players.AddRange(FindObjectsOfType<Player>());
        if (players.Count > 0)
        {
            // 0�� �÷��̾� Ȱ��ȭ
            SetActivePlayer(PlayerObjects[0].GetComponent<Player>());

            if (PlayerIndex != -1)
            {
                BtnSelPlayer(PlayerIndex);
            }
        }

        // PlayerStat ���� Ȯ��
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
                Debug.LogError("PlayerStat �� ����� " + (i + 1));
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
        // ����/���� �÷��̾� ��ũ��Ʈ Ȱ��/��Ȱ�� ó��
        foreach (var p in players)
        {
            if (p == player)
            {
                p.enabled = true;  // ���� �÷��̾�
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null) autoPlayer.enabled = false;
            }
            else
            {
                p.enabled = false; // ���� ����
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null) autoPlayer.enabled = true; // ���� �ѱ�
            }
        }

        activePlayer = player;
    }

    public void BtnSelPlayer(int num)
    {
        // 1) ���� ��ư �ڷ�ƾ �ߴ� & ��� ���󺹱�
        if (lastActiveButtonIndex != -1 && lastActiveButtonIndex != num)
        {
            // ���� �ڷ�ƾ �ߴ�
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

            // ���� ��ư RectTransform�� ���� ũ��/��ġ�� ����
            RectTransform oldRect = buttons[lastActiveButtonIndex].GetComponent<RectTransform>();
            oldRect.sizeDelta = new Vector2(oldRect.sizeDelta.x, defaultHeights[lastActiveButtonIndex]);
            oldRect.anchoredPosition = defaultPositions[lastActiveButtonIndex];

            isButtonIncreased[lastActiveButtonIndex] = false;
        }

        // �̹� Ȯ��� ��ư�̸� ����
        if (isButtonIncreased[num])
        {
            Debug.Log("�̹� ������ ��ư�Դϴ�.");
            return;
        }

        // 2) �� ��ư �ڷ�ƾ �ߴ� & ���󺹱� (Ȥ�ö� ��ø�� ���� �� ������)
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

        // ī�޶� Ȱ��ȭ
        ActivateCamera(num);

        // 3) �� ��ư Ȯ�� �ڷ�ƾ ����
        float currentHeight = currentButtonRectTransform.sizeDelta.y;
        float targetHeight = currentHeight + 100f; // 100f��ŭ Ű���
        Vector2 currentPos = currentButtonRectTransform.anchoredPosition;
        Vector2 targetPos = new Vector2(currentPos.x, 0f); // y=0���� �̵�

        increaseHeightCoroutines[num] = StartCoroutine(
            IncreaseButtonHeightAndMove(currentButtonRectTransform, targetHeight, targetPos, 0.5f)
        );

        isButtonIncreased[num] = true;

        // 4) UI �̹���/�ؽ�Ʈ Ȱ��ȭ/��Ȱ�� ����
        SetActiveImageText(false, num);

        // 5) ���� ��ư�� '���� ��ư'���� ����
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
        // ���� ��ư�� �ؽ�Ʈ/�̹��� �ǵ�����
        if (lastActiveButtonIndex != -1 && lastActiveButtonIndex != num)
        {
            SetChildActive(lastActiveButtonIndex, true);
        }

        // ���� ���õ� ��ư �ؽ�Ʈ/�̹��� ��Ȱ��ȭ
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

    //��ưũ�� Ű��� �ڷ�ƾ
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

        // ���� ����
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight);
        rectTransform.anchoredPosition = targetPos;
    }

    //��ưũ�� ���̴� �ڷ�ƾ (�ʿ��ϸ� ���)
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

    // �� �� ��° ���� ��Ŀ����� ���� �ڷ�ƾ�� ������ ��ٸ��� �����Ƿ�
    //   WaitForCoroutineToEnd�� ������� �ʾƵ� �˴ϴ�.
    //   �ʿ� ���ٸ� �����ϼŵ� �����մϴ�.
    IEnumerator WaitForCoroutineToEnd(Coroutine coroutine, System.Action onComplete)
    {
        yield return coroutine;
        onComplete();
    }
}
