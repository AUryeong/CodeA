﻿using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISaveWindow : UIWindow
    {
        [SerializeField] private Button exitButton;

        [SerializeField] private Button[] saves;
        private readonly List<TextMeshProUGUI> saveTitles = new List<TextMeshProUGUI>();
        private readonly List<TextMeshProUGUI> saveDates = new List<TextMeshProUGUI>();
        [SerializeField] private Button savePrevButton;
        [SerializeField] private Button saveNextButton;
        [SerializeField] private TextMeshProUGUI savePageText;

        [SerializeField] private Image warningWindow;

        [SerializeField] private Image warningWindow2;
        [SerializeField] private Button warningOkayButton;
        [SerializeField] private Button warningCancelButton;

        private int savesIdx;

        public override void OnCreated()
        {
            base.OnCreated();

            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(WindowManager.Instance.CloseScrollAndWindow);

            foreach (var button in saves)
            {
                saveTitles.Add(button.transform.GetChild(1).GetComponent<TextMeshProUGUI>());
                saveDates.Add(button.transform.GetChild(2).GetComponent<TextMeshProUGUI>());
            }

            saveNextButton.onClick.RemoveAllListeners();
            saveNextButton.onClick.AddListener(NextSaves);

            savePrevButton.onClick.RemoveAllListeners();
            savePrevButton.onClick.AddListener(PrevSaves);

            savesIdx = 0;
        }

        public override void Init(Image button)
        { 
            base.Init(button);
            warningWindow.gameObject.SetActive(GameManager.Instance.nowGameData == null);
            warningWindow2.gameObject.SetActive(false);
            
            warningCancelButton.onClick.RemoveAllListeners();
            warningCancelButton.onClick.AddListener(() => warningWindow2.gameObject.SetActive(false));
            
            ReloadSaves();
        }

        private void ReloadSaves()
        {
            savePageText.text = (savesIdx + 1).ToString();
            for (int i = 0; i < saves.Length; i++)
            {
                int idx = i + (savesIdx * 6);
                saves[i].onClick.RemoveAllListeners();
                var getSaveData = SaveManager.Instance.GameData.GetSaveData(idx);
                if (getSaveData != null)
                {
                    if (getSaveData.leftTalks.Count <= 0 || string.IsNullOrEmpty(getSaveData.leftTalks[0].background.name))
                        saves[i].image.sprite = null;
                    else
                        saves[i].image.sprite = ResourcesManager.Instance.GetBackground(getSaveData.leftTalks[0].background.name);

                    saveTitles[i].gameObject.SetActive(true);
                    saveTitles[i].text = $"{getSaveData.year}년 {getSaveData.month}월 {getSaveData.week}주 {Utility.GetTimeToString(getSaveData.time)}";

                    saveDates[i].gameObject.SetActive(true);
                    saveDates[i].text = getSaveData.saveTime;
                    
                    saves[i].onClick.AddListener(() => WarningSave(idx));
                }
                else
                {
                    saves[i].image.sprite = null;
                    saveTitles[i].gameObject.SetActive(false);
                    saveDates[i].gameObject.SetActive(false);
                    
                    saves[i].onClick.AddListener(() => Save(idx));
                }
            }
        }

        private void WarningSave(int idx)
        {
            warningWindow2.gameObject.SetActive(true);
            warningOkayButton.onClick.RemoveAllListeners();
            warningOkayButton.onClick.AddListener(() => Save(idx));
        }

        private void Save(int idx)
        {
            warningWindow2.gameObject.SetActive(false);
            SaveManager.Instance.GameData.ChangeSaveData(idx);
            ReloadSaves();
        }


        private void NextSaves()
        {
            savesIdx++;
            ReloadSaves();
        }

        private void PrevSaves()
        {
            if (savesIdx <= 0)
            {
                WindowManager.Instance.CloseAllWindow();
                return;
            }

            savesIdx--;
            ReloadSaves();
        }
    }
}