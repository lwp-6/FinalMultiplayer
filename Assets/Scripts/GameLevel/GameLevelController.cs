using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLevelController : MonoBehaviour
{
    // 敌人数量
    public int EnemyNum;

    public PauseMenuController pauseMenuController;

    private string CurrentSenceName;
    private Scene CurrentScene;
    // Start is called before the first frame update
    void Start()
    {
        CurrentScene = SceneManager.GetActiveScene();
        CurrentSenceName = CurrentScene.name;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log(GetNextGameLevelName(CurrentSenceName));
        }

        if (EnemyNum == 0)
        {
            pauseMenuController.FinishMenu();
        }
    }

    public void NextScene()
    {
        string nextScene = GetNextGameLevelName(CurrentSenceName);
        SceneManager.LoadScene(nextScene);
        ObjectPool.GetInstance().ClearObjectPool();
    }

    public string GetNextGameLevelName(string CurrentGameLevelName)
    {
        int GameLevelNum, len;
        string nextGameLevelName;
        len = CurrentGameLevelName.Length;
        if (len != GlobleVar.GameLevelNameLen)
        {
            Debug.LogError("GameLevelName Error");
            return "MenuScene";
        }
        nextGameLevelName = CurrentGameLevelName.Substring(0, len - 1);
        GameLevelNum = CurrentGameLevelName[len - 1] - 48 + 1;
        Debug.Log(GameLevelNum);
        if (GameLevelNum > GlobleVar.TotalGameLevel)
        {
            Debug.Log("No more Game");
            return "MenuScene";
        }
        nextGameLevelName += GameLevelNum;
        return nextGameLevelName;
    }
}
