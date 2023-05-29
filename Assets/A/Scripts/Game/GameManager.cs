using System;
using DG.Tweening;
using Ingame;
using UnityEngine;

public enum Scene
{
    LOADING,
    TITLE,
    INGAME
}

public class GameManager : Singleton<GameManager>
{
    protected override bool IsDontDestroying => true;

    public Camera UICamera => uiCamera;
    [SerializeField] private Camera uiCamera;

    public SceneManager sceneManager;
    public SaveManager saveManager;
    protected override void OnCreated()
    {
        Application.targetFrameRate = Application.platform == RuntimePlatform.Android ? 30 : 120;
        
        sceneManager.OnCreated();
        saveManager.OnCreated();
    }

    protected override void OnReset()
    {
        sceneManager.OnReset();
        saveManager.OnReset();
    }

    private void Update()
    {
        ClickEffect();
    }

    private void ClickEffect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (ResourcesManager.Instance.IsLoading) return;

            GameObject obj = PoolManager.Instance.Init("Click Effect");
            var vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vector.z = 0;
            obj.transform.position = vector;
        }
    }

}