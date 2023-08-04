using UI;
using UnityEngine;

public enum Scene
{
    Loading,
    Title,
    InGame
}

public class GameManager : Singleton<GameManager>
{
    protected override bool IsDontDestroying => true;

    public Camera UICamera => uiCamera;
    [SerializeField] private Camera uiCamera;

    [Header("매니저들")]
    public SceneManager sceneManager;
    public SaveManager saveManager;
    public PoolManager poolManager;
    public DialogManager dialogManager;
    public SoundManager soundManager;
    public ResourcesManager resourcesManager;
    public WindowManager windowManager;
    protected override void OnCreated()
    {
        Application.targetFrameRate = Application.platform == RuntimePlatform.Android ? 30 : 120;
        
        sceneManager.OnCreated();
        saveManager.OnCreated();
        dialogManager.OnCreated();
        soundManager.OnCreated();
        poolManager.OnCreated();
        resourcesManager.OnCreated();
        windowManager.OnCreated();
    }

    protected override void OnReset()
    {
        sceneManager.OnReset();
        saveManager.OnReset();
        dialogManager.OnReset();
        soundManager.OnReset();
        poolManager.OnReset();
        resourcesManager.OnReset();
        windowManager.OnReset();
    }

    private void Update()
    {
        ClickEffect();
    }

    private void ClickEffect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (resourcesManager.IsLoading) return;

            GameObject obj = poolManager.Init("Click Effect");
            Camera camera = Camera.main;
            var vector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vector.z = 0;
            obj.transform.position = vector;
        }
    }

}