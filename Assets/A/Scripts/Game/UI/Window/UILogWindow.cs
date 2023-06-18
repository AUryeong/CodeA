using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UILogWindow : UIWindow
    {
        [SerializeField] private Button exitButton;
        [SerializeField] private UILogVerticalScroll logScroll;

        [SerializeField] private UILogCell logCell;
        public DialogEventDetail EventDetail { get; private set; }

        public override void OnCreated()
        {
            base.OnCreated();
            EventDetail = GetComponent<DialogEventDetail>();

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(GameManager.Instance.windowManager.CloseAllWindow);

            EventDetail.OnCreated();

            logCell.logWindow = this;
        }

        public override void Init(Vector3 pos)
        {
            base.Init(pos);
            logScroll.SetLog(GameManager.Instance.dialogManager.LogDetail.GetDatas());
        }
    }
}