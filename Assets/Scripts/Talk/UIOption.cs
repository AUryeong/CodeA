using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIOption : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI scriptText;

        public Option option
        {
            get; private set;
        }

        private RectTransform rectTransform;
        private Button button;

        protected void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ClickOption);
        }

        public void SetOption(Option setOption)
        {
            option = setOption;

            scriptText.text = setOption.script;
        }

        private void ClickOption()
        {
            TalkManager.Instance.SelectOption(option);
        }
    }

}