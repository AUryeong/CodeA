using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : Manager
{
    public Scene nowScene { get; private set; }

    [Header("�� ��ȯ")]
    [SerializeField] private SpriteRenderer sceneTransitionBlack;
    [SerializeField] private MeshRenderer sceneTransitionSquare;
    private bool sceneLoading;
    public override void OnCreated()
    {
        SetResolution(GameManager.Instance.UICamera);
    }
    public override void OnReset()
    {
        if (nowScene == Scene.LOADING) return;

        SetResolution(Camera.main);
        foreach (var canvas in FindObjectsOfType<Canvas>())
            canvas.worldCamera = GameManager.Instance.UICamera;

        SceneLoadFadeOut();
    }

    public void SceneLoad(Scene scene)
    {
        if (sceneLoading) return;

        nowScene = scene;
        SceneLoadFadeIn(() => UnityEngine.SceneManagement.SceneManager.LoadScene((int)scene));
    }

    public void SceneLoadFadeIn(Action action)
    {
        sceneTransitionBlack.gameObject.SetActive(true);
        sceneTransitionSquare.gameObject.SetActive(true);

        sceneTransitionSquare.DOKill();
        sceneTransitionSquare.transform.localScale = Vector3.one * 20;
        sceneTransitionSquare.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            action?.Invoke();
        });
    }

    public void SceneLoadFadeOut(float delay = 0)
    {
        sceneTransitionBlack.gameObject.SetActive(true);
        sceneTransitionSquare.gameObject.SetActive(true);

        sceneTransitionSquare.DOKill();
        sceneTransitionSquare.transform.localScale = Vector3.zero;
        sceneTransitionSquare.transform.DOScale(Vector3.one * 20, 0.5f).SetDelay(delay).OnComplete(() =>
        {
            sceneTransitionBlack.gameObject.SetActive(false);
            sceneTransitionSquare.gameObject.SetActive(false);
        });
    }

    private void SetResolution(Camera changeCamera)
    {
        if (changeCamera == null) return;
        int setWidth = 1920;
        int setHeight = 1080;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);

        float screenMultiplier = (float)setWidth / setHeight;
        float deviceMultiplier = (float)deviceWidth / deviceHeight;

        if (screenMultiplier < deviceMultiplier)
        {
            float newWidth = screenMultiplier / deviceMultiplier;
            changeCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = deviceMultiplier / screenMultiplier;
            changeCamera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}
