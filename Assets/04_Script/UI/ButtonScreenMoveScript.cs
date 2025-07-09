using UnityEngine;
using DG.Tweening;  
public class ButtonScreenMoveScript : MonoBehaviour
{
    [SerializeField] private RectTransform mainPos;
    [SerializeField] private RectTransform targetPosition1;
    [SerializeField] private RectTransform targetPosition2;
    [SerializeField] private Sequence moveSequence;
    [SerializeField] private Ease moveEase;
    [SerializeField] private float moveDuration = 0.5f;

    private bool isActive = false;

    private void Start()
    {
        isActive = false;
        mainPos.DOMove(targetPosition1.position, 0.1f).SetEase(moveEase);
    }
    public void ToggleScreen()
    {
        if (isActive)
        {
            mainPos.DOMove(targetPosition1.position, moveDuration).SetEase(moveEase);
        }
        else
        {
            mainPos.DOMove(targetPosition2.position, moveDuration).SetEase(moveEase);
        }
    
        isActive = !isActive;
    }
}
