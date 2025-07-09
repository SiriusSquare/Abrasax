using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UISoundScript : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private Button button;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip highlightSound;
    [SerializeField] private AudioClip clickSound;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

    }

    public void PlayClickSound()
    {
        if (button.interactable && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable && highlightSound != null)
        {
            audioSource.PlayOneShot(highlightSound);
        }
    }
}
