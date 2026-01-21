using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUIManager : MonoBehaviour
{
    [SerializeField] private Animator _UIAnimator;
    [SerializeField] private GameObject _inforMenu;

    // Singleton
    public static MainMenuUIManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }


        if (_inforMenu != null)
            _inforMenu.SetActive(false);
    }

    public void StartMainMenuAnimation()
    {
        _UIAnimator.SetTrigger(AnimationStrings.openMainMenu);
    }

    public void StartLevelMenuAnimation()
    {
        _UIAnimator.SetTrigger(AnimationStrings.openLevelMenu);
    }

    public void TurnOnInforMenu()
    {
        _inforMenu.SetActive(true);
    }

    public void TurnOffInforMenu()
    {
        if (_inforMenu != null)
            _inforMenu.SetActive(false);
    }
}
