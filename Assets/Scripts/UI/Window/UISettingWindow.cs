using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

namespace UI
{
    public class UISettingWindow : UIWindow
    {
        [SerializeField] private Button exitButton;
        [Header("사운드")] [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider bgmSlider;

        [Header("이름 설정")] [SerializeField] private TMP_InputField namingInput;

        [SerializeField] private Image warningWindow;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Button warningOkay;
        [SerializeField] private Button warningCancel;

        [Header("텍스트 출력")] [SerializeField]
        private Slider textSpeedSlider;

        [SerializeField] private Button textTypeToggle;
        private Image textTypeImage;
        [SerializeField] private Sprite textTypeSpriteOn;
        [SerializeField] private Sprite textTypeSpriteOff;

        [Space(20f)] [SerializeField] private Button talkUI;
        [FormerlySerializedAs("descreptionText")] [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI endTextEffect;
        
        private float talkDuration;
        private const float defaultTalkCooltime = 0.05f;

        private float autoDuration;
        private const float autoWaitTime = 2f;

        private const string exampleText = "가장 사랑스러운 소녀 앨리스 리델이라 부른다면 앨리스 피리아를 잊어서는 안된다";

        public override void OnCreated()
        {
            base.OnCreated();
            sfxSlider.onValueChanged.RemoveAllListeners();
            sfxSlider.onValueChanged.AddListener(ChangeSfxSlider);

            bgmSlider.onValueChanged.RemoveAllListeners();
            bgmSlider.onValueChanged.AddListener(ChangeBgmSlider);

            textSpeedSlider.onValueChanged.RemoveAllListeners();
            textSpeedSlider.onValueChanged.AddListener(ChangeTextSpeedSlider);

            textTypeToggle.onClick.RemoveAllListeners();
            textTypeToggle.onClick.AddListener(ChangeTextType);

            talkUI.onClick.RemoveAllListeners();
            talkUI.onClick.AddListener(CheckClick);
            
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(WindowManager.Instance.CloseAllWindow);
            
            namingInput.onEndEdit.AddListener((text) =>
            {
                warningWindow.gameObject.SetActive(true);
                warningText.text = "당신의 이름을 " + (string.IsNullOrEmpty(text) ? "김준우" : text) + "(으)로 확정하시겠습니까?";
                //TODO SOUND
            });

            warningOkay.onClick.RemoveAllListeners();
            warningOkay.onClick.AddListener(EnterName);

            warningCancel.onClick.RemoveAllListeners();
            warningCancel.onClick.AddListener(() =>
            {
                warningWindow.gameObject.SetActive(false);
                //TODO SOUND
            });

            textTypeImage = textTypeToggle.GetComponent<Image>();
            warningWindow.gameObject.SetActive(false);
            descriptionText.text = exampleText;
        }

        private void TalkUpdate()
        {
            if (descriptionText.maxVisibleCharacters < descriptionText.text.Length)
            {
                talkDuration += Time.unscaledDeltaTime;
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
                    endTextEffect.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);

                    var characterPos = new Vector3(-824.4f, 32.39394f);
                    if (!string.IsNullOrWhiteSpace(descriptionText.text) && !descriptionText.text.Equals(string.Empty))
                    {
                        if (descriptionText.textInfo.characterInfo != null &&
                            descriptionText.textInfo.characterInfo.Length >= descriptionText.maxVisibleCharacters - 1)
                            characterPos = Utility.GetVector3Aver(
                                descriptionText.textInfo.characterInfo[descriptionText.maxVisibleCharacters - 1].topRight,
                                descriptionText.textInfo.characterInfo[descriptionText.maxVisibleCharacters - 1].bottomRight);
                    }

                    endTextEffect.rectTransform.anchoredPosition = new Vector2(characterPos.x + 30, characterPos.y - 5);
                }

                if (SaveManager.Instance.GameData.textAuto)
                {
                    autoDuration += Time.unscaledDeltaTime;
                    if (autoDuration >= autoWaitTime)
                    {
                        autoDuration -= autoWaitTime;
                        NewTalk();
                    }
                }
            }
        }

        private void Update()
        {
            TalkUpdate();
        }

        private void CheckClick()
        {
            if (descriptionText.maxVisibleCharacters < exampleText.Length)
                descriptionText.maxVisibleCharacters = exampleText.Length;
            else
                NewTalk();
        }

        private void NewTalk()
        {
            descriptionText.maxVisibleCharacters = 0;
            endTextEffect.gameObject.SetActive(false);
            talkDuration = 0;
            autoDuration = 0;
        }

        private void ChangeSfxSlider(float value)
        {
            SaveManager.Instance.GameData.sfxSound = value;
            SoundManager.Instance.UpdateVolume(ESoundType.SFX, value);
        }

        private void ChangeBgmSlider(float value)
        {
            SaveManager.Instance.GameData.bgmSound = value;
            SoundManager.Instance.UpdateVolume(ESoundType.BGM, value);
        }

        private void ChangeTextSpeedSlider(float value)
        {
            SaveManager.Instance.GameData.textSpeed = (1 - value) - 0.2f;
        }

        private void ChangeTextType()
        {
            SaveManager.Instance.GameData.textAuto = !SaveManager.Instance.GameData.textAuto;
            textTypeImage.sprite = SaveManager.Instance.GameData.textAuto ? textTypeSpriteOn : textTypeSpriteOff;
        }

        public override void Init(Image button)
        {
            base.Init(button);

            NamingSetting();
            SoundSetting();
            TextSetting();
            NewTalk();
        }

        private void TextSetting()
        {
            textSpeedSlider.value = 1 - (SaveManager.Instance.GameData.textSpeed + 0.2f);
            textTypeImage.sprite = SaveManager.Instance.GameData.textAuto ? textTypeSpriteOn : textTypeSpriteOff;
        }

        private void SoundSetting()
        {
            bgmSlider.value = SaveManager.Instance.GameData.bgmSound;
            sfxSlider.value = SaveManager.Instance.GameData.sfxSound;
        }

        private void NamingSetting()
        {
            namingInput.text = SaveManager.Instance.GameData.name;
        }

        private void EnterName()
        {
            //TODO SOUND
            SaveManager.Instance.GameData.name = string.IsNullOrEmpty(namingInput.text) ? "김준우" : namingInput.text;
            warningWindow.gameObject.SetActive(false);
        }
    }
}