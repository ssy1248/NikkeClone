using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    EliminationStage,//������
    DefenceStage,//������
    StrongholdDefenceStage,//���������
    BossStage,//������
}

[CreateAssetMenu(fileName = "StageInformationScriptableObject", menuName = "ScriptableObject/Stage")]
public class StageInformation : ScriptableObject
{
    [SerializeField]
    StageType stageType;
    public StageType StageTypes { get => stageType; set => stageType = value; }

    [SerializeField]
    int stageTime;
    public int StageTime { get => stageTime; set => stageTime = value; }
}
