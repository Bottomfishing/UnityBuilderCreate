using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AvatarOption : MonoBehaviour, IPointerClickHandler
{
    [Header("UI组件")]
    public Image avatarImage;
    public Image selectBorder;

    private int _avatarIndex;
    private System.Action<int> _onSelected;

    public void Setup(Sprite avatar, int index, bool isSelected, System.Action<int> onSelected)
    {
        _avatarIndex = index;
        _onSelected = onSelected;

        if (avatarImage != null)
        {
            avatarImage.sprite = avatar;
            // Enable raycast target so the image can receive pointer click events
            avatarImage.raycastTarget = true;
        }

        SetSelected(isSelected);
    }

    public void SetSelected(bool selected)
    {
        if (selectBorder != null)
        {
            selectBorder.enabled = selected;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_onSelected != null)
        {
            _onSelected(_avatarIndex);
        }
    }
}
