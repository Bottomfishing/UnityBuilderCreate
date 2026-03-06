using UnityEngine;
using UnityEngine.EventSystems;

public class ClickOutsideDetector : MonoBehaviour, IPointerClickHandler
{
    public GameObject targetPanel;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        // 检查点击是否在面板外
        if (!RectTransformUtility.RectangleContainsScreenPoint(
            targetPanel.GetComponent<RectTransform>(), 
            eventData.position, 
            eventData.pressEventCamera))
        {
            // 关闭面板
            EnergyNotEnoughPanel panel = targetPanel.GetComponent<EnergyNotEnoughPanel>();
            if (panel != null)
            {
                panel.HidePanel();
            }
        }
    }
}
