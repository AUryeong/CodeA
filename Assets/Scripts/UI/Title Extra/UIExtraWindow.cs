using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIExtraWindow : MonoBehaviour
    {
        private RectTransform rectTransform;
        private Vector3 pos;

        [SerializeField] private Image title;
        
        [Header("베이스")] [SerializeField] private Button exitButton;
        [SerializeField] private Button selectExtra;
        [SerializeField] private TextMeshProUGUI selectExtraText;
        [SerializeField] private TextMeshProUGUI selectExtraEffect;

        [SerializeField] private RectTransform buttonsParent;

        [Header("사진 관련")] [SerializeField] private Button picturesButton;

        [SerializeField] private RectTransform picturesWindow;

        [SerializeField] private Button[] cgs;
        [SerializeField] private Button cgPrevButton;
        [SerializeField] private Button cgNextButton;
        [SerializeField] private TextMeshProUGUI cgPageText;
        [SerializeField] private Image fullScreenCgImage;

        private int picturesIdx;

        [Header("씬 관련")] [SerializeField] private Button videosButton;

        [SerializeField] private RectTransform videosWindow;

        [SerializeField] private Button[] videos;
        private readonly List<TextMeshProUGUI> videoTitles = new List<TextMeshProUGUI>();
        [SerializeField] private Button videoPrevButton;
        [SerializeField] private Button videoNextButton;
        [SerializeField] private TextMeshProUGUI videoPageText;

        private int videosIdx;

        [Header("팁 관련")] [SerializeField] private Button tipsButton;

        [SerializeField] private RectTransform tipsWindow;
        [SerializeField] private UITipCell tipCell;
        [SerializeField] private UITipVerticalScroll verticalScroll;

        [Header("이벤트")][SerializeField] private Image eventWindow;
        [SerializeField] private TextMeshProUGUI eventTitleText;
        [SerializeField] private TextMeshProUGUI eventDescriptionText;
        [SerializeField] private Button eventExitButton;

        public void OnCreated()
        {
            rectTransform = GetComponent<RectTransform>();
            
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(Escape);
            
            selectExtra.onClick.RemoveAllListeners();
            selectExtra.onClick.AddListener(Reset);


            picturesButton.onClick.RemoveAllListeners();
            picturesButton.onClick.AddListener(ClickPictures);

            cgNextButton.onClick.RemoveAllListeners();
            cgNextButton.onClick.AddListener(NextCgs);

            cgPrevButton.onClick.RemoveAllListeners();
            cgPrevButton.onClick.AddListener(PrevCgs);

            var temp = fullScreenCgImage.GetComponent<Button>();
            temp.onClick.RemoveAllListeners();
            temp.onClick.AddListener(DisableFullscreenCg);
            DisableFullscreenCg();


            videosButton.onClick.RemoveAllListeners();
            videosButton.onClick.AddListener(ClickVideos);

            foreach (var button in videos)
                videoTitles.Add(button.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>());

            videoNextButton.onClick.RemoveAllListeners();
            videoPrevButton.onClick.AddListener(NextVideos);

            videoPrevButton.onClick.RemoveAllListeners();
            videoPrevButton.onClick.AddListener(PrevVideos);

            tipsButton.onClick.RemoveAllListeners();
            tipsButton.onClick.AddListener(ClickTips);

            tipCell.extraWindow = this;

            eventExitButton.onClick.RemoveAllListeners();
            eventExitButton.onClick.AddListener(EventExit);
        }

        private void Escape()
        {
            if(selectExtra.gameObject.activeSelf)
                Reset();
            else
                Disable();
        }

        public void Init(Vector3 toPos)
        {
            pos = toPos;
            
            rectTransform.DOKill();
            gameObject.SetActive(true);

            rectTransform.position = pos; 
            rectTransform.localScale = Vector3.zero;

            rectTransform.DOScale(Vector3.one, 0.2f).SetUpdate(true);
            rectTransform.DOAnchorPos(Vector2.zero, 0.2f).SetUpdate(true);
            Reset();
        }

        private void Disable()
        {
            rectTransform.DOKill();
            rectTransform.localScale = Vector3.one;

            rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => gameObject.SetActive(false)).SetUpdate(true);
            rectTransform.DOMove(pos, 0.2f).SetUpdate(true);
        }

        private void Reset()
        {
            DisableFullscreenCg();
            selectExtra.gameObject.SetActive(false);
            picturesWindow.gameObject.SetActive(false);
            videosWindow.gameObject.SetActive(false);
            tipsWindow.gameObject.SetActive(false);

            buttonsParent.gameObject.SetActive(true);
            title.gameObject.SetActive(true);
        }

        private void DisableFullscreenCg()
        {
            fullScreenCgImage.gameObject.SetActive(false);
        }

        private void ClickPictures()
        {
            picturesWindow.gameObject.SetActive(true);
            ReloadCgs();
            ExtraSelectOn("Pictures");
        }

        private void ExtraSelectOn(string text)
        {
            buttonsParent.gameObject.SetActive(false);

            selectExtra.gameObject.SetActive(true);
            selectExtraText.text = "ㄴ " + text;

            selectExtraEffect.gameObject.SetActive(true);
            selectExtraEffect.DOKill();
            selectExtraEffect.color = new Color(105 / 255f, 1, 126 / 255f, 1);
            selectExtraEffect.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            
            title.gameObject.SetActive(false);
        }

        private void ReloadCgs()
        {
            var getCgs = SaveManager.Instance.GameData.getCg;
            for (int i = 0; i < cgs.Length; i++)
            {
                if (getCgs.Count > (picturesIdx * 6) + i)
                {
                    Sprite sprite = ResourcesManager.Instance.GetBackground(getCgs[(picturesIdx * 6) + i]);
                    cgs[i].image.sprite = sprite;
                    cgs[i].onClick.RemoveAllListeners();
                    cgs[i].onClick.AddListener(() => ShowFullscreenCg(sprite));
                }
                else
                {
                    cgs[i].onClick.RemoveAllListeners();
                    cgs[i].gameObject.SetActive(false);
                }
            }

            cgPageText.text = (picturesIdx + 1).ToString();
            cgNextButton.gameObject.SetActive(getCgs.Count > ((picturesIdx + 1) * 6));
        }

        private void PrevCgs()
        {
            if (picturesIdx <= 0)
            {
                Reset();
                return;
            }

            picturesIdx--;
            ReloadCgs();
        }

        private void NextCgs()
        {
            picturesIdx++;
            ReloadCgs();
        }

        private void ShowFullscreenCg(Sprite selectCg)
        {
            fullScreenCgImage.gameObject.SetActive(true);
            fullScreenCgImage.sprite = selectCg;
        }

        private void ClickVideos()
        {
            videosWindow.gameObject.SetActive(true);
            ExtraSelectOn("Videos");
            ReloadVideos();
        }

        private void ReloadVideos()
        {
            var getVideos = SaveManager.Instance.GameData.getVideo;
            for (int i = 0; i < videos.Length; i++)
            {
                if (getVideos.Count > (videosIdx * 6) + i)
                {
                    var talks = ResourcesManager.Instance.GetTalk(getVideos[(videosIdx * 6) + i]);
                    videos[i].image.sprite = ResourcesManager.Instance.GetBackground(talks.talks[0].background.name);
                    videos[i].onClick.RemoveAllListeners();
                    videos[i].onClick.AddListener(() => ShowTalk(talks));
                    videoTitles[i].text = talks.cgTitle;
                }
                else
                {
                    videos[i].onClick.RemoveAllListeners();
                    videos[i].gameObject.SetActive(false);
                }
            }

            videoPageText.text = (videosIdx + 1).ToString();
            videoNextButton.gameObject.SetActive(getVideos.Count > ((videosIdx + 1) * 6));
        }

        private void ShowTalk(Talks talks)
        {
            TalkManager.Instance.AddTalk(talks);
        }


        private void NextVideos()
        {
            videosIdx++;
            ReloadVideos();
        }

        private void PrevVideos()
        {
            if (videosIdx <= 0)
            {
                Reset();
                return;
            }

            videosIdx--;
            ReloadVideos();
        }

        private void ClickTips()
        {
            tipsWindow.gameObject.SetActive(true);
            ExtraSelectOn("Tips");
            verticalScroll.SetLog(SaveManager.Instance.GameData.getTips);
        }

        public void EventOpen(string eventName)
        {
            eventTitleText.text = eventName + "에 대해";
            eventDescriptionText.text = ResourcesManager.Instance.GetTip(eventName);

            eventWindow.gameObject.SetActive(true);
            eventWindow.rectTransform.DOKill();
            eventWindow.rectTransform.localScale = Vector3.zero;
            eventWindow.rectTransform.DOScale(Vector3.one, 0.2f);
        }

        private void EventExit()
        {
            eventWindow.rectTransform.DOKill();
            eventWindow.rectTransform.localScale = Vector3.one;
            eventWindow.rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => eventWindow.gameObject.SetActive(false));
        }
    }
}