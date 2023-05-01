using System;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UI;

public class TalkManager : Singleton<TalkManager>
{
    protected override bool IsDontDestroying => true;

    private readonly Queue<Talk> talkQueue = new Queue<Talk>();
    private Talk nowTalk;
    private Talk prevTalk;

    [Header("대화문")] [SerializeField] private GameObject talkWindow;

    [SerializeField] private Image dialogueImage;
    [SerializeField] private TextMeshProUGUI talkerText;

    [FormerlySerializedAs("descreptionText")] [SerializeField]
    private TextMeshProUGUI descriptionText;

    [Header("연출")] [SerializeField] private UITransitionEffect blackFadeIn;
    [SerializeField] private UITransitionEffect blackFadeOut;
    [SerializeField] private TextMeshProUGUI endTextEffect;

    [Space(10)] [SerializeField] private UITransitionEffect endingBlackFadeIn;
    [SerializeField] private UITransitionEffect endingBlackFadeOut;
    private bool isEnding;
    private float endingDuration;
    private const float endingWaitTime = 1;

    [Space(20)] [SerializeField] private Image backgroundTitle;
    [SerializeField] private TextMeshProUGUI backgroundTitleText;
    [SerializeField] private TextMeshProUGUI backgroundTitleLore;

    [Header("배경")] [SerializeField] private Image backgroundImage;
    [SerializeField] private Image subBackgroundImage;
    private UITransitionEffect subBackgroundEffect;

    [Header("스탠딩")] [SerializeField] private UIStanding uiStanding;
    [SerializeField] private RectTransform standingParent;
    private readonly List<UIStanding> uiStandings = new List<UIStanding>();

    [Header("애니메이션")] private Queue<Animation> animations = new Queue<Animation>();
    private float animationWaitTime;

    private float talkDuration;
    private const float defaultTalkCooltime = 0.05f;

    private float autoDuration;
    private const float autoWaitTime = 2f;

    [Header("선택지")] [SerializeField] private UIOption uiOption;
    [SerializeField] private RectTransform optionParent;
    private readonly List<UIOption> uiOptions = new List<UIOption>();
    private bool isHasOption;
    private bool optionActive;

    [Header("스킵")] [SerializeField] private Image skipBackground;
    [SerializeField] private Image skipGaugeBackground;
    [SerializeField] private Image skipGaugeImage;

    [Space(10)] [SerializeField] private Image skipWindow;
    [SerializeField] private Button skipOkayButton;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private Button skipExitButton;
    private TextMeshProUGUI skipOkayButtonText;

    public string talkSkipText;
    private bool skipButtonClicking;
    private float skipDuration;

    [Header("터치 이벤트")] private Vector2 dragStartPos;
    private bool isDialogHide;

    [Header("이벤트")] [SerializeField] private Image eventWindow;
    [SerializeField] private TextMeshProUGUI eventTitleText;
    [SerializeField] private TextMeshProUGUI eventDescriptionText;
    [SerializeField] private Button eventOkayButton;
    [SerializeField] private Button eventExitButton;
    [SerializeField] private TextMeshProUGUI eventContinueText;
    private TextMeshProUGUI eventOkayButtonText;


    private bool isHasEvent;
    private string selectEvent;
    private Vector2 nowPos;
    private Vector2 defaultPos;

    protected override void OnCreated()
    {
        subBackgroundEffect = subBackgroundImage.GetComponent<UITransitionEffect>();

        eventOkayButtonText = eventOkayButton.GetComponent<TextMeshProUGUI>();

        skipOkayButtonText = skipOkayButton.GetComponent<TextMeshProUGUI>();

        defaultPos = eventWindow.rectTransform.anchoredPosition;
        eventWindow.gameObject.SetActive(false);

        eventExitButton.onClick.RemoveAllListeners();
        eventExitButton.onClick.AddListener(EventExit);

        eventOkayButton.onClick.RemoveAllListeners();
        eventOkayButton.onClick.AddListener(EventOkay);

        skipOkayButton.onClick.RemoveAllListeners();
        skipOkayButton.onClick.AddListener(SkipOkay);

        skipExitButton.onClick.RemoveAllListeners();
        skipExitButton.onClick.AddListener(SkipExit);

        foreach (RectTransform rect in standingParent)
            uiStandings.Add(rect.GetComponent<UIStanding>());

        foreach (RectTransform rect in optionParent)
            uiOptions.Add(rect.GetComponent<UIOption>());

        talkWindow.gameObject.SetActive(false);
    }

    protected override void OnReset()
    {
        nowTalk = null;
        prevTalk = null;

        talkQueue.Clear();
        animations.Clear();

        talkDuration = 0;
        autoDuration = 0;
        endingDuration = 0;

        talkWindow.gameObject.SetActive(false);
        eventWindow.gameObject.SetActive(false);

        blackFadeIn.gameObject.SetActive(false);
        blackFadeOut.gameObject.SetActive(false);

        endingBlackFadeIn.gameObject.SetActive(false);
        endingBlackFadeOut.gameObject.SetActive(false);

        talkSkipText = string.Empty;
        isEnding = false;

        SkipReset();
        OptionReset();

        foreach (var obj in uiStandings)
        {
            obj.gameObject.SetActive(false);
            obj.Init();
        }
    }

    private void Update()
    {
        if (isEnding) return;

        TalkUpdate();
        AnimationWait();
    }

    private void EndingSetUp()
    {
        isEnding = true;
        GameManager.Instance.SceneLoadFadeIn(() =>
        {
            endingBlackFadeIn.gameObject.SetActive(false);
            endingBlackFadeOut.gameObject.SetActive(true);
            endingBlackFadeOut.effectFactor = 1;

            foreach (var obj in uiStandings)
            {
                obj.gameObject.SetActive(false);
                obj.Init();
            }

            talkWindow.gameObject.SetActive(false);

            endingDuration = 0;
            isEnding = false;
            GameManager.Instance.SceneLoadFadeOut();
        });
    }

    public void PointerDown(BaseEventData data)
    {
        if (isEnding) return;
        dragStartPos = (data as PointerEventData).position;

        if (string.IsNullOrEmpty(talkSkipText) || isHasOption) return;

        skipButtonClicking = true;
    }

    public void PointerUp(BaseEventData data)
    {
        if (isEnding) return;

        bool isSkipping = false;
        if (!string.IsNullOrEmpty(talkSkipText) && !isHasOption)
        {
            isSkipping = skipDuration >= 1;
            skipButtonClicking = false;
            skipDuration = 0;

            if (!skipWindow.gameObject.activeSelf)
                SkipReset();
            else
                return;
        }

        if (isSkipping) return;

        if (dialogueImage.gameObject.activeSelf)
        {
            Vector2 subVector = dragStartPos - (data as PointerEventData).position;
            if (subVector.y > 400)
            {
                dialogueImage.gameObject.SetActive(false);
                optionParent.gameObject.SetActive(false);
                isDialogHide = true;
                return;
            }
            else if (subVector.y < -400)
            {
                WindowManager.Instance.ClickWindow(WindowType.LOG);
                return;
            }
            else if (subVector.x > 400)
            {
                WindowManager.Instance.WindowOpen();
                return;
            }
        }

        if (isDialogHide)
        {
            dialogueImage.gameObject.SetActive(true);
            optionParent.gameObject.SetActive(true);
            isDialogHide = false;
            return;
        }

        if (string.IsNullOrEmpty(talkSkipText) || isHasOption) return;

        if (dialogueImage.gameObject.activeSelf)
        {
            if (descriptionText.maxVisibleCharacters < descriptionText.textInfo.characterCount)
            {
                descriptionText.maxVisibleCharacters = descriptionText.textInfo.characterCount;
                if (nowTalk.dialogue.talkAnimations.Count <= 0) return;

                var talkAnimations = nowTalk.dialogue.talkAnimations.OrderBy(talkAnimation => talkAnimation.parameter)
                    .ToList();
                foreach (var talkAnimation in talkAnimations)
                    if (talkAnimation.type == TalkAnimationType.ANIM)
                        EffectSetting(Mathf.RoundToInt(talkAnimation.parameter));
            }
            else
            {
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(descriptionText, Input.mousePosition,
                    GameManager.Instance.UICamera);

                if (linkIndex != -1)
                {
                    var linkInfo = descriptionText.textInfo.linkInfo[linkIndex];
                    EventOpen(linkInfo.GetLinkID(), (data as PointerEventData).position);
                }
                else
                {
                    if (nowTalk.optionList == null || nowTalk.optionList.Count <= 0)
                    {
                        NewTalk();
                    }
                }
            }
        }
    }

