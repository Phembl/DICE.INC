using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private bool printLog;
    [TitleGroup("References")] 
    [SerializeField] private GameObject menuGO;
    [SerializeField] private GameObject backgroundGO;
    [Space] 
    [SerializeField] private GameObject containerMessage;
    [SerializeField] private GameObject containerStats;
    [SerializeField] private GameObject containerSettings;
    private GameObject[] container = new GameObject[3];
    
    private Image menuImage;
    private Image backgroundImage;
    private BoxCollider2D backgroundCollider;
    
    //State Tracking
    private bool isOpen;
    public bool GetMenuStatus() => isOpen;
    private bool isBusy;
    public bool GetBusyStatus() => isBusy;
    private int currentMenu;
    
    
    //Positions
    private float menuPosYInactive = 1805f;
    private float menuPosYActive = 0f;
        
    
    public static MenuManager instance;
    private void Awake()
    {
        if  (instance == null) instance = this;
    }

    void Start()
    {
        menuImage = menuGO.GetComponent<Image>();
        backgroundImage = backgroundGO.GetComponent<Image>();
        backgroundCollider = backgroundGO.GetComponent<BoxCollider2D>();

        container[0] = containerMessage;
        container[1] = containerStats;
        container[2] = containerSettings;
        
        //Set Menu Starting Conditions
        menuGO.transform.localPosition = new Vector3(0f, menuPosYInactive, 0f);
        if (backgroundGO.activeSelf) backgroundGO.SetActive(false);
        
        //Deactivate Container
        SwitchContainer(false, 0);
        SwitchContainer(false, 1);
        SwitchContainer(false, 2);
    }

    public void OpenMenu(int menuIndex)
    {
        if (isBusy) return;
        isBusy = true;

        if (isOpen)
        {
            //Switch Menu
            SwitchContainer(false, currentMenu);
            SwitchContainer(true, menuIndex, true);
            
            currentMenu = menuIndex;
            isBusy = false;
        }
        else //Open Menu
        {
            isOpen = true;
            currentMenu = menuIndex;
            
            menuImage.DOFade(0, 0);
            backgroundImage.DOFade(0, 0);
            menuImage.transform.DOLocalMoveY(menuPosYInactive, 0);
        
            backgroundGO.SetActive(true);

            StartCoroutine(OpenMenuAnimation(menuIndex));
            
            if (printLog) Debug.Log("MenuManager: Opening menu");
        }
    }

    IEnumerator OpenMenuAnimation(int menuIndex)
    {
        menuImage.DOFade(1f, 0f);
        backgroundImage.DOFade(0.93f, 0.5f);
        menuImage.transform.DOLocalMoveY(menuPosYActive, 1f).SetEase(Ease.OutQuad);

        yield return new WaitForSeconds(0.8f);
        
        SwitchContainer(true, menuIndex, true);
        
        yield return new WaitForSeconds(0.5f);
        
        isBusy = false;
    }

    public void CloseMenu()
    {
        if (!isOpen || isBusy) return;
        
        isBusy = true;
        if (printLog) Debug.Log("MenuManager: Closing menu");
        SwitchContainer(false, currentMenu);
        StartCoroutine(CloseMenuAnimation());
    }

    IEnumerator CloseMenuAnimation()
    {
        backgroundImage.DOFade(0f, 0.5f);
        menuImage.transform.DOLocalMoveY(menuPosYInactive, 0.8f);

        yield return new WaitForSeconds(0.8f);
        
        backgroundGO.SetActive(false);
        
        isOpen = false;
        isBusy = false;
    }

    void SwitchContainer(bool activate, int containerIndex, bool fade = false)
    {
        GameObject currentContainer = container[containerIndex];
        float fadeTime = 0f;
        if (fade) fadeTime = .5f;
        
        if (activate)   //Open Container
        {
            if (!currentContainer.activeSelf) currentContainer.SetActive(true);
            currentContainer.GetComponent<CanvasGroup>().DOFade(1f, fadeTime);
        }
        
        else    //Close Container
        {
            currentContainer.GetComponent<CanvasGroup>().DOFade(0f, fadeTime)
                .OnComplete(() =>
                {
                    if (currentContainer.activeSelf) currentContainer.SetActive(false);
                });
            
        }
    }

}
