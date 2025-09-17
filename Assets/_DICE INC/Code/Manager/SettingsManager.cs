using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public Color colorNormal;
    public Color colorDark;
    public Color colorInactive;
    
    public static SettingsManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
        
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }
}
