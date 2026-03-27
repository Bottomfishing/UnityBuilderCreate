using System;
using UnityEngine;

[Serializable]
public class TutorialStep
{
    [TextArea(2, 5)]
    public string message;
    public Vector2 pointerPosition;
    public bool showPointer;
    public float delay = 0.5f;
    public TutorialHighlightType highlightType;
    
    [Header("箭头设置（可选，覆盖全局）")]
    public Vector2 pointerOffsetOverride = Vector2.zero;
    public float pointerScaleOverride = 1f;
    
    [Header("箭头旋转设置（可选，覆盖全局）")]
    public float pointerRotationOverride = 0f;
    public float pointerRotateSpeedOverride = 0f;
}

public enum TutorialHighlightType
{
    None,
    SpawnPoint,
    EndPoint,
    PauseButton,
    SpeedButton,
    SkillButton,
    TowerSlot
}
