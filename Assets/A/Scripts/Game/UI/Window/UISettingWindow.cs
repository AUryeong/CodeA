﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI
{
    public class UISettingWindow : UIWindow
    {
        [SerializeField] private Button exitButton;
        [Header("사운드")] 
        
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private Slider bgmSlider;

        [Header("이름 설정")] [SerializeField] private TMP_InputField namingInput;

        [SerializeField] private Image warningWindow;
        [SerializeField] private TextMeshProUGUI warningText;
        [SerializeField] private Button warningOkay;
        [SerializeField] private Button warningCancel;

        [Header("텍스트 출력")] [SerializeField] private Slider textSpeedSlider;

        [SerializeField] private Button textTypeToggle;
        private Image textTypeImage;
        [SerializeField] private Sprite textTypeSpriteOn;
        [SerializeField] private Sprite textTypeSpriteOff;

        [Space(20f)] [SerializeField] private Button dialogUI;

        [SerializeField] private TextMeshProUGUI descriptionText;

        [SerializeField] private TextMeshProUGUI endTextEffect;

        private float dialogDuration;
        private const float defaultDialogCooltime = 0.05f;

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

            dialogUI.onClick.RemoveAllListeners();
            dialogUI.onClick.AddListener(CheckClick);

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(GameManager.Instance.windowManager.CloseAllWindow);

            namingInput.onEndEdit.AddListener((text) =>
            {
                if (text == GameManager.Instance.saveManager.GameData.name) return;

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

        private void DialogUpdate()
        {
            if (descriptionText.maxVisibleCharacters < descriptionText.text.Length)
            {
                dialogDuration += Time.unscaledDeltaTime;
                float cooltime = defaultDialogCooltime * GameManager.Instance.saveManager.GameData.textSpeed;
                if (dialogDuration >= cooltime)
                {
                    dialogDuration -= cooltime;
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

                if (GameManager.Instance.saveManager.GameData.textAuto)
                {
                    autoDuration += Time.unscaledDeltaTime;
                    if (autoDuration >= autoWaitTime)
                    {
                        autoDuration -= autoWaitTime;
                        NewDialog();
                    }
                }
            }
        }

        private void Update()
        {
            DialogUpdate();
        }

        private void CheckClick()
        {
            if (descriptionText.maxVisibleCharacters < exampleText.Length)
                descriptionText.maxVisibleCharacters = exampleText.Length;
            else
                NewDialog();
        }

        private void NewDialog()
        {
            descriptionText.maxVisibleCharacters = 0;
            endTextEffect.gameObject.SetActive(false);
            dialogDuration = 0;
            autoDuration = 0;
        }

        private void ChangeSfxSlider(float value)
        {
            GameManager.Instance.saveManager.GameData.sfxSoundMultiplier = value;
            GameManager.Instance.soundManager.UpdateVolume(ESoundType.Sfx, value);
        }

        private void ChangeBgmSlider(float value)
        {
            GameManager.Instance.saveManager.GameData.bgmSoundMultiplier = value;
            GameManager.Instance.soundManager.UpdateVolume(ESoundType.Bgm, value);
        }

        private void ChangeTextSpeedSlider(float value)
        {
            GameManager.Instance.saveManager.GameData.textSpeed = (1 - value) * 1.8f - 0.2f;
        }

        private void ChangeTextType()
        {
            GameManager.Instance.saveManager.GameData.textAuto = !GameManager.Instance.saveManager.GameData.textAuto;
            textTypeImage.sprite = GameManager.Instance.saveManager.GameData.textAuto ? textTypeSpriteOn : textTypeSpriteOff;
        }

        public override void Init(Vector3 pos)
        {
            base.Init(pos);

            NamingSetting();
            SoundSetting();
            TextSetting();
            NewDialog();
        }

        private void TextSetting()
        {
            textSpeedSlider.value = (1 - (GameManager.Instance.saveManager.GameData.textSpeed + 0.2f)/1.8f);
            textTypeImage.sprite = GameManager.Instance.saveManager.GameData.textAuto ? textTypeSpriteOn : textTypeSpriteOff;
        }

        private void SoundSetting()
        {
            bgmSlider.value = GameManager.Instance.saveManager.GameData.bgmSoundMultiplier;
            sfxSlider.value = GameManager.Instance.saveManager.GameData.sfxSoundMultiplier;
        }

        private void NamingSetting()
        {
            namingInput.text = GameManager.Instance.saveManager.GameData.name;
        }

        private void EnterName()
        {
            //TODO SOUND
            GameManager.Instance.saveManager.GameData.name = string.IsNullOrEmpty(namingInput.text) ? "김준우" : namingInput.text;
            warningWindow.gameObject.SetActive(false);
        }
    }
}