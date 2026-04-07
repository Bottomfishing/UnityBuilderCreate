using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MonsterBook : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject monsterBookUI;
    public Button closeButton;
    public Transform contentContainer;
    public GameObject monsterItemPrefab;
    public GraphicRaycaster graphicRaycaster;
    
    [Header("怪物数据")]
    public MonsterData[] monsters;
    
    private bool isInitialized = false;
    
    private void Awake()
    {
        DoInit();
    }
    
    private void DoInit()
    {
        if (isInitialized) return;
        isInitialized = true;
        
        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(CloseMonsterBook);
        }
        
        GenerateMonsterItems();
        
        if (monsterBookUI != null)
            monsterBookUI.SetActive(false);
    }
    
    private void GenerateMonsterItems()
    {
        if (contentContainer == null || monsterItemPrefab == null || monsters == null)
        {
            return;
        }
        
        foreach (Transform child in contentContainer)
        {
            if (child != contentContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        foreach (MonsterData monster in monsters)
        {
            if (monster != null)
            {
                GameObject itemObj = Instantiate(monsterItemPrefab, contentContainer);
                SetupMonsterItem(itemObj, monster);
            }
        }
    }
    
    private void SetupMonsterItem(GameObject itemObj, MonsterData monster)
    {
        if (itemObj == null || monster == null)
        {
            return;
        }
        
        Text nameText = itemObj.transform.Find("NameText")?.GetComponent<Text>();
        if (nameText != null)
        {
            nameText.text = monster.monsterName;
        }
        
        Image iconImage = itemObj.transform.Find("IconImage")?.GetComponent<Image>();
        if (iconImage != null && monster.monsterIcon != null)
        {
            iconImage.sprite = monster.monsterIcon;
        }
        
        Text descText = itemObj.transform.Find("DescText")?.GetComponent<Text>();
        if (descText != null)
        {
            descText.text = monster.description;
        }
        
        Text healthText = itemObj.transform.Find("HealthText")?.GetComponent<Text>();
        if (healthText != null)
        {
            healthText.text = $"血量: {monster.maxHealth}";
        }
        
        Text speedText = itemObj.transform.Find("SpeedText")?.GetComponent<Text>();
        if (speedText != null)
        {
            speedText.text = $"速度: {monster.moveSpeed}";
        }
    }
    
    public void OpenMonsterBook()
    {
        if (!isInitialized) DoInit();
        
        if (monsterBookUI != null)
        {
            monsterBookUI.SetActive(true);
        }
    }
    
    public void CloseMonsterBook()
    {
        if (monsterBookUI != null)
        {
            monsterBookUI.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (monsterBookUI != null && monsterBookUI.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIObject())
                {
                    CloseMonsterBook();
                }
            }
        }
    }
    
    private bool IsPointerOverUIObject()
    {
        if (graphicRaycaster == null)
        {
            graphicRaycaster = FindObjectOfType<GraphicRaycaster>();
        }
        
        if (graphicRaycaster == null || monsterBookUI == null)
        {
            return false;
        }
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        graphicRaycaster.Raycast(eventData, results);
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject.transform.IsChildOf(monsterBookUI.transform))
            {
                return true;
            }
        }
        
        return false;
    }
}
