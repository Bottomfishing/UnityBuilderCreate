using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerProfileDisplay : MonoBehaviour, IPointerClickHandler
{
    [Header("UI组件")]
    public Image avatarImage;
    public Text nameText;

    [Header("弹窗设置")]
    public GameObject nameEditPanel;
    public InputField nameInputField;
    public Button confirmNameButton;
    public Button cancelNameButton;

    public GameObject avatarSelectPanel;
    public Transform avatarContainer;
    public GameObject avatarOptionPrefab;

    private void Awake()
    {
        if (confirmNameButton != null)
        {
            confirmNameButton.onClick.AddListener(OnConfirmName);
        }

        if (cancelNameButton != null)
        {
            cancelNameButton.onClick.AddListener(OnCancelName);
        }
    }

    private void Start()
    {
        if (PlayerProfileManager.instance != null)
        {
            PlayerProfileManager.instance.OnProfileChanged += RefreshDisplay;
        }
        RefreshDisplay();

        if (nameEditPanel != null)
        {
            nameEditPanel.SetActive(false);
        }

        if (avatarSelectPanel != null)
        {
            avatarSelectPanel.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (PlayerProfileManager.instance != null)
        {
            PlayerProfileManager.instance.OnProfileChanged -= RefreshDisplay;
        }
    }

    public void RefreshDisplay()
    {
        if (PlayerProfileManager.instance == null) return;

        if (avatarImage != null)
        {
            avatarImage.sprite = PlayerProfileManager.instance.GetAvatar();
        }

        if (nameText != null)
        {
            nameText.text = PlayerProfileManager.instance.GetPlayerName();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
        Debug.Log($"点击了: {clickedObject?.name}");
        
        if (avatarImage != null)
        {
            if (clickedObject == avatarImage.gameObject || clickedObject.transform.IsChildOf(avatarImage.transform))
            {
                Debug.Log("打开头像选择");
                OpenAvatarSelection();
                return;
            }
        }
        
        Debug.Log("打开名字编辑");
        OpenNameEdit();
    }

    public void OpenNameEdit()
    {
        if (nameEditPanel != null)
        {
            nameEditPanel.SetActive(true);

            if (nameInputField != null && PlayerProfileManager.instance != null)
            {
                nameInputField.text = PlayerProfileManager.instance.GetPlayerName();
                nameInputField.Select();
            }
        }
    }

    public void CloseNameEdit()
    {
        if (nameEditPanel != null)
        {
            nameEditPanel.SetActive(false);
        }
    }

    private void OnConfirmName()
    {
        if (nameInputField != null && PlayerProfileManager.instance != null)
        {
            PlayerProfileManager.instance.SetPlayerName(nameInputField.text);
        }
        CloseNameEdit();
    }

    private void OnCancelName()
    {
        CloseNameEdit();
    }

    public void OpenAvatarSelection()
    {
        if (avatarSelectPanel != null)
        {
            avatarSelectPanel.SetActive(true);
            RefreshAvatarOptions();
        }
    }

    public void CloseAvatarSelection()
    {
        if (avatarSelectPanel != null)
        {
            avatarSelectPanel.SetActive(false);
        }
    }

    private void RefreshAvatarOptions()
    {
        if (avatarContainer == null || avatarOptionPrefab == null) return;

        foreach (Transform child in avatarContainer)
        {
            Destroy(child.gameObject);
        }

        if (PlayerProfileManager.instance == null) return;

        Sprite[] avatars = PlayerProfileManager.instance.GetAvailableAvatars();
        int currentIndex = PlayerProfileManager.instance.GetAvatarIndex();

        for (int i = 0; i < avatars.Length; i++)
        {
            GameObject optionObj = Instantiate(avatarOptionPrefab, avatarContainer);
            AvatarOption option = optionObj.GetComponent<AvatarOption>();
            
            if (option != null)
            {
                option.Setup(avatars[i], i, i == currentIndex, OnAvatarSelected);
            }
        }
    }

    private void OnAvatarSelected(int index)
    {
        if (PlayerProfileManager.instance != null)
        {
            PlayerProfileManager.instance.SetAvatar(index);
        }
        CloseAvatarSelection();
    }
}
