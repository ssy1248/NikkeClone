using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public StageInformation stageInformation;

    public StageType currentStageType;
    public int currentStageTime;

    void Awake()
    {
        currentStageType = stageInformation.StageTypes;
        currentStageTime = stageInformation.StageTime;
    }
}
