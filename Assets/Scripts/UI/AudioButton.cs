using UnityEngine;
using UnityEngine.UI;

public class AudioButton : MonoBehaviour
{
    [SerializeField] private Image iconOn;
    [SerializeField] private Image iconOff;

    private void Start()
    {
        ChangeIcon();
    }

    public void ChangeIcon()
    {
        if (PlayerPrefs.GetFloat("BGMVolume") <= -80) // muted
        {
            iconOff.enabled = true;
            iconOn.enabled = false;
        }
        else
        {
            iconOn.enabled = true;
            iconOff.enabled = false;
        }

        if (PlayerPrefs.GetFloat("SFXVolume") <= -80) // muted
        {
            iconOff.enabled = true;
            iconOn.enabled = false;
        }
        else
        {
            iconOn.enabled = true;
            iconOff.enabled = false;
        }
    }
}