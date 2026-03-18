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
    public Button closeAvatarButton;

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

        if (closeAvatarButton != null)
        {
            closeAvatarButton.onClick.AddListener(CloseAvatarSelection);
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
        
        if (avatarImage != null)
        {
            if (clickedObject == avatarImage.gameObject || clickedObject.transform.IsChildOf(avatarImage.transform))
            {
                OpenAvatarSelection();
                return;
            }
        }
        
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
                
                #if UNITY_ANDROID || UNITY_IOS
                // 移动端：打开触摸键盘
                nameInputField.Select();
                nameInputField.ActivateInputField();
                TouchScreenKeyboard.Open(nameInputField.text, TouchScreenKeyboardType.Default, false, false, false, false);
                #else
                // PC端：直接选中输入框
                nameInputField.Select();
                nameInputField.ActivateInputField();
                #endif
            }
        }
        
        // 关闭头像选择弹窗
        if (avatarSelectPanel != null)
        {
            avatarSelectPanel.SetActive(false);
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
        
        // 关闭名字编辑弹窗
        if (nameEditPanel != null)
        {
            nameEditPanel.SetActive(false);
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

        if (avatars == null || avatars.Length == 0) return;

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
