using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageType
{
    EliminationStage,//섬멸전
    DefenceStage,//저지전
    StrongholdDefenceStage,//거점방어전
    BossStage,//보스전
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
