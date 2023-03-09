using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILogWindow : UIWindow
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private UILogVerticalScroll logScroll;
        [SerializeField] private Image warningWindow;

        public override void OnCreated()
        {
            base.OnCreated();

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(WindowManager.Instance.CloseScrollAndWindow);
        }

        public override void Init(Image button)
        {
            base.Init(button);
            var logList = LogManager.Instance.GetDatas();
            if(logList == null || logList.Count <= 0)
            {
                warningWindow.gameObject.SetActive(true);
            }
            logScroll.SetLog(logList);
        }
    }
}