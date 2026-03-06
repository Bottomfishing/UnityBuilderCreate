using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;
    public static bool skillClickProcessed = false;
    
    [Header("炸弹技能设置")]
    public Button bombSkillButton;
    public Image bombSkillCooldownImage;
    public Text bombSkillCostText;
    public float bombCooldown = 30f;
    public int bombCost = 100;
    public float bombRadius = 3f;
    public int bombDamage = 50;
    public GameObject bombExplosionPrefab;
    
    [Header("技能状态")]
    public bool isBombReady = true;
    public bool isSelectingBombTarget = false;
    private float bombCooldownTimer;
    
    [Header("其他引用")]
    public Camera mainCamera;
    public GridManager gridManager;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (bombSkillButton != null)
        {
            bombSkillButton.onClick.AddListener(OnBombSkillClicked);
        }
        
        if (bombSkillCooldownImage != null)
        {
            bombSkillCooldownImage.fillAmount = 0;
        }
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        if (gridManager == null)
        {
            gridManager = FindObjectOfType<GridManager>();
        }
        
        UpdateBombSkillUI();
    }
    
    private void Update()
    {
        skillClickProcessed = false;
        
        if (!isBombReady)
        {
            bombCooldownTimer -= Time.deltaTime;
            UpdateCooldownUI();
            
            if (bombCooldownTimer <= 0)
            {
                isBombReady = true;
                UpdateBombSkillUI();
            }
        }
        
        if (isSelectingBombTarget)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverSkillButton())
                {
                    PlaceBomb();
                    skillClickProcessed = true;
                }
            }
        }
    }
    
    private bool IsPointerOverSkillButton()
    {
        if (bombSkillButton == null)
        {
            return false;
        }
        
        UnityEngine.EventSystems.PointerEventData eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = Input.mousePosition;
        
        System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult> results = new System.Collections.Generic.List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);
        
        foreach (UnityEngine.EventSystems.RaycastResult result in results)
        {
            if (result.gameObject == bombSkillButton.gameObject || result.gameObject.transform.IsChildOf(bombSkillButton.transform))
            {
                return true;
            }
        }
        
        return false;
    }
    
    private void OnBombSkillClicked()
    {
        if (!isBombReady)
        {
            return;
        }
        
        if (isSelectingBombTarget)
        {
            CancelBombSelection();
            skillClickProcessed = true;
            return;
        }
        
        isSelectingBombTarget = true;
        UpdateBombSkillUI();
        skillClickProcessed = true;
    }
    
    private void PlaceBomb()
    {
        if (mainCamera == null)
        {
            Debug.LogError("MainCamera is null!");
            CancelBombSelection();
            return;
        }
        
        if (GameManager.Instance == null || !GameManager.Instance.SpendMoney(bombCost))
        {
            CancelBombSelection();
            return;
        }
        
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        if (bombExplosionPrefab != null)
        {
            GameObject explosion = Instantiate(bombExplosionPrefab, mouseWorldPos, Quaternion.identity);
            Destroy(explosion, 2f);
        }
        
        DealBombDamage(mouseWorldPos);
        
        isSelectingBombTarget = false;
        isBombReady = false;
        bombCooldownTimer = bombCooldown;
        
        UpdateBombSkillUI();
    }
    
    private void DealBombDamage(Vector3 explosionPosition)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(explosionPosition, bombRadius);
        
        foreach (Collider2D col in hitColliders)
        {
            if (col.CompareTag("Zombie"))
            {
                ZombieHealth zombieHealth = col.GetComponent<ZombieHealth>();
                if (zombieHealth != null)
                {
                    zombieHealth.TakeDamage(bombDamage);
                }
            }
        }
    }
    
    private void CancelBombSelection()
    {
        isSelectingBombTarget = false;
        UpdateBombSkillUI();
    }
    
    private void UpdateBombSkillUI()
    {
        if (bombSkillButton != null)
        {
            bombSkillButton.interactable = isBombReady || isSelectingBombTarget;
            
            if (isSelectingBombTarget)
            {
                bombSkillButton.GetComponent<Image>().color = Color.yellow;
            }
            else if (isBombReady)
            {
                bombSkillButton.GetComponent<Image>().color = Color.white;
            }
            else
            {
                bombSkillButton.GetComponent<Image>().color = Color.gray;
            }
        }
        
        if (bombSkillCostText != null)
        {
            if (isSelectingBombTarget)
            {
                bombSkillCostText.text = "点击释放";
            }
            else
            {
                bombSkillCostText.text = $"${bombCost}";
            }
        }
    }
    
    private void UpdateCooldownUI()
    {
        if (bombSkillCooldownImage != null)
        {
            bombSkillCooldownImage.fillAmount = bombCooldownTimer / bombCooldown;
        }
    }
    
    private bool IsPointerOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
    
    public bool IsSkillActive()
    {
        return isSelectingBombTarget;
    }
    
    private void OnDrawGizmosSelected()
    {
        if (isSelectingBombTarget)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, bombRadius);
        }
    }
}
