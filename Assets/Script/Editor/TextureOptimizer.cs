using UnityEditor;
using UnityEngine;

public class TextureOptimizer : EditorWindow
{
    [MenuItem("工具/优化图片设置")]
    public static void OptimizeAllTextures()
    {
        OptimizeFolder("Assets/Image/BG", 1024, true);
        OptimizeFolder("Assets/Image/僵尸", 512, true);
        OptimizeFolder("Assets/Image/炮塔", 256, true);
        OptimizeFolder("Assets/Image/UI", 512, true);
        OptimizeFolder("Assets/Image/子弹", 256, true);
        OptimizeFolder("Assets/Image/技能", 512, true);
        OptimizeFolder("Assets/Image/icon", 256, true);
        
        Debug.Log("图片优化完成！");
    }

    static void OptimizeFolder(string folderPath, int maxSize, bool useCompression)
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            
            if (importer != null)
            {
                importer.maxTextureSize = maxSize;
                importer.textureCompression = TextureImporterCompression.Compressed;
                
                var webglSettings = importer.GetPlatformTextureSettings("WebGL");
                webglSettings.overridden = true;
                webglSettings.format = TextureImporterFormat.ASTC_6x6;
                webglSettings.maxTextureSize = maxSize;
                importer.SetPlatformTextureSettings(webglSettings);
                
                var androidSettings = importer.GetPlatformTextureSettings("Android");
                androidSettings.overridden = true;
                androidSettings.format = TextureImporterFormat.ASTC_6x6;
                androidSettings.maxTextureSize = maxSize;
                importer.SetPlatformTextureSettings(androidSettings);
                
                importer.SaveAndReimport();
            }
        }
        
        Debug.Log($"优化完成: {folderPath} ({guids.Length} 张图片)");
    }
}
