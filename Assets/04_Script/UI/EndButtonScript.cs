using System.Collections;
using UnityEditor;
using UnityEngine;

public class EndButtonScript : MonoBehaviour
{
    [SerializeField] private GameObject endScreen;
    public void ToggleScreen()
    {
        endScreen.SetActive(true);
        StartCoroutine(WaitAndClose());
    }
    private IEnumerator WaitAndClose()
    {
        yield return new WaitForSeconds(0.3f);
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit(); // 어플리케이션 종료
        #endif
    }
}