    public void PointerClick(BaseEventData data)
    {
    }

    private void CheckNextNoDialogue()
    {
        if (animations.Count > 0) return;

        if (isHasOption)
        {
            if (!optionActive)
            {
                OptionSetting();
                optionActive = true;
            }

            return;
        }

        autoDuration += Time.deltaTime;
        if (autoDuration >= autoWaitTime)
        {
            autoDuration -= autoWaitTime;

            NewTalk();
        }
    }

    private void TalkUpdate()
    {
        if (!talkWindow.gameObject.activeSelf) return;
        if (nowTalk == null) return;
        if (isDialogHide) return;

        if (!dialogueImage.gameObject.activeSelf || nowTalk.dialogue == null)
        {
            CheckNextNoDialogue();
            return;
        }

        if (eventWindow.gameObject.activeSelf) return;

        SkipUpdate();

        if (skipWindow.gameObject.activeSelf) return;
        if (descriptionText.maxVisibleCharacters < descriptionText.textInfo.characterCount)
        {
            talkDuration += Time.deltaTime;
            float nextDuration = defaultTalkCooltime * SaveManager.Instance.GameData.textSpeed;
            if (talkDuration >= nextDuration)
            {
                talkDuration -= nextDuration;
                descriptionText.maxVisibleCharacters++;
                foreach (var talkAnimation in nowTalk.dialogue.talkAnimations)
                {
                    if (talkAnimation.startIndex == descriptionText.maxVisibleCharacters)
                    {
                        switch (talkAnimation.type)
                        {
                            case TalkAnimationType.WAIT:
                                talkDuration -= talkAnimation.parameter;
                                break;
                            case TalkAnimationType.ANIM:
                                EffectSetting(Mathf.RoundToInt(talkAnimation.parameter));
                                break;
                        }
                    }
                }
            }
        }
        else
        {
            if (!endTextEffect.gameObject.activeSelf)
            {
                endTextEffect.gameObject.SetActive(true);
                endTextEffect.DOKill();
                endTextEffect.color = Utility.ChangeColorFade(endTextEffect.color, 1);
                endTextEffect.DOFade(0, 0.8f).SetLoops(-1, LoopType.Yoyo);
            }

            var characterPos = new Vector3(-824.4f, 32.39394f);
            if (!string.IsNullOrWhiteSpace(descriptionText.text) && !descriptionText.text.Equals(string.Empty))
            {
                if (descriptionText.textInfo.characterInfo != null && descriptionText.maxVisibleCharacters > 0 &&
                    descriptionText.textInfo.characterInfo.Length >= descriptionText.maxVisibleCharacters - 1)
                {
                    characterPos = Utility.GetVector3Aver(
                        descriptionText.textInfo.characterInfo[descriptionText.maxVisibleCharacters - 1].topRight,
                        descriptionText.textInfo.characterInfo[descriptionText.maxVisibleCharacters - 1].bottomRight);
                }
            }

            endTextEffect.rectTransform.anchoredPosition = new Vector2(characterPos.x + 40, characterPos.y - 20);

            if (isHasOption)
            {
                if (!optionActive)
                {
                    OptionSetting();
                    optionActive = true;
                }

                return;
            }

            if (SaveManager.Instance.GameData.textAuto && !isHasEvent)
            {
                autoDuration += Time.deltaTime;
                if (autoDuration >= autoWaitTime)
                {
                    autoDuration -= autoWaitTime;
                    NewTalk();
                }
            }
        }

        EffectUpdate();
    }

