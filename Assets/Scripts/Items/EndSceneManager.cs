using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class EndSceneManager : MonoBehaviour
{
    void Start()
    {
        AudioManager.instance.PlayMusic("endGame");
        StartCoroutine(LoadMenuScene());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SkipToMenu();
        }
    }

    IEnumerator LoadMenuScene()
    {
        yield return new WaitForSeconds(200f);
        SceneManager.LoadScene("MainMenu");
    }

    public void SkipToMenu()
    {
        StopAllCoroutines();
        SceneManager.LoadScene("MainMenu");
    }
}
