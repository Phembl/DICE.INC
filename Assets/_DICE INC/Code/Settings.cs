using UnityEngine;

public class Settings : MonoBehaviour
{
    public Color colorNormal;
    public Color colorDark;
    public Color colorInactive;
    
    public static Settings instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }
}