    private void NewTalk(Talk newTalk = null)
    {
        if (talkQueue.Count <= 0)
        {
            EndingSetUp();

            animations.Clear();

            talkSkipText = string.Empty;

            prevTalk = null;
            nowTalk = null;

            EventExit();
            SkipReset();
            OptionReset();

            return;
        }

        prevTalk = nowTalk;
        talkWindow.gameObject.SetActive(true);
        dialogueImage.DOKill();

        descriptionText.maxVisibleCharacters = 0;
        talkDuration = 0;
        autoDuration = 0;
        animationWaitTime = 0;
        animations.Clear();

        optionParent.gameObject.SetActive(true);
        OptionReset();

        endTextEffect.gameObject.SetActive(false);
        nowTalk = newTalk == null ? talkQueue.Dequeue() : newTalk;
        isHasEvent = false;
        isDialogHide = false;

        isHasOption = nowTalk.optionList.Count > 0;
        optionActive = false;

        if (nowTalk.dialogue != null)
        {
            isHasEvent = nowTalk.dialogue.tipEvent != null && nowTalk.dialogue.tipEvent.Exists((tipEvent) =>
                SaveManager.Instance.GameData.getTips.Contains(tipEvent.eventName));
            talkerText.text = string.IsNullOrEmpty(nowTalk.dialogue.talker)
                ? " "
                : "> " + Utility.GetTalkerName(nowTalk.dialogue.talker);
            descriptionText.text = string.IsNullOrEmpty(nowTalk.dialogue.text)
                ? " "
                : Utility.GetTalkerName(nowTalk.dialogue.text);
            dialogueImage.gameObject.SetActive(nowTalk.dialogue.active);
            dialogueImage.color = nowTalk.dialogue.invisible
                ? Color.clear
                : new Color(0.08627451F, 0.08627451F, 0.08627451F, 0.9137255F);
            if (nowTalk.dialogue.active)
            {
                LogManager.Instance.AddLog(nowTalk);
                EventSetting();
            }
        }
        else
        {
            dialogueImage.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(nowTalk.bgm))
        {
            if (prevTalk == null || !nowTalk.bgm.Equals(prevTalk.bgm))
            {
                SoundManager.Instance.PlaySound(nowTalk.bgm);
            }
        }

        var background = ResourcesManager.Instance.GetBackground(nowTalk.background.name);
        if (prevTalk != null)
        {
            if (prevTalk.background.name != nowTalk.background.name ||
                Math.Abs(prevTalk.background.scale - nowTalk.background.scale) > 0.01f ||
                prevTalk.background.title != nowTalk.background.title)
            {
                if (!string.IsNullOrEmpty(nowTalk.background.name))
                {
                    subBackgroundImage.DOKill(true);
                    switch (nowTalk.background.effect)
                    {
                        default:
                        case BackgroundEffect.NONE:
                            backgroundImage.sprite = background;
                            backgroundImage.rectTransform.localScale = Vector3.one * nowTalk.background.scale;
                            subBackgroundImage.gameObject.SetActive(false);
                            break;
                        case BackgroundEffect.TRANS:
                            subBackgroundImage.gameObject.SetActive(true);
                            subBackgroundImage.sprite = background;
                            backgroundImage.rectTransform.localScale = Vector3.one * prevTalk.background.scale;
                            subBackgroundImage.rectTransform.localScale = Vector3.one * nowTalk.background.scale;
                            subBackgroundEffect.effectFactor = 0;
                            break;
                        case BackgroundEffect.FADE:
                            subBackgroundEffect.effectFactor = 1;

                            backgroundImage.rectTransform.localScale = Vector3.one * prevTalk.background.scale;

                            subBackgroundImage.gameObject.SetActive(true);
                            subBackgroundImage.sprite = background;
                            subBackgroundImage.rectTransform.localScale = Vector3.one * nowTalk.background.scale;
                            subBackgroundImage.color = Utility.fadeWhite;

                            subBackgroundImage.DOFade(1, nowTalk.background.effectDuration).OnComplete(() =>
                            {
                                backgroundImage.sprite = subBackgroundImage.sprite;
                                backgroundImage.rectTransform.localScale = subBackgroundImage.rectTransform.localScale;
                                subBackgroundEffect.gameObject.SetActive(false);
                            });
                            break;
                    }
                }
            }
        }
        else
        {
            subBackgroundImage.DOKill(true);
            backgroundImage.sprite = background;
            backgroundImage.rectTransform.localScale = Vector3.one * nowTalk.background.scale;
            subBackgroundImage.gameObject.SetActive(false);
        }

        if (!string.IsNullOrEmpty(nowTalk.background.title))
        {
            backgroundTitle.rectTransform.DOKill();
            backgroundTitle.rectTransform.anchoredPosition =
                new Vector2(-1377, backgroundTitle.rectTransform.anchoredPosition.y);
            backgroundTitle.rectTransform.DOAnchorPosX(0, 1.5f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                backgroundTitle.rectTransform.DOAnchorPosX(1377, 1.5f).SetEase(Ease.InQuart);
            });
            backgroundTitleText.text = nowTalk.background.title;
            if (!string.IsNullOrEmpty(nowTalk.background.description))
            {
                backgroundTitleLore.gameObject.SetActive(true);
                backgroundTitleLore.text = nowTalk.background.description;
            }
            else
            {
                backgroundTitleLore.gameObject.SetActive(false);
            }
        }

