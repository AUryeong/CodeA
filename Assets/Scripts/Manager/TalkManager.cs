using System;
using System.Collections.Generic;
using Coffee.UIEffects;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    protected override void OnCreated()
    {
        subBackgroundEffect = subBackgroundImage.GetComponent<UITransitionEffect>();
    }

    protected override void OnReset()
    {
        talkQueue.Clear();
        animations.Clear();

        talkDuration = 0;
        autoDuration = 0;

        talkWindow.gameObject.SetActive(false);
        nowTalk = null;
        prevTalk = null;

        blackFadeIn.gameObject.SetActive(false);
        blackFadeOut.gameObject.SetActive(false);

        foreach (var obj in uiStandings)
        {
            obj.gameObject.SetActive(false);
            obj.Init();
        }
    }

    public List<Talk> GetLeftTalks()
    {
        var talkList = new List<Talk>() { nowTalk };
        talkList.AddRange(talkQueue);
        return talkList;
    }

    public void AddTalk(string talkName)
    {
        var talks = ResourcesManager.Instance.GetTalk(talkName);

        if (!string.IsNullOrEmpty(talks.cgTitle))
            if (!SaveManager.Instance.GameData.getVideo.Contains(talkName))
                SaveManager.Instance.GameData.getVideo.Add(talkName);

        bool flag = talkQueue.Count <= 0;
        foreach (var talk in talks.talks)
            talkQueue.Enqueue(talk);

        if (flag) NewTalk();
    }

    public void AddTalk(Talks talks)
    {
        bool flag = talkQueue.Count <= 0;
        foreach (var talk in talks.talks)
            talkQueue.Enqueue(talk);

        if (flag) NewTalk();
    }

    public void AddTalk(List<Talk> talks)
    {
        bool flag = talkQueue.Count <= 0;
        foreach (var talk in talks)
            talkQueue.Enqueue(talk);

        if (flag) NewTalk();
    }

    private void Update()
    {
        TalkUpdate();
        AnimationWait();
    }

    private void AnimationWait()
    {
        if (animationWaitTime > 0)
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
                var findStanding = uiStandings.Find((standing) => standing.gameObject.activeSelf && standing.NowStanding.name == anim.name);
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
                    }
                }

                break;
            }
            case AnimationType.DIAL:
            {
                switch (anim.effect)
                {
                    case "On":
                        dialogueImage.DOKill();
                        dialogueImage.gameObject.SetActive(true);
                        dialogueImage.color = new Color(0.08627451F, 0.08627451F, 0.08627451F, 0);
                        dialogueImage.DOFade(0.9137255F, 0.2f);
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

    public void CheckClick(PointerEventData data)
    {
        if (dialogueImage.gameObject.activeSelf)
        {
            if (descriptionText.maxVisibleCharacters < descriptionText.textInfo.characterCount)
            {
                descriptionText.maxVisibleCharacters = descriptionText.textInfo.characterCount;
            }
            else
            {
                int linkIndex = TMP_TextUtilities.FindIntersectingLink(descriptionText, Input.mousePosition, GameManager.Instance.UICamera);

                if (linkIndex != -1)
                {
                    var linkInfo = descriptionText.textInfo.linkInfo[linkIndex];
                    if (!string.IsNullOrEmpty(ResourcesManager.Instance.GetTip(linkInfo.GetLinkID())))
                    {
                        EventManager.Instance.EventOpen(linkInfo.GetLinkID(), data.position);
                    }
                }
                else
                {
                    NewTalk();
                }
            }
        }
    }

    private void TalkUpdate()
    {
        if (!talkWindow.gameObject.activeSelf) return;
        if (!dialogueImage.gameObject.activeSelf) return;
        if (nowTalk.dialogue == null)
        {
            if (animations.Count <= 0)
            {
                autoDuration += Time.deltaTime;
                if (autoDuration >= autoWaitTime)
                {
                    autoDuration -= autoWaitTime;
                    NewTalk();
                }
            }

            return;
        }

        if (descriptionText.maxVisibleCharacters < descriptionText.textInfo.characterCount)
        {
            talkDuration += Time.deltaTime;
            float cooltime = defaultTalkCooltime * SaveManager.Instance.GameData.textSpeed;
            if (talkDuration >= cooltime)
            {
                talkDuration -= cooltime;
                descriptionText.maxVisibleCharacters++;
            }
        }
        else
        {
            if (!endTextEffect.gameObject.activeSelf)
            {
                endTextEffect.gameObject.SetActive(true);
                endTextEffect.DOKill();
                endTextEffect.color = new Color(105 / 255f, 1, 126 / 255f, 1);
                endTextEffect.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);

            }
            var characterPos = new Vector3(-824.4f, 32.39394f);
            if (!string.IsNullOrWhiteSpace(descriptionText.text) && !descriptionText.text.Equals(string.Empty))
            {
                if (descriptionText.textInfo.characterInfo != null &&
                    descriptionText.textInfo.characterInfo.Length >= descriptionText.maxVisibleCharacters - 1)
                    characterPos = Utility.GetVector3Aver(
                        descriptionText.textInfo.characterInfo[descriptionText.maxVisibleCharacters - 1].topRight,
                        descriptionText.textInfo.characterInfo[descriptionText.maxVisibleCharacters - 1].bottomRight);
            }

            endTextEffect.rectTransform.anchoredPosition = new Vector2(characterPos.x + 40, characterPos.y - 20);

            if (SaveManager.Instance.GameData.textAuto)
            {
                autoDuration += Time.deltaTime;
                if (autoDuration >= autoWaitTime)
                {
                    autoDuration -= autoWaitTime;
                    NewTalk();
                }
            }
        }

        if (subBackgroundImage.gameObject.activeSelf)
        {
            subBackgroundEffect.effectFactor = Mathf.Min(1, subBackgroundEffect.effectFactor + Time.deltaTime);
            if (subBackgroundEffect.effectFactor >= 1)
            {
                backgroundImage.sprite = subBackgroundImage.sprite;
                subBackgroundEffect.gameObject.SetActive(false);
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

    private void NewTalk()
    {
        if (talkQueue.Count <= 0)
        {
            talkWindow.gameObject.SetActive(false);
            animations.Clear();

            prevTalk = null;
            nowTalk = null;

            foreach (var obj in uiStandings)
            {
                obj.gameObject.SetActive(false);
                obj.Init();
            }

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

        endTextEffect.gameObject.SetActive(false);
        nowTalk = talkQueue.Dequeue();

        if (nowTalk.dialogue != null)
        {
            talkerText.text = "> " + Utility.GetTalkerName(nowTalk.dialogue.talker);
            descriptionText.text = Utility.GetTalkerName(nowTalk.dialogue.text);
            dialogueImage.gameObject.SetActive(nowTalk.dialogue.active);
        }
        else
            dialogueImage.gameObject.SetActive(false);

        descriptionText.maxVisibleCharacters = 0;

        if (!string.IsNullOrEmpty(nowTalk.bgm))
        {
            if (prevTalk == null || !nowTalk.bgm.Equals(prevTalk.bgm))
            {
                SoundManager.Instance.PlaySound(nowTalk.bgm);
            }
        }

        if (prevTalk == null || prevTalk.background.name != nowTalk.background.name)
        {
            if (!string.IsNullOrEmpty(nowTalk.background.name))
            {
                var background = ResourcesManager.Instance.GetBackground(nowTalk.background.name);

                subBackgroundImage.DOKill(true);
                switch (nowTalk.background.effect)
                {
                    default:
                    case BackgroundEffect.NONE:
                        backgroundImage.sprite = background;
                        subBackgroundImage.gameObject.SetActive(false);
                        break;
                    case BackgroundEffect.TRANS:
                        subBackgroundImage.gameObject.SetActive(true);
                        subBackgroundImage.sprite = background;
                        subBackgroundEffect.effectFactor = 0;
                        break;
                    case BackgroundEffect.FADE:
                        subBackgroundImage.gameObject.SetActive(true);
                        subBackgroundImage.sprite = background;
                        subBackgroundImage.color = Utility.fadeWhite;
                        subBackgroundImage.DOFade(1, 1).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            backgroundImage.sprite = subBackgroundImage.sprite;
                            subBackgroundEffect.gameObject.SetActive(false);
                        });
                        break;
                }

                if (nowTalk.background.name.StartsWith("CG_"))
                {
                    foreach (var standing in uiStandings)
                    {
                        standing.gameObject.SetActive(false);
                        standing.Init();
                    }

                    GameManager.Instance.ViewCG(nowTalk.background.name);
                    GetEffect();
                    return;
                }
            }
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

        GetEffect();
    }

    private void GetEffect()
    {
        foreach (var anim in nowTalk.animations)
        {
            animations.Enqueue(anim);
        }

        AnimationUpdate();
    }
}