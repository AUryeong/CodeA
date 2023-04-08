using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOption : MonoBehaviour
{
    [SerializeField] private Image baseStanding;
    [SerializeField] private Image face;
    [SerializeField] private Image sideFace;

    private RectTransform rectTransform;
    private Button button;

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
    }
}
