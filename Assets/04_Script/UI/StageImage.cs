using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class StageImage : MonoBehaviour
{
    [SerializeField]private Image wow;
    [SerializeField]private TextMeshProUGUI textMeshProUGUI;

    void Update()
    {
        wow.fillAmount = StageManager.Instance.Floor / 100f;
        textMeshProUGUI.text = StageManager.Instance.Floor.ToString();
        
    }
}
