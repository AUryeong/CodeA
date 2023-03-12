using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIStanding : MonoBehaviour, IPoolObject
{
    [SerializeField] private Image baseStanding;
    [SerializeField] private Image face;
    [SerializeField] private Image sideFace;

    public Character NowStanding { get; private set; }

    private RectTransform rectTransform;

    [Header("감정 표현용")] [SerializeField] private Image emotionBase;

    [Space(10)] [SerializeField] private Image[] thinkingTalks;

    [Space(10)] [SerializeField] private Image flushBase;
    [SerializeField] private Image flushLine;

    [Space(10)] [SerializeField] private Image funnyNote;

    [Space(10)] [SerializeField] private Image worry;

    protected void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init()
    {
        if (NowStanding == null)
        {
            gameObject.SetActive(false);
            return;
        }

        NowStanding = null;

        if (!gameObject.activeSelf) return;

        baseStanding.DOKill();
        baseStanding.DOColor(Utility.fadeOutBlack, 0.3f).OnComplete(() => { gameObject.SetActive(false); });

        face.DOKill();
        face.DOColor(Utility.fadeOutBlack, 0.3f);
    }

    public void ShowCharacter(Character talkStanding)
    {
        if (!talkStanding.dark)
            rectTransform.SetAsLastSibling();
        if (NowStanding != null)
        {
            if (NowStanding.name.Equals(talkStanding.name) && NowStanding.clothes.Equals(talkStanding.clothes))
            {
                var sprite = ResourcesManager.Instance.GetCharacter(talkStanding.name).standings[talkStanding.clothes].faces[talkStanding.face];
                var toColor2 = talkStanding.dark ? Utility.darkColor : Color.white;
                var toFadeColor = talkStanding.dark ? Utility.fadeDarkColor : Utility.fadeWhite;
                if (sprite != face.sprite)
                {
                    var duration = 0.4f;

                    sideFace.DOKill();
                    sideFace.sprite = face.sprite;
                    sideFace.color = toColor2;
                    sideFace.gameObject.SetActive(true);
                    sideFace.DOFade(0, duration).SetEase(Ease.Linear).OnComplete(() => { sideFace.gameObject.SetActive(false); });

                    face.color = toFadeColor;
                    face.sprite = sprite;
                    face.DOKill();
                    face.DOFade(1, duration).SetEase(Ease.Linear);
                }
                else
                {
                    face.DOKill();
                    face.DOColor(toColor2, 0.3f);
                }

                baseStanding.DOKill();
                baseStanding.DOColor(toColor2, 0.3f);

                rectTransform.DOKill(true);

                if (!NowStanding.size.Equals(talkStanding.size))
                    rectTransform.DOScale(Utility.SizeToScale(talkStanding.size), 1);

                if (!NowStanding.pos.Equals(talkStanding.pos))
                    rectTransform.DOAnchorPosX(Utility.PosToVector2(talkStanding.pos), 1);

                NowStanding = talkStanding;
                return;
            }
        }

        var character = ResourcesManager.Instance.GetCharacter(talkStanding.name);
        Standing standing;
        if (character.standings.ContainsKey(talkStanding.clothes))
            standing = character.standings[talkStanding.clothes];
        else
            standing = character.standings.Values.ToArray()[0];

        var toScale = Utility.SizeToScale(talkStanding.size);
        var toPos = Utility.PosToVector2(talkStanding.pos);

        rectTransform.DOKill(true);
        rectTransform.localScale = toScale;
        rectTransform.anchoredPosition = new Vector2(toPos, 431.5f);

        baseStanding.DOKill();
        baseStanding.sprite = standing.baseStanding;
        baseStanding.color = Utility.fadeOutBlack;
        var toColor = talkStanding.dark ? Utility.darkColor : Color.white;
        baseStanding.DOColor(toColor, 0.3f);

        face.DOKill();
        face.sprite = standing.faces[talkStanding.face];
        face.color = Utility.fadeOutBlack;
        face.DOColor(toColor, 0.3f);

        sideFace.gameObject.SetActive(false);
        NowStanding = talkStanding;
    }

    public void SetDark(bool dark)
    {
        if(!dark)
            transform.SetAsLastSibling();
        var toColor = dark ? Utility.darkColor : Color.white;
        
        baseStanding.DOKill();
        baseStanding.DOColor(toColor, 0.3f);
        
        face.DOKill();
        face.DOColor(toColor, 0.3f);
    }

    public void Move(float posX, float duration)
    {
        rectTransform.DOKill(true);
        rectTransform.DOAnchorPosX(posX, duration);
    }

    public void FaceChange(string faceName)
    {
        var duration = 0.4f;

        sideFace.DOKill();
        sideFace.sprite = face.sprite;
        sideFace.color = Color.white;
        sideFace.gameObject.SetActive(true);
        sideFace.DOFade(0, duration).SetEase(Ease.Linear).OnComplete(() => { sideFace.gameObject.SetActive(false); });

        face.DOKill();
        face.color = Utility.fadeWhite;
        face.sprite = ResourcesManager.Instance.GetCharacter(NowStanding.name).standings[NowStanding.clothes].faces[faceName];
        face.DOFade(1, duration).SetEase(Ease.Linear);
        
        if (!NowStanding.dark)
            rectTransform.SetAsLastSibling();
    }

    public void Bounce(int repeat, float duration)
    {
        rectTransform.DOAnchorPosY(-50, duration).SetRelative().SetLoops(repeat * 2, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        if (!NowStanding.dark)
            rectTransform.SetAsLastSibling();
    }

    public void Scale(Vector3 scale, float duration)
    {
        rectTransform.DOScale(scale, duration);
        if (!NowStanding.dark)
            rectTransform.SetAsLastSibling();
    }

    public void Shake(float power, float duration)
    {
        if (power < 0)
            power = 6;
        rectTransform.DOShakeAnchorPos(duration, power, 30, 90, false, false).SetRelative();
        if (!NowStanding.dark)
            rectTransform.SetAsLastSibling();
    }

    public void Emotion(string emotionName, float duration)
    {
        if (!NowStanding.dark)
            rectTransform.SetAsLastSibling();
        
        if (duration < 0)
            duration = 1f;

        float fadeOutDelay = duration + 0.5f;
        float fadeOutDuration = 0.3f;

        emotionBase.gameObject.SetActive(true);
        emotionBase.DOKill();
        emotionBase.color = Color.white;
        emotionBase.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay).OnComplete(() => { emotionBase.gameObject.SetActive(false); });

        emotionBase.rectTransform.DOKill();
        emotionBase.rectTransform.localScale = Vector3.zero;
        emotionBase.rectTransform.DOScale(Vector3.one, fadeOutDuration);

        foreach (Image image in thinkingTalks)
        {
            image.DOKill();
            image.gameObject.SetActive(false);
        }

        flushBase.DOKill();
        flushBase.gameObject.SetActive(false);
        flushLine.DOKill();
        flushLine.rectTransform.DOKill();
        flushLine.gameObject.SetActive(false);

        funnyNote.DOKill();
        funnyNote.gameObject.SetActive(false);

        worry.DOKill();
        worry.rectTransform.DOKill();
        worry.gameObject.SetActive(false);

        switch (emotionName)
        {
            case "Thinking":
                for (var index = 0; index < thinkingTalks.Length; index++)
                {
                    var image = thinkingTalks[index];
                    image.gameObject.SetActive(true);
                    image.color = Utility.fadeWhite;
                    image.DOFade(1, duration / 6).SetDelay(duration / 4 * index);
                    image.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay);
                }

                break;
            case "Flush":
                flushBase.gameObject.SetActive(true);
                flushBase.color = Utility.fadeWhite;
                flushBase.DOFade(1, duration / 10);
                flushBase.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay);

                flushLine.gameObject.SetActive(true);
                flushLine.rectTransform.DOLocalRotate(new Vector3(0, 0, 10), duration / 3).OnComplete(() =>
                {
                    flushLine.rectTransform.DOLocalRotate(new Vector3(0, 0, -10), duration / 3).OnComplete(() => { flushLine.rectTransform.DOLocalRotate(new Vector3(0, 0, 0), duration / 3); });
                });
                flushLine.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay);
                break;
            case "Funny":
                funnyNote.gameObject.SetActive(true);
                funnyNote.color = Utility.fadeWhite;
                funnyNote.DOFade(1, duration / 10);
                funnyNote.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay);

                funnyNote.rectTransform.anchoredPosition = new Vector2(20, 44);
                funnyNote.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);

                funnyNote.rectTransform.DOAnchorPos(new Vector2(-30, 64), fadeOutDelay);
                funnyNote.rectTransform.DOLocalRotate(new Vector3(0, 0, Random.Range(5f, 15f)), duration / 2).OnComplete(() =>
                {
                    funnyNote.rectTransform.DOLocalRotate(new Vector3(0, 0, Random.Range(-15f, -35f)), duration / 2).OnComplete(() =>
                    {
                        funnyNote.rectTransform.DOLocalRotate(new Vector3(0, 0, Random.Range(10f, 30f)), 0.5f);
                    });
                });
                break;
            case "Worry":
                worry.gameObject.SetActive(true);
                worry.color = Utility.fadeWhite;
                worry.DOFade(1, duration / 10);
                worry.DOFade(0, fadeOutDuration).SetDelay(fadeOutDelay);

                worry.rectTransform.anchoredPosition = Vector2.zero;
                worry.rectTransform.localScale = new Vector3(1, 1, 1);
                worry.rectTransform.localRotation = Quaternion.identity;

                worry.rectTransform.DOShakeScale(fadeOutDelay, 0.4f);
                worry.rectTransform.DOShakePosition(fadeOutDelay, 10, 10, 90F, false, false);
                worry.rectTransform.DOLocalRotate(new Vector3(0, 0, Random.Range(35f, 15f)), duration).OnComplete(() =>
                {
                    worry.rectTransform.DOLocalRotate(new Vector3(0, 0, Random.Range(-15f, -35f)), fadeOutDuration);
                });
                break;
        }
    }
}