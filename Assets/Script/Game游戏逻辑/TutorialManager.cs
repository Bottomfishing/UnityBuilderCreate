using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    
    [Header("教学步骤")]
    public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
    
    [Header("教学设置")]
    public string tutorialCompletedKey = "TutorialCompleted";
    public bool forceShowTutorial = false;
    
    [Header("引用")]
    public GameObject tutorialUIPrefab;
    public GameObject pointerPrefab;
    
    [Header("箭头设置")]
    public Vector2 pointerOffset = new Vector2(0, 80);
    public float pointerScale = 1f;
    
    [Header("箭头旋转设置")]
    public float pointerRotation = 0f;
    public float pointerRotateSpeed = 0f;
    
    private int currentStep = 0;
    private bool isTutorialActive = false;
    private GameObject tutorialUIInstance;
    private GameObject pointerInstance;
    private TutorialUI tutorialUI;
    private GameObject spawnPoint;
    private GameObject endPoint;
    private GameObject pauseButton;
    private GameObject speedButton;
    private GameObject skillButton;
    
    public bool IsTutorialActive => isTutorialActive;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void Start()
    {
        FindReferences();
        
        if (ShouldShowTutorial())
        {
            StartCoroutine(StartTutorialWithDelay());
        }
    }
    
    private void FindReferences()
    {
        spawnPoint = GameObject.Find("SpawnPoint");
        endPoint = GameObject.Find("EndPoint");
        
        pauseButton = GameObject.Find("PauseButton");
        speedButton = GameObject.Find("SpeedButton");
        skillButton = GameObject.Find("SkillButton");
        
        if (pauseButton == null)
        {
            pauseButton = GameObject.Find("暂停");
        }
        if (speedButton == null)
        {
            speedButton = GameObject.Find("加速");
        }
        if (skillButton == null)
        {
            skillButton = GameObject.Find("技能");
        }
    }
    
    private bool ShouldShowTutorial()
    {
        if (forceShowTutorial) return true;
        
        bool alwaysShow = PlayerPrefs.GetInt("AlwaysShowTutorial", 0) == 1;
        bool isFirstTime = PlayerPrefs.GetInt(tutorialCompletedKey, 0) == 0;
        
        return alwaysShow || isFirstTime;
    }
    
    private IEnumerator StartTutorialWithDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        StartTutorial();
    }
    
    public void StartTutorial()
    {
        if (tutorialSteps.Count == 0)
        {
            CreateDefaultTutorialSteps();
        }
        
        isTutorialActive = true;
        currentStep = 0;
        
        PauseGame();
        CreateTutorialUI();
        ShowCurrentStep();
    }
    
    private void CreateDefaultTutorialSteps()
    {
        tutorialSteps = new List<TutorialStep>
        {
            new TutorialStep 
            { 
                message = "欢迎来到游戏！\n这是新手教学，点击任意位置继续",
                showPointer = false,
                highlightType = TutorialHighlightType.None
            },
            new TutorialStep 
            { 
                message = "僵尸会从这里出现！\n小心防守！",
                showPointer = true,
                highlightType = TutorialHighlightType.SpawnPoint
            },
            new TutorialStep 
            { 
                message = "如果僵尸到达这里，\n你会失去生命值！",
                showPointer = true,
                highlightType = TutorialHighlightType.EndPoint
            },
            new TutorialStep 
            { 
                message = "点击这里可以暂停游戏",
                showPointer = true,
                highlightType = TutorialHighlightType.PauseButton
            },
            new TutorialStep 
            { 
                message = "点击这里可以加速游戏\n最高2倍速！",
                showPointer = true,
                highlightType = TutorialHighlightType.SpeedButton
            },
            new TutorialStep 
            { 
                message = "点击这里使用技能\n消灭僵尸！",
                showPointer = true,
                highlightType = TutorialHighlightType.SkillButton
            },
            new TutorialStep 
            { 
                message = "教学完成！\n祝你好运！",
                showPointer = false,
                highlightType = TutorialHighlightType.None
            }
        };
    }
    
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
    
    private void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    
    private void CreateTutorialUI()
    {
        if (tutorialUIInstance != null)
        {
            Destroy(tutorialUIInstance);
        }
        
        if (tutorialUIPrefab != null)
        {
            tutorialUIInstance = Instantiate(tutorialUIPrefab);
            
            GameObject canvasObj = FindCanvas();
            if (canvasObj != null)
            {
                tutorialUIInstance.transform.SetParent(canvasObj.transform, false);
            }
            
            RectTransform rectTransform = tutorialUIInstance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
            }
            
            tutorialUIInstance.SetActive(true);
            
            for (int i = 0; i < tutorialUIInstance.transform.childCount; i++)
            {
                Transform child = tutorialUIInstance.transform.GetChild(i);
                child.gameObject.SetActive(true);
            }
            
            tutorialUI = tutorialUIInstance.GetComponent<TutorialUI>();
        }
        else
        {
            Debug.LogError("[TutorialManager] Could not find TutorialUI prefab!");
            CreateSimpleTutorialUI();
            return;
        }
        
        if (pointerPrefab == null)
        {
            pointerPrefab = Resources.Load<GameObject>("TutorialPointer");
            if (pointerPrefab == null)
            {
                pointerPrefab = LoadPrefabByName("TutorialPointer");
            }
        }
        
        if (pointerPrefab != null)
        {
            pointerInstance = Instantiate(pointerPrefab);
            
            GameObject canvasObj = FindCanvas();
            if (canvasObj != null)
            {
                pointerInstance.transform.SetParent(canvasObj.transform, false);
            }
            
            RectTransform pointerRect = pointerInstance.GetComponent<RectTransform>();
            if (pointerRect == null)
            {
                pointerRect = pointerInstance.AddComponent<RectTransform>();
            }
            pointerRect.sizeDelta = new Vector2(50 * pointerScale, 50 * pointerScale);
            pointerInstance.transform.localScale = Vector3.one * pointerScale;
            
            pointerInstance.SetActive(false);
        }
    }
    
    private GameObject FindCanvas()
    {
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj == null)
        {
            canvasObj = GameObject.Find("UI Canvas");
        }
        if (canvasObj == null)
        {
            canvasObj = GameObject.Find("UICanvas");
        }
        if (canvasObj == null)
        {
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            if (canvases.Length > 0)
            {
                canvasObj = canvases[0].gameObject;
            }
        }
        return canvasObj;
    }
    
    private GameObject LoadPrefabByName(string prefabName)
    {
#if UNITY_EDITOR
        string[] guids = UnityEditor.AssetDatabase.FindAssets(prefabName + " t:Prefab");
        if (guids.Length > 0)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
            return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
#endif
        return null;
    }
    
    private void CreateSimpleTutorialUI()
    {
        GameObject canvasObj = FindCanvas();
        
        if (canvasObj == null)
        {
            Debug.LogError("[TutorialManager] No Canvas found!");
            return;
        }
        
        tutorialUIInstance = new GameObject("TutorialUI");
        tutorialUIInstance.transform.SetParent(canvasObj.transform, false);
        
        RectTransform rectTransform = tutorialUIInstance.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        CanvasGroup canvasGroup = tutorialUIInstance.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        Image bgImage = tutorialUIInstance.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.7f);
        bgImage.raycastTarget = true;
        
        tutorialUI = tutorialUIInstance.AddComponent<TutorialUI>();
        tutorialUI.background = tutorialUIInstance;
        tutorialUI.canvasGroup = canvasGroup;
        
        CreateSimpleUIElements();
    }
    
    private void CreateSimpleUIElements()
    {
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(tutorialUIInstance.transform, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(600, 300);
        panelRect.anchoredPosition = Vector2.zero;
        
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = Color.white;
        
        GameObject messageObj = new GameObject("MessageText");
        messageObj.transform.SetParent(panel.transform, false);
        
        RectTransform messageRect = messageObj.AddComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0, 0.5f);
        messageRect.anchorMax = new Vector2(1, 0.5f);
        messageRect.sizeDelta = new Vector2(-40, 150);
        messageRect.anchoredPosition = new Vector2(20, 30);
        
        Text messageText = messageObj.AddComponent<Text>();
        messageText.fontSize = 24;
        messageText.alignment = TextAnchor.MiddleCenter;
        messageText.color = Color.black;
        
        GameObject stepObj = new GameObject("StepText");
        stepObj.transform.SetParent(panel.transform, false);
        
        RectTransform stepRect = stepObj.AddComponent<RectTransform>();
        stepRect.anchorMin = new Vector2(0.5f, 0);
        stepRect.anchorMax = new Vector2(0.5f, 0);
        stepRect.sizeDelta = new Vector2(200, 40);
        stepRect.anchoredPosition = new Vector2(0, 80);
        
        Text stepText = stepObj.AddComponent<Text>();
        stepText.fontSize = 18;
        stepText.alignment = TextAnchor.MiddleCenter;
        stepText.color = Color.gray;
        
        GameObject nextBtnObj = new GameObject("NextButton");
        nextBtnObj.transform.SetParent(panel.transform, false);
        
        RectTransform nextBtnRect = nextBtnObj.AddComponent<RectTransform>();
        nextBtnRect.anchorMin = new Vector2(1, 0);
        nextBtnRect.anchorMax = new Vector2(1, 0);
        nextBtnRect.sizeDelta = new Vector2(120, 50);
        nextBtnRect.anchoredPosition = new Vector2(-80, 20);
        
        Button nextButton = nextBtnObj.AddComponent<Button>();
        Image nextBtnImage = nextBtnObj.AddComponent<Image>();
        nextBtnImage.color = new Color(0.2f, 0.6f, 0.9f);
        
        GameObject nextTextObj = new GameObject("Text");
        nextTextObj.transform.SetParent(nextBtnObj.transform, false);
        
        RectTransform nextTextRect = nextTextObj.AddComponent<RectTransform>();
        nextTextRect.anchorMin = Vector2.zero;
        nextTextRect.anchorMax = Vector2.one;
        nextTextRect.sizeDelta = Vector2.zero;
        
        Text nextBtnText = nextTextObj.AddComponent<Text>();
        nextBtnText.fontSize = 20;
        nextBtnText.alignment = TextAnchor.MiddleCenter;
        nextBtnText.color = Color.white;
        nextBtnText.text = "下一步";
        
        GameObject skipBtnObj = new GameObject("SkipButton");
        skipBtnObj.transform.SetParent(panel.transform, false);
        
        RectTransform skipBtnRect = skipBtnObj.AddComponent<RectTransform>();
        skipBtnRect.anchorMin = new Vector2(0, 0);
        skipBtnRect.anchorMax = new Vector2(0, 0);
        skipBtnRect.sizeDelta = new Vector2(120, 50);
        skipBtnRect.anchoredPosition = new Vector2(80, 20);
        
        Button skipButton = skipBtnObj.AddComponent<Button>();
        Image skipBtnImage = skipBtnObj.AddComponent<Image>();
        skipBtnImage.color = new Color(0.7f, 0.7f, 0.7f);
        
        GameObject skipTextObj = new GameObject("Text");
        skipTextObj.transform.SetParent(skipBtnObj.transform, false);
        
        RectTransform skipTextRect = skipTextObj.AddComponent<RectTransform>();
        skipTextRect.anchorMin = Vector2.zero;
        skipTextRect.anchorMax = Vector2.one;
        skipTextRect.sizeDelta = Vector2.zero;
        
        Text skipBtnText = skipTextObj.AddComponent<Text>();
        skipBtnText.fontSize = 20;
        skipBtnText.alignment = TextAnchor.MiddleCenter;
        skipBtnText.color = Color.white;
        skipBtnText.text = "跳过";
        
        tutorialUI.messageText = messageText;
        tutorialUI.stepText = stepText;
        tutorialUI.nextButton = nextButton;
        tutorialUI.skipButton = skipButton;
    }
    
    private void ShowCurrentStep()
    {
        if (currentStep >= tutorialSteps.Count)
        {
            EndTutorial();
            return;
        }
        
        TutorialStep step = tutorialSteps[currentStep];
        
        if (tutorialUI == null && tutorialUIInstance != null)
        {
            tutorialUI = tutorialUIInstance.GetComponent<TutorialUI>();
        }
        
        if (tutorialUI != null)
        {
            tutorialUI.ShowMessage(step.message, currentStep + 1, tutorialSteps.Count);
        }
        else
        {
            Debug.LogError("[TutorialManager] tutorialUI is NULL!");
        }
        
        if (step.showPointer && pointerInstance != null)
        {
            Vector2 currentOffset = pointerOffset;
            float currentScale = pointerScale;
            float currentRotation = pointerRotation;
            float currentRotateSpeed = pointerRotateSpeed;
            
            if (step.pointerOffsetOverride != Vector2.zero)
            {
                currentOffset = step.pointerOffsetOverride;
            }
            
            if (step.pointerScaleOverride != 1f)
            {
                currentScale = step.pointerScaleOverride;
                RectTransform pointerRect = pointerInstance.GetComponent<RectTransform>();
                if (pointerRect != null)
                {
                    pointerRect.sizeDelta = new Vector2(50 * currentScale, 50 * currentScale);
                }
                pointerInstance.transform.localScale = Vector3.one * currentScale;
            }
            
            if (step.pointerRotationOverride != 0f)
            {
                currentRotation = step.pointerRotationOverride;
            }
            
            if (step.pointerRotateSpeedOverride != 0f)
            {
                currentRotateSpeed = step.pointerRotateSpeedOverride;
            }
            
            Vector3 pointerPos = GetPointerPosition(step);
            Vector2 finalPos = new Vector2(pointerPos.x + currentOffset.x, pointerPos.y + currentOffset.y);
            
            RectTransform rectTransform = pointerInstance.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = finalPos;
            }
            else
            {
                pointerInstance.transform.position = new Vector3(finalPos.x, finalPos.y, pointerPos.z);
            }
            
            pointerInstance.transform.localEulerAngles = new Vector3(0, 0, currentRotation);
            
            TutorialPointer pointerScript = pointerInstance.GetComponent<TutorialPointer>();
            if (pointerScript != null)
            {
                pointerScript.SetPosition(pointerInstance.transform.position);
                pointerScript.rotateSpeed = currentRotateSpeed;
            }
            
            pointerInstance.SetActive(true);
        }
        else if (pointerInstance != null)
        {
            pointerInstance.SetActive(false);
        }
    }
    
    private Vector3 GetPointerPosition(TutorialStep step)
    {
        if (step.pointerPosition != Vector2.zero)
        {
            return new Vector3(step.pointerPosition.x, step.pointerPosition.y, 0f);
        }
        
        return GetHighlightPosition(step.highlightType);
    }
    
    private Vector3 GetHighlightPosition(TutorialHighlightType type)
    {
        GameObject target = null;
        
        switch (type)
        {
            case TutorialHighlightType.SpawnPoint:
                target = spawnPoint;
                break;
            case TutorialHighlightType.EndPoint:
                target = endPoint;
                break;
            case TutorialHighlightType.PauseButton:
                target = pauseButton;
                break;
            case TutorialHighlightType.SpeedButton:
                target = speedButton;
                break;
            case TutorialHighlightType.SkillButton:
                target = skillButton;
                break;
        }
        
        if (target != null)
        {
            Vector3 finalPos = ConvertToUIPosition(target);
            return finalPos;
        }
        
        return Vector3.zero;
    }
    
    private Vector3 ConvertToUIPosition(GameObject target)
    {
        Canvas canvas = FindCanvas()?.GetComponent<Canvas>();
        Camera mainCamera = Camera.main;
        
        if (canvas == null || mainCamera == null)
        {
            return target.transform.position;
        }
        
        Vector3 worldPos = target.transform.position;
        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        Vector2 localPoint;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera,
            out localPoint
        );
        
        return localPoint;
    }
    
    public void NextStep()
    {
        currentStep++;
        
        if (currentStep >= tutorialSteps.Count)
        {
            EndTutorial();
        }
        else
        {
            ShowCurrentStep();
        }
    }
    
    public void SkipTutorial()
    {
        EndTutorial();
    }
    
    private void EndTutorial()
    {
        isTutorialActive = false;
        
        PlayerPrefs.SetInt(tutorialCompletedKey, 1);
        PlayerPrefs.Save();
        
        if (tutorialUIInstance != null)
        {
            Destroy(tutorialUIInstance);
            tutorialUIInstance = null;
            tutorialUI = null;
        }
        
        if (pointerInstance != null)
        {
            Destroy(pointerInstance);
            pointerInstance = null;
        }
        
        ResumeGame();
        
        ZombieSpawner spawner = FindObjectOfType<ZombieSpawner>();
        if (spawner != null)
        {
            spawner.StartWave();
        }
    }
    
    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(tutorialCompletedKey);
        PlayerPrefs.Save();
    }
}
