using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneMoveScript : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private GameObject activeobject;
    [SerializeField] private float delayTime = 0.5f;
    [SerializeField] private AudioClip playMusic;
    public void SceneChange()
    {
        
        
        
        StartCoroutine(LoadSceneWithDelay());
    }
    public void RealSceneChange()
    {
        Time.timeScale = 1f;
        if (playMusic != null)
        {
            MusicScriptMain.Instance.PlayMusic(playMusic, true);
        }
        SceneManager.LoadScene(sceneName);
    }
    private IEnumerator LoadSceneWithDelay()
    {
        Time.timeScale = 1f;
        if (activeobject != null)
        {
            activeobject.SetActive(true);
        }
        yield return new WaitForSeconds(delayTime);
        if (playMusic != null)
        {
            MusicScriptMain.Instance.PlayMusic(playMusic,true);
        }
        SceneManager.LoadScene(sceneName);
    }
}
