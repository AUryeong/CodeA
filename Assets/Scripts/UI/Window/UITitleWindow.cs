using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITitleWindow : UIWindow
    {
        [SerializeField] private Button warningOkayButton;
        [SerializeField] private Button warningCancelButton;
        [SerializeField] private TextMeshProUGUI warningDescription;
        [SerializeField] private TextMeshProUGUI warningDescription2;

        public override void OnCreated()
        {
            base.OnCreated();

            warningCancelButton.onClick.RemoveAllListeners();
            warningCancelButton.onClick.AddListener(WindowManager.Instance.CloseScrollAndWindow);
        }

        public override void Init(Image button)
        {
            base.Init(button);

            warningOkayButton.onClick.RemoveAllListeners();

            if (GameManager.Instance.nowGameData == null)
            {
                warningDescription.text = "정말로 종료 하시겠습니까?";
                warningDescription2.gameObject.SetActive(false);
                warningOkayButton.onClick.AddListener(Application.Quit);
            }
            else
            {
                warningDescription.text = "정말로 타이틀로 돌아가시겠습니까?";
                warningDescription2.gameObject.SetActive(true);
                warningOkayButton.onClick.AddListener(() => GameManager.Instance.SceneLoad(Scene.TITLE));
            }
        }
    }
}