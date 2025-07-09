using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
public class MenuScript : MonoBehaviour
{
    public bool isMenuOpen = false;
    public GameObject menuUI;
    public TextMeshProUGUI resumeText;
    
    void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            OnResumeButtonClicked();
        }
    }
    public void OnResumeButtonClicked()
    {
        isMenuOpen = !isMenuOpen;
        menuUI.SetActive(isMenuOpen);
        Time.timeScale = isMenuOpen ? 0f : 1f;
    }
    public void ResumeTextPlus(string pushstring)
    {

        resumeText.text += $"\n{pushstring}";
    }
}
