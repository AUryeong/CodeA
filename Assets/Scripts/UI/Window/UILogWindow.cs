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

        public override void OnCreated()
        {
            base.OnCreated();

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(WindowManager.Instance.CloseAllWindow);
        }

        public override void Init(Vector3 pos)
        {
            base.Init(pos);
            logScroll.SetLog(LogManager.Instance.GetDatas());
        }
    }
}