using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIDifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI _difficultyInfo;
    [SerializeField, TextArea] private string _description;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _difficultyInfo.text = _description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _difficultyInfo.text = "";
    }
}