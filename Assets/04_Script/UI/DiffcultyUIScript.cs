using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DiffcultyUIScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tm;
    private string[] difficultyLevels = { "Super Easy","Easy", "Normal", "Hard", "Apocalypse", "Azathoth" };

    private void Start()
    {
        tm.text = $"난이도 설정 : {difficultyLevels[CoreScript.Instance.Difficulty]}";
    }
    public void OnChange()
    {
        if (CoreScript.Instance.Difficulty < difficultyLevels.Length - 1)
        {
            
            CoreScript.Instance.Difficulty++;
            tm.text = $"난이도 설정 : {difficultyLevels[CoreScript.Instance.Difficulty]} (난이도 배율 : {CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty]})";
        }
        else
        {
            CoreScript.Instance.Difficulty = 0;
            tm.text = $"난이도 설정 : {difficultyLevels[CoreScript.Instance.Difficulty]} (난이도 배율 : {CoreScript.Instance.Diffint[CoreScript.Instance.Difficulty]})";
        }
    }
}
