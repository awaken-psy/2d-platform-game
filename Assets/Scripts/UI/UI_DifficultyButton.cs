using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_DifficultyButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI difficultyText;

    [TextArea]
    [SerializeField] private string description;

    public void OnPointerEnter(PointerEventData eventData) {
        difficultyText.text = description;
    }

    public void OnPointerExit(PointerEventData eventData) {
        difficultyText.text = "";
    }
}