        if (nowTalk.background.name.StartsWith("CG_"))
        {
            foreach (var standing in uiStandings)
            {
                standing.gameObject.SetActive(false);
                standing.Init();
            }

            GameManager.Instance.ViewCG(nowTalk.background.name);
            EffectSetting();
            return;
        }

        if (uiStandings.Count < nowTalk.characters.Count)
        {
            int repeatCount = nowTalk.characters.Count - uiStandings.Count;
            for (int i = 0; i < repeatCount; i++)
            {
                var temp = Instantiate(uiStanding, standingParent);
                temp.Init();
                uiStandings.Add(temp);
            }
        }

        var standings = new List<UIStanding>(uiStandings);
        var talkStandings = new List<Character>(nowTalk.characters);

        foreach (var standing in nowTalk.characters)
        {
            var prevStanding = standings.Find(x =>
                x.NowStanding != null && x.NowStanding.name.Equals(standing.name) &&
                x.NowStanding.clothes.Equals(standing.clothes));

            if (prevStanding == null) continue;

            standings.Remove(prevStanding);
            prevStanding.gameObject.SetActive(true);
            prevStanding.ShowCharacter(standing);
            talkStandings.Remove(standing);
        }

        foreach (var standing in talkStandings)
        {
            var newStanding = standings[0];

            if (newStanding == null) continue;

            standings.Remove(newStanding);
            newStanding.gameObject.SetActive(true);
            newStanding.ShowCharacter(standing);
        }

        foreach (var standing in standings)
        {
            standing.Init();
        }

