using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    [Header("体力设置")]
    public int energyCostPerGame = 10;
    
    public void LoadScene(string sceneName)
    {
        // 跳过体力消耗检查
        // if (ResourceManager.instance != null)
        // {
        //     if (!ResourceManager.instance.SpendEnergy(energyCostPerGame))
        //     {
        //         return;
        //     }
        // }
        
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentScene()
    {
        // 跳过体力消耗检查
        // if (ResourceManager.instance != null)
        // {
        //     if (!ResourceManager.instance.SpendEnergy(energyCostPerGame))
        //     {
        //         return;
        //     }
        // }
        
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MenuScene");
    }
}
