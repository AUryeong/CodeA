using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UITitleWindow : UIWindow
    {
        [SerializeField] private Button warningOkayButton;
        [SerializeField] private Button warningCancelButton;
        [SerializeField] private Button blockButton;
        [SerializeField] private TextMeshProUGUI warningDescription;
        [SerializeField] private TextMeshProUGUI warningDescription2;

        public override void OnCreated()
        {
            base.OnCreated();

            warningCancelButton.onClick.RemoveAllListeners();
            warningCancelButton.onClick.AddListener(WindowManager.Instance.CloseAllWindow);

            blockButton.onClick.RemoveAllListeners();
            blockButton.onClick.AddListener(WindowManager.Instance.CloseAllWindow);
        }

        public override void Init(Vector3 pos)
        {
            base.Init(pos);

            warningOkayButton.onClick.RemoveAllListeners();

            if (GameManager.Instance.saveManager.nowGameData == null && TalkManager.Instance.GetLeftTalks().Count <= 0)
            {
                warningDescription.text = "정말로 종료하시겠습니까?";
                warningDescription2.gameObject.SetActive(false);
                warningOkayButton.onClick.AddListener(() => GameManager.Instance.sceneManager.SceneLoadFadeIn(Application.Quit));
            }
            else
            {
                warningDescription.text = "정말로 타이틀로\n돌아가시겠습니까?";
                warningDescription2.gameObject.SetActive(true);
                warningOkayButton.onClick.AddListener(() => GameManager.Instance.sceneManager.SceneLoad(Scene.TITLE));
            }
        }
    }
}