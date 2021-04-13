using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpController : MonoBehaviour
{
    public Image LifeBarImage;
    public Image ScreenFlashImage;
    public float FlashTime;
    public Color FlashColor;
    public PauseMenuController PauseMenuController;

    private Color OriginColor;
    private HPManager hpManager;
    // Start is called before the first frame update
    void Start()
    {
        hpManager = GetComponent<HPManager>();
        OriginColor = ScreenFlashImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        // 暂停
        if (GlobleVar.isPause)
        {
            return;
        }

        if (hpManager.HP <= 0 && !GlobleVar.isPause)
        {
            Died();
        }
    }

    // 玩家击中效果
    public void Damage(float damage)
    {
        // UI变化
        LifeBarImage.fillAmount -= damage;

        // 屏幕受伤提示
        FlashScreen();
    }

    // 玩家死亡，游戏结束
    private void Died()
    {
        // 死亡界面，选择界面
        PauseMenuController.GameOver();
    }

    // 启动协程 闪烁
    public void FlashScreen()
    {
        StartCoroutine(Flash());
    }

    // 闪烁协程
    IEnumerator Flash()
    {
        ScreenFlashImage.color = FlashColor;
        yield return new WaitForSeconds(FlashTime);
        ScreenFlashImage.color = OriginColor;
    }
}
