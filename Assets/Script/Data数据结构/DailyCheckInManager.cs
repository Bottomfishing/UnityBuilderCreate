using UnityEngine;
using UnityEngine.UI;
using System;

public class DailyCheckInManager : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject checkInUI;
    public Button closeButton;
    public Button checkInButton;
    public Transform daysContainer;
    public GameObject dayItemPrefab;
    
    [Header("签到数据")]
    public DailyCheckInData checkInData;
    
    [Header("文本设置")]
    public Text statusText;
    public Text checkedInDaysText;
    public Text insideTipText;
    public Text outsideTipText;
    
    [Header("布局设置")]
    public float normalWidth = 90f;
    public float normalHeight = 90f;
    public float bigWidth = 290f;
    public float bigHeight = 90f;
    public float spacing = 10f;
    
    private const string LastCheckInDateKey = "LastCheckInDate";
    private const string CheckedInDaysKey = "CheckedInDays";
    
    private void Start()
    {
        if (checkInUI != null)
        {
            checkInUI.SetActive(false);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseCheckIn);
        }
        
        if (checkInButton != null)
        {
            checkInButton.onClick.AddListener(OnCheckInButtonClick);
        }
        
        GenerateDayItems();
        UpdateCheckInStatus();
    }
    
    private void GenerateDayItems()
    {
        if (daysContainer == null || dayItemPrefab == null || checkInData == null)
        {
            return;
        }
        
        foreach (Transform child in daysContainer)
        {
            if (child != daysContainer)
            {
                Destroy(child.gameObject);
            }
        }
        
        float totalWidth = normalWidth * 3 + spacing * 2;
        float rowHeight = normalHeight + spacing;
        
        for (int i = 0; i < 7; i++)
        {
            GameObject dayObj = Instantiate(dayItemPrefab, daysContainer);
            RectTransform rectTrans = dayObj.GetComponent<RectTransform>();
            
            float x, y, width, height;
            
            if (i < 3)
            {
                float rowStartX = -totalWidth / 2f + normalWidth / 2f;
                x = rowStartX + i * (normalWidth + spacing);
                y = rowHeight;
                width = normalWidth;
                height = normalHeight;
            }
            else if (i < 6)
            {
                float rowStartX = -totalWidth / 2f + normalWidth / 2f;
                x = rowStartX + (i - 3) * (normalWidth + spacing);
                y = 0f;
                width = normalWidth;
                height = normalHeight;
            }
            else
            {
                x = 0f;
                y = -rowHeight;
                width = bigWidth;
                height = bigHeight;
            }
            
            if (rectTrans != null)
            {
                rectTrans.anchorMin = new Vector2(0.5f, 0.5f);
                rectTrans.anchorMax = new Vector2(0.5f, 0.5f);
                rectTrans.pivot = new Vector2(0.5f, 0.5f);
                rectTrans.sizeDelta = new Vector2(width, height);
                rectTrans.anchoredPosition = new Vector2(x, y);
            }
            
            SetupDayItem(dayObj, i);
        }
    }
    
    private void SetupDayItem(GameObject dayObj, int dayIndex)
    {
        if (dayObj == null || checkInData == null || dayIndex >= checkInData.dailyRewards.Length)
        {
            return;
        }
        
        DailyReward reward = checkInData.dailyRewards[dayIndex];
        if (reward == null)
        {
            return;
        }
        
        Text dayText = dayObj.transform.Find("DayText")?.GetComponent<Text>();
        if (dayText != null)
        {
            dayText.text = $"第 {dayIndex + 1} 天";
        }
        
        Image iconImage = dayObj.transform.Find("IconImage")?.GetComponent<Image>();
        if (iconImage != null && reward.rewardIcon != null)
        {
            iconImage.sprite = reward.rewardIcon;
        }
        
        Text amountText = dayObj.transform.Find("AmountText")?.GetComponent<Text>();
        if (amountText != null)
        {
            amountText.text = $"+{reward.amount}";
        }
        
        Image backgroundImage = dayObj.GetComponent<Image>();
        GameObject checkMark = dayObj.transform.Find("CheckMark")?.gameObject;
        
        int checkedInDays = GetCheckedInDays();
        bool isChecked = dayIndex < checkedInDays;
        bool isToday = dayIndex == checkedInDays;
        
        if (checkMark != null)
        {
            checkMark.SetActive(isChecked);
        }
        
        if (backgroundImage != null)
        {
            if (isChecked)
            {
                Color color = backgroundImage.color;
                color.r = 0.7f;
                color.g = 0.7f;
                color.b = 0.7f;
                backgroundImage.color = color;
            }
            else if (isToday && CanCheckInToday())
            {
                Color color = backgroundImage.color;
                color.r = 1f;
                color.g = 0.9f;
                color.b = 0.5f;
                backgroundImage.color = color;
            }
        }
    }
    
    private void UpdateCheckInStatus()
    {
        bool canCheckIn = CanCheckInToday();
        int checkedInDays = GetCheckedInDays();
        
        if (statusText != null)
        {
            if (canCheckIn)
            {
                statusText.text = "今日可签到！";
                statusText.color = Color.green;
            }
            else
            {
                statusText.text = "今日已签到";
                statusText.color = Color.yellow;
            }
        }
        
        if (insideTipText != null)
        {
            insideTipText.gameObject.SetActive(canCheckIn);
        }
        
        if (outsideTipText != null)
        {
            outsideTipText.gameObject.SetActive(canCheckIn);
        }
        
        if (checkedInDaysText != null)
        {
            checkedInDaysText.text = $"已签到: {checkedInDays}/7 天";
        }
        
        if (checkInButton != null)
        {
            checkInButton.interactable = canCheckIn;
        }
        
        UpdateAllDayItems();
    }
    
    private void UpdateAllDayItems()
    {
        if (daysContainer == null)
        {
            return;
        }
        
        for (int i = 0; i < daysContainer.childCount; i++)
        {
            Transform child = daysContainer.GetChild(i);
            if (child != null)
            {
                SetupDayItem(child.gameObject, i);
            }
        }
    }
    
    private bool CanCheckInToday()
    {
        string lastDateStr = PlayerPrefs.GetString(LastCheckInDateKey, "");
        DateTime today = DateTime.Today;
        
        if (string.IsNullOrEmpty(lastDateStr))
        {
            return true;
        }
        
        DateTime lastDate;
        if (DateTime.TryParse(lastDateStr, out lastDate))
        {
            if (lastDate < today)
            {
                if ((today - lastDate).TotalDays > 1)
                {
                    PlayerPrefs.SetInt(CheckedInDaysKey, 0);
                }
                return true;
            }
        }
        
        return false;
    }
    
    private int GetCheckedInDays()
    {
        return PlayerPrefs.GetInt(CheckedInDaysKey, 0);
    }
    
    private void OnCheckInButtonClick()
    {
        if (!CanCheckInToday())
        {
            return;
        }
        
        int checkedInDays = GetCheckedInDays();
        if (checkedInDays >= 7)
        {
            checkedInDays = 0;
        }
        
        if (checkInData != null && checkedInDays < checkInData.dailyRewards.Length)
        {
            DailyReward reward = checkInData.dailyRewards[checkedInDays];
            if (reward != null && ResourceManager.instance != null)
            {
                if (reward.rewardType == RewardType.Coins)
                {
                    ResourceManager.instance.AddCoins(reward.amount);
                }
                else if (reward.rewardType == RewardType.Diamonds)
                {
                    ResourceManager.instance.AddDiamonds(reward.amount);
                }
            }
        }
        
        checkedInDays++;
        if (checkedInDays > 7)
        {
            checkedInDays = 1;
        }
        
        PlayerPrefs.SetInt(CheckedInDaysKey, checkedInDays);
        PlayerPrefs.SetString(LastCheckInDateKey, DateTime.Today.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
        
        UpdateCheckInStatus();
    }
    
    public void OpenCheckIn()
    {
        if (checkInUI != null)
        {
            checkInUI.SetActive(true);
            UpdateCheckInStatus();
        }
    }
    
    public void CloseCheckIn()
    {
        if (checkInUI != null)
        {
            checkInUI.SetActive(false);
        }
    }
}