        EffectSetting();
    }

    #region Skip

    private void SkipReset()
    {
        skipBackground.gameObject.SetActive(false);
        skipBackground.DOKill();
        skipWindow.gameObject.SetActive(false);
        skipWindow.rectTransform.DOKill();
        skipGaugeBackground.gameObject.SetActive(false);
        skipGaugeBackground.rectTransform.DOKill();
    }

    private void SkipUpdate()
    {
        if (!skipButtonClicking || skipWindow.gameObject.activeSelf || talkQueue.Count <= 0) return;
        skipDuration += Time.deltaTime;
        if (skipDuration >= 2.5f)
        {
            skipGaugeBackground.gameObject.SetActive(false);

            skipWindow.gameObject.SetActive(true);
            skipWindow.rectTransform.localScale = Vector3.zero;
            skipWindow.rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

            skipOkayButtonText.DOKill();
            skipOkayButtonText.color = Utility.ChangeColorFade(eventOkayButtonText.color, 1);
            skipOkayButtonText.DOFade(0, 0.8f).SetLoops(-1, LoopType.Yoyo);

            skipText.text = Utility.GetTalkerName(talkSkipText);
        }
        else if (skipDuration >= 1)
        {
            if (!skipBackground.gameObject.activeSelf)
            {
                skipBackground.gameObject.SetActive(true);
                skipBackground.color = Utility.ChangeColorFade(skipBackground.color, 0);
                skipBackground.DOFade(0.3f, 1.5f);

                skipGaugeBackground.rectTransform.localScale = Vector3.zero;
                skipGaugeBackground.rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
                skipGaugeBackground.gameObject.SetActive(true);
            }

            skipGaugeImage.fillAmount = (skipDuration - 1) / 1.5f;
        }
    }

    private void SkipOkay()
    {
        SkipExit();

        while (talkQueue.Count > 0)
        {
            Talk talk = talkQueue.Dequeue();

            if (talk.optionList != null && talk.optionList.Count > 0)
            {
                if (talk.optionList.Exists((x) => (x.eventList != null && x.eventList.Count > 0) || x.special))
                {
                    NewTalk(talk);
                    return;
                }

                DialogAdd(talk.optionList[0].eventType,
                    ResourcesManager.Instance.GetTalk(talk.optionList[0].dialog).talks);
            }

            if (talk.eventList != null && talk.eventList.Count > 0)
            {
                foreach (var getEvent in talk.eventList)
                {
                    EventInteract(getEvent);
                }
            }

            LogManager.Instance.AddLog(talk);
        }

        NewTalk();
    }

    private void SkipExit()
    {
        skipWindow.rectTransform.DOKill();
        skipWindow.rectTransform.localScale = Vector3.one;
        skipWindow.rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => skipWindow.gameObject.SetActive(false));

        skipBackground.DOKill();
        skipBackground.DOFade(0, 0.2f).OnComplete(() => skipBackground.gameObject.SetActive(false));

        skipGaugeBackground.gameObject.SetActive(false);
        skipGaugeBackground.rectTransform.DOKill();
    }

    #endregion

    #region Option

    private void OptionSetting()
    {
        if (nowTalk == null || nowTalk.optionList == null || nowTalk.optionList.Count <= 0) return;

        if (uiOptions.Count < nowTalk.optionList.Count)
        {
            int repeatCount = nowTalk.optionList.Count - uiOptions.Count;
            for (int i = 0; i < repeatCount; i++)
            {
                var temp = Instantiate(uiOption, optionParent);
                uiOptions.Add(temp);
            }
        }

        var options = new List<UIOption>(uiOptions);
        var talkOptions = new List<Option>(nowTalk.optionList);

        foreach (var option in talkOptions)
        {
            var changeOption = options[0];

            if (changeOption == null) continue;

            options.Remove(changeOption);
            changeOption.gameObject.SetActive(true);
            changeOption.SetOption(option);
        }

        foreach (var option in options)
            option.gameObject.SetActive(false);
    }

    private void OptionReset()
    {
        foreach (var option in uiOptions)
            option.gameObject.SetActive(false);
    }


    public void SelectOption(UIOption setOption)
    {
        foreach (UIOption option in uiOptions)
        {
            if (option.gameObject.activeSelf)
            {
                if (option != setOption)
                {
                    option.Disable();
                }
            }
        }

        var talks = ResourcesManager.Instance.GetTalk(setOption.Option.dialog);

        DialogAdd(setOption.Option.eventType, talks == null ? null : talks.talks);
    }

    #endregion

    #region Event

    private void EventOpen(string eventName, Vector2 pos)
    {
        nowPos = pos;
        selectEvent = eventName;

        bool isHaveTips = SaveManager.Instance.GameData.getTips.Contains(eventName);
        var tipEvent = nowTalk.dialogue.tipEvent.Find((tip) => tip.eventName == eventName);
        bool isDialogTips = !string.IsNullOrEmpty(tipEvent.talkName);

        eventTitleText.text = eventName + "에 대해";
        eventDescriptionText.text = isHaveTips ? ResourcesManager.Instance.GetTip(eventName) : "정보가 없다...";

        eventWindow.gameObject.SetActive(true);
        eventWindow.rectTransform.DOKill();

        eventWindow.rectTransform.anchoredPosition = pos;
        eventWindow.rectTransform.localScale = Vector3.zero;

        eventWindow.rectTransform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        eventWindow.rectTransform.DOAnchorPos(defaultPos, 0.3f).SetEase(Ease.OutBack);

        eventContinueText.gameObject.SetActive(isHaveTips && isDialogTips);
        eventOkayButtonText.gameObject.SetActive(isHaveTips && isDialogTips);
        if (isHaveTips && isDialogTips)
        {
            eventOkayButtonText.DOKill();
            eventOkayButtonText.color = new Color(105 / 255f, 1, 126 / 255f, 1);
            eventOkayButtonText.DOFade(0, 0.8f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void EventOkay()
    {
        EventExit();
        var tipEvent = nowTalk.dialogue.tipEvent.Find((tip) => tip.eventName == selectEvent);
        var talks = ResourcesManager.Instance.GetTalk(tipEvent.talkName).talks;

        DialogAdd(tipEvent.eventType, talks);
    }

    private void EventExit()
    {
        if (!eventWindow.gameObject.activeSelf) return;

        eventWindow.rectTransform.DOKill();
        eventWindow.rectTransform.localScale = Vector3.one;

        eventWindow.rectTransform.DOScale(Vector3.zero, 0.2f).OnComplete(() => eventWindow.gameObject.SetActive(false));
        eventWindow.rectTransform.DOAnchorPos(nowPos, 0.2f);
    }

    private void EventSetting()
    {
        if (nowTalk.eventList == null || nowTalk.eventList.Count <= 0) return;

        foreach (var getEvent in nowTalk.eventList)
            EventInteract(getEvent);
    }

    private void EventInteract(Event getEvent)
    {
        switch (getEvent.type)
        {
            case Event.Type.ADD_TIP:
            {
                GameManager.Instance.AddTip(getEvent.name);
                break;
            }
        }
    }

    #endregion

    #region Add / Save Talk

    public void AddTalk(string talkName)
    {
        var talks = ResourcesManager.Instance.GetTalk(talkName);

        if (!string.IsNullOrEmpty(talks.cgTitle))
            if (!SaveManager.Instance.GameData.getVideo.Contains(talkName))
                SaveManager.Instance.GameData.getVideo.Add(talkName);

        AddTalk(talks);
    }

    public void AddTalk(Talks talks)
    {
        bool flag = talkQueue.Count <= 0;
        foreach (var talk in talks.talks)
            talkQueue.Enqueue(talk);

        if (!string.IsNullOrEmpty(talks.skipText))
            talkSkipText = talks.skipText;

        if (flag) NewTalk();
    }

    public void AddTalk(List<Talk> talks)
    {
        bool flag = talkQueue.Count <= 0;
        foreach (var talk in talks)
            talkQueue.Enqueue(talk);

        if (flag) NewTalk();
    }

    public List<Talk> GetLeftTalks()
    {
        var talkList = new List<Talk>();
        if (nowTalk != null)
            talkList.Add(nowTalk);
        talkList.AddRange(talkQueue);
        return talkList;
    }

    private void DialogAdd(EventType eventType, List<Talk> talks)
    {
        if (talks == null || talks.Count <= 0)
        {
            NewTalk();
            return;
        }

        switch (eventType)
        {
            case EventType.BEFORE:
            {
                var leftTalks = talkQueue.ToList();
                talkQueue.Clear();

                foreach (var talk in talks)
                    talkQueue.Enqueue(talk);

                foreach (var talk in leftTalks)
                    talkQueue.Enqueue(talk);

                if (talks.Count != 0 && leftTalks.Count != 0)
                    NewTalk();
                break;
            }
            case EventType.AFTER:
            {
                bool flag = talkQueue.Count <= 0;

                AddTalk(talks);
                if (!flag)
                    NewTalk();
                break;
            }
            default:
            case EventType.CHANGE:
            {
                talkQueue.Clear();
                AddTalk(talks);
                break;
            }
        }
    }

    #endregion

    #region Animation

    private void AnimationWait()
    {
        if (animationWaitTime > 0 && !eventWindow.gameObject.activeSelf)
        {
            animationWaitTime -= Time.deltaTime;
            if (animationWaitTime <= 0)
                AnimationUpdate();
        }
    }

    private void AnimationUpdate()
    {
        if (!talkWindow.gameObject.activeSelf) return;
        if (nowTalk == null) return;
        if (animations.Count <= 0) return;

        var anim = animations.Dequeue();
        switch (anim.type)
        {
            case AnimationType.CHAR:
            {
                var findStanding = uiStandings.Find((standing) =>
                    standing.gameObject.activeSelf && standing.NowStanding != null &&
                    standing.NowStanding.name == anim.name);
                if (findStanding != null)
                {
                    switch (anim.effect)
                    {
                        case "Move":
                            findStanding.Move(Utility.PosToVector2(anim.parameter), anim.duration);
                            break;
                        case "Face":
                            findStanding.FaceChange(anim.parameter);
                            break;
                        case "Bounce":
                            findStanding.Bounce(int.Parse(anim.parameter), anim.duration);
                            break;
                        case "Shake":
                            findStanding.Shake(float.Parse(anim.parameter), anim.duration);
                            break;
                        case "Scale":
                            findStanding.Scale(Utility.SizeToScale(anim.parameter), anim.duration);
                            break;
                        case "Emotion":
                            findStanding.Emotion(anim.parameter, anim.duration);
                            break;
                        case "Dark":
                            findStanding.SetDark(bool.Parse(anim.parameter));
                            break;
                    }
                }

                break;
            }
            case AnimationType.DIAL:
            {
                switch (anim.effect)
                {
                    case "On":
                        if (nowTalk.dialogue == null) break;

                        dialogueImage.DOKill();
                        dialogueImage.gameObject.SetActive(true);
                        if (nowTalk.dialogue.invisible)
                        {
                            dialogueImage.color = Color.clear;
                        }
                        else
                        {
                            dialogueImage.color = new Color(0.08627451F, 0.08627451F, 0.08627451F, 0);
                            dialogueImage.DOFade(0.9137255F, 0.2f);
                        }

                        EventSetting();

                        if (!string.IsNullOrEmpty(nowTalk.dialogue.owner))
                        {
                            foreach (var standing in uiStandings)
                            {
                                if (!standing.gameObject.activeSelf) continue;

                                standing.SetDark(standing.NowStanding.name != nowTalk.dialogue.owner);
                            }
                        }

                        if (!nowTalk.dialogue.active)
                        {
                            LogManager.Instance.AddLog(nowTalk);
                        }

                        break;
                    case "Shake":
                        float power = float.Parse(anim.parameter);

                        if (power < 0)
                            power = 6;
                        dialogueImage.rectTransform.DOKill(true);
                        dialogueImage.rectTransform
                            .DOShakeAnchorPos(anim.duration, float.Parse(anim.parameter), 30, 90, false, false)
                            .SetRelative();
                        break;
                }

                break;
            }
            case AnimationType.CAM:
            {
                switch (anim.effect)
                {
                    case "Black_FadeIn":
                        blackFadeIn.gameObject.SetActive(true);
                        blackFadeIn.effectFactor = 0;
                        break;
                    case "Black_FadeOut":
                        if (blackFadeIn.gameObject.activeSelf)
                            blackFadeIn.gameObject.SetActive(false);
                        blackFadeOut.gameObject.SetActive(true);
                        blackFadeOut.effectFactor = 1;
                        break;
                }

                break;
            }
            default:
            case AnimationType.UTIL:
            {
                switch (anim.effect)
                {
                    case "Wait":
                        animationWaitTime = anim.duration;
                        return;
                }

                break;
            }
        }

        AnimationUpdate();
    }

    #endregion

    #region Effect

    private void EffectUpdate()
    {
        if (subBackgroundImage.gameObject.activeSelf)
        {
            if (subBackgroundEffect.effectFactor < 1)
            {
                subBackgroundEffect.effectFactor = Mathf.Min(1,
                    subBackgroundEffect.effectFactor + Time.deltaTime / nowTalk.background.effectDuration);
                if (subBackgroundEffect.effectFactor >= 1)
                {
                    backgroundImage.sprite = subBackgroundImage.sprite;
                    backgroundImage.rectTransform.localScale = subBackgroundImage.rectTransform.localScale;
                    subBackgroundEffect.gameObject.SetActive(false);
                }
            }
        }

        if (blackFadeIn.gameObject.activeSelf)
        {
            blackFadeIn.effectFactor = Mathf.Min(1, blackFadeIn.effectFactor + Time.deltaTime);
        }

        if (blackFadeOut.gameObject.activeSelf)
        {
            blackFadeOut.effectFactor = Mathf.Max(0, blackFadeOut.effectFactor - Time.deltaTime);
            if (blackFadeOut.effectFactor <= 0)
                blackFadeOut.gameObject.SetActive(false);
        }
    }

    private void EffectSetting(int index = 0)
    {
        if (nowTalk.animationLists.Count <= 0) return;

        var talkAnimation = nowTalk.animationLists.Find((anim) => anim.index == index);
        if (talkAnimation == null) return;

        bool flag = animations.Count == 0;

        foreach (var anim in talkAnimation.animations)
        {
            animations.Enqueue(anim);
        }

        if (flag)
            AnimationUpdate();
    }

    #endregion
}