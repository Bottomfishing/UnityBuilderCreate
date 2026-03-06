using UnityEngine;

public class UIClickEffectManager : MonoBehaviour
{
    public static UIClickEffectManager instance;
    
    [Header("点击特效预制件")]
    public GameObject uiClickEffectPrefab;
    
    [Header("特效设置")]
    public bool enableClickEffect = true;
    
    private Canvas mainCanvas;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        FindMainCanvas();
    }
    
    private void FindMainCanvas()
    {
        mainCanvas = FindObjectOfType<Canvas>();
    }
    
    private void Update()
    {
        if (!enableClickEffect || uiClickEffectPrefab == null)
        {
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            SpawnClickEffect(Input.mousePosition);
        }
        
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    SpawnClickEffect(touch.position);
                }
            }
        }
    }
    
    private void SpawnClickEffect(Vector2 screenPosition)
    {
        if (mainCanvas == null)
        {
            FindMainCanvas();
        }
        
        if (mainCanvas == null)
        {
            Debug.LogError("没有找到Canvas！");
            return;
        }
        
        GameObject effect = Instantiate(uiClickEffectPrefab, mainCanvas.transform);
        
        RectTransform rectTransform = effect.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(0, 0);
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform,
                screenPosition,
                mainCanvas.worldCamera,
                out localPoint
            );
            
            rectTransform.localPosition = localPoint;
        }
        
        effect.transform.SetAsLastSibling();
    }
    
    public void SetClickEffectEnabled(bool enabled)
    {
        enableClickEffect = enabled;
    }
}
