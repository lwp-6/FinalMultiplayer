using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuController : MonoBehaviour
{
    public Transform PauseMenuTransform;
    public Transform GameOverMenuTransform;
    public Transform FinishMenuTransform;

    private Scene CurrentScene;

    // Start is called before the first frame update
    void Start()
    {
        CurrentScene = SceneManager.GetActiveScene();
        GlobleVar.isPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool isGameOver = GameOverMenuTransform.gameObject.activeSelf;
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            GlobleVar.isPause = !GlobleVar.isPause;
            PauseMenu(GlobleVar.isPause);
        }

    }

    // 玩家完成
    public void FinishMenu()
    {
        // 暂停
        GlobleVar.isPause = true;

        // 显示菜单
        FinishMenuTransform.gameObject.SetActive(true);

        // 解锁鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 玩家死亡，
    public void GameOver()
    {
        // 暂停
        GlobleVar.isPause = true;

        // 显示菜单
        GameOverMenuTransform.gameObject.SetActive(true);

        // 解锁鼠标
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // 暂停菜单
    public void PauseMenu(bool flag)
    {
        // 暂停游戏
        GlobleVar.isPause = flag;

        // 菜单是否显示
        PauseMenuTransform.gameObject.SetActive(flag);
        // 显示,解锁 鼠标
        Cursor.visible = flag;
        Cursor.lockState = flag ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void ContinueGame()
    {
        GlobleVar.isPause = false;
        PauseMenu(false);
    }

    // 重新开始
    public void ReStartGame()
    {
        SceneManager.LoadScene(CurrentScene.name);
        ObjectPool.GetInstance().ClearObjectPool();
        GlobleVar.isPause = false;

        // 隐藏，锁定 鼠标
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // 回到主菜单
    public void MainMenu()
    {
        SceneManager.LoadScene("MenuScene");
        ObjectPool.GetInstance().ClearObjectPool();

        if (GlobleVar.isJoinRoom)
        {
            PhotonNetwork.LeaveRoom();
            GlobleVar.isJoinRoom = false;
        }
        
    }
}
