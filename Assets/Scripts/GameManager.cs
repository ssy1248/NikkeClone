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
    public Button[] buttons; // ��ư �迭

    public List<Player> players = new List<Player>();

    //���� Ȱ��ȭ�� �÷��̾� ������Ʈ
    [SerializeField]
    private Player activePlayer;

    private RectTransform previousButtonRectTransform = null; // ���� ��ư�� RectTransform
    private float originalHeight = 0f; // ���� ���� ����
    private Vector2 originalPos; // ���� ��ġ ����
    private Dictionary<int, Coroutine> increaseHeightCoroutines = new Dictionary<int, Coroutine>(); // �� ��ư ���� ���� �ڷ�ƾ
    private Dictionary<int, Coroutine> decreaseHeightCoroutines = new Dictionary<int, Coroutine>(); // �� ��ư ���� ��� �ڷ�ƾ
    private int lastActiveButtonIndex = -1; // ������ ���� ��ư�� �ε���
    private Dictionary<int, bool> isButtonIncreased = new Dictionary<int, bool>(); // �� ��ư�� ���� ���¸� Ȯ���ϴ� ����

    //ĳ������ �ִ� źâ�� �����ϱ� ���� List
    [SerializeField]
    public List<float> MaxAmmos = new List<float>();
    //���� Ȱ��ȭ�� Player�� �˱� ���� Index��
    public int PlayerIndex;

    //1����Ʈ���� ���� ���� ��
    public float TotalBurst = 0;

    public List<Enemy> EnemyList = new List<Enemy>();

    //private List<GameObject> exceptionChildObjects = new List<GameObject>();

    //��ư Auto �� ����� ��� �÷��̾� Player�Լ��� ��Ȱ��ȭ ��Ű�� AutoPlayer�Լ��� Ȱ��ȭ SelPlayer�Լ��� ����Ǵ��� Player�Լ��� Ȱ��ȭ�Ǵ°��� ����ó��
    //��ư �Ͻ����� ���
    //��ư AutoBurst�� ���� Ŭ���� �� �� �����Ǵ� Burst1Image�� ����Ʈ�� ���� ���� �����Ű�� ���� ���� ���� ��Ÿ���� �ɷ��ִٸ� ++ �� �����Ͽ� �ߵ� ���� ��� ����Ʈ�� ��Ÿ���� �������̶�� �ƹ��ൿ�� ����
    //ũ�� ���̴� �ڷ�ƾ�� �߻��Ͽ��� ũ�Ⱑ Ŀ���� hpbar�� defencebar, code SetActive ��Ȱ��ȭ ���� ó���� �Ϸ� �׷��� ��ġ�� �̻��ϰ� ó�� �����ϴ� �÷��̾�� �ϴ� setActive�� false�� �Ǿ�����

    void Start()
    {
        //�ʱ�ȭ
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
                //���� �� 
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
        foreach (var p in players)
        {
            //p.enabled = false;
            if (p == player)
            {
                p.enabled = true;  // Ȱ��ȭ�� �÷��̾�� Player ��ũ��Ʈ�� Ȱ��ȭ�ϰ�
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null)
                {
                    autoPlayer.enabled = false;  // AutoPlayer ��ũ��Ʈ�� ��Ȱ��ȭ
                }
            }
            else
            {
                p.enabled = false;  // �ٸ� �÷��̾�� Player ��ũ��Ʈ�� ��Ȱ��ȭ�ϰ�
                AutoPlayer autoPlayer = p.GetComponent<AutoPlayer>();
                if (autoPlayer != null)
                {
                    autoPlayer.enabled = true;  // AutoPlayer ��ũ��Ʈ�� Ȱ��ȭ
                }
            }
        }

        player.enabled = true;
        activePlayer = player;
    }

    public void BtnSelPlayer(int num)
    {
        SetActivePlayer(PlayerObjects[num].GetComponent<Player>());

        // ���� ��ư�� ���̸� ������� ���̱�
        if (previousButtonRectTransform != null && previousButtonRectTransform != buttons[num].GetComponent<RectTransform>())
        {
            if (decreaseHeightCoroutines.ContainsKey(lastActiveButtonIndex) && decreaseHeightCoroutines[lastActiveButtonIndex] != null)
            {
                // ���� �ڷ�ƾ�� ���� ������ ��ٸ���
                StartCoroutine(WaitForCoroutineToEnd(decreaseHeightCoroutines[lastActiveButtonIndex], () =>
                {
                    decreaseHeightCoroutines[lastActiveButtonIndex] = StartCoroutine(DecreaseButtonHeight(previousButtonRectTransform, originalHeight, originalPos, 0.5f));
                }));
            }
            else
            {
                // ���� ���̱� �ڷ�ƾ ����
                decreaseHeightCoroutines[lastActiveButtonIndex] = StartCoroutine(DecreaseButtonHeight(previousButtonRectTransform, originalHeight, originalPos, 0.5f));
            }
            isButtonIncreased[lastActiveButtonIndex] = false;
        }

        // ���ο� ��ư�� RectTransform ��������
        RectTransform currentButtonRectTransform = buttons[num].GetComponent<RectTransform>();

        // �̹� ������ ��ư�̸� ũ�⸦ ������Ű�� ����
        if (isButtonIncreased[num])
        {
            Debug.Log("�̹� ������ ��ư�Դϴ�.");
            return;
        }

        originalHeight = currentButtonRectTransform.sizeDelta.y; // ���� ���� ����
        originalPos = currentButtonRectTransform.anchoredPosition; // ���� ��ġ ����

        // ī�޶� Ȱ��ȭ
        ActivateCamera(num);

        // ��ư ���� ���� �ڷ�ƾ ����
        if (increaseHeightCoroutines.ContainsKey(num) && increaseHeightCoroutines[num] != null)
        {
            // ���� �ڷ�ƾ�� ���� ������ ��ٸ���
            StartCoroutine(WaitForCoroutineToEnd(increaseHeightCoroutines[num], () =>
            {
                increaseHeightCoroutines[num] = StartCoroutine(IncreaseButtonHeightAndMove(currentButtonRectTransform, 100f, new Vector2(originalPos.x, 0), 0.5f));
            }));
        }
        else
        {
            increaseHeightCoroutines[num] = StartCoroutine(IncreaseButtonHeightAndMove(currentButtonRectTransform, 100f, new Vector2(originalPos.x, 0), 0.5f));
        }
        isButtonIncreased[num] = true; // ��ư�� �����Ǿ����� ǥ��

        // ���� ��ư�� ���� ��ư���� ����
        previousButtonRectTransform = currentButtonRectTransform;

        // �̹����� �ؽ�Ʈ Ȱ��ȭ/��Ȱ��ȭ
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
            // ������ ��Ȱ��ȭ�� �̹����� �ؽ�Ʈ�� �ٽ� Ȱ��ȭ
            SetChildActive(lastActiveButtonIndex, true);
        }

        // ���� ��ư�� �̹����� �ؽ�Ʈ�� Ȱ��ȭ/��Ȱ��ȭ
        SetChildActive(num, active);

        // ���������� Ȱ��ȭ�� ��ư �ε����� ����
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

    //��ưũ�� Ű��� �ڷ�ƾ
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

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight); // ���� ũ�� ����
        rectTransform.anchoredPosition = targetPos; // ���� ��ġ ����
    }

    //��ưũ�� ���̴� �ڷ�ƾ
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

        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, targetHeight); // ���� ũ�� ����
        rectTransform.anchoredPosition = targetPos; // ���� ��ġ ����
    }

    //��ư�� �پ��ų� Ŀ���� �ð��� ��ٸ��� �ϴ� �ڷ�ƾ
    IEnumerator WaitForCoroutineToEnd(Coroutine coroutine, System.Action onComplete)
    {
        yield return coroutine;
        onComplete();
    }
}
