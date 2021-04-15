using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class Player : MonoBehaviour, IDamage
{
    public int Health;
    public Image LifeBarImage;

    public Image ScreenFlashImage;
    public float FlashTime;
    public Color FlashColor;

    public Transform LifeNum;
    public PauseMenuController pauseMenuController;

    private Color OriginColor;

    public static event Action<float> Respawn;

    private PhotonView photonView;
    private GameObject GlobalCamera;
    private int maxHealth;

    private GameObject GlobalCanvas;

    private Image[] LifeTransforms;
    private BattleRoomManager battleRoomManager;

    // Start is called before the first frame update
    private void Start()
    {
        OriginColor = ScreenFlashImage.color;
        maxHealth = Health;
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            GlobalCamera = GameObject.FindWithTag("GlobalCamera");
            if (GlobalCamera != null)
            {
                GlobalCamera.SetActive(false);
            }

            GlobalCanvas = GameObject.FindWithTag("GlobalCanvas");
            if (GlobalCanvas != null)
            {
                GlobalCanvas.SetActive(false);
            }
        }

        // 玩家生命数量初始化
        battleRoomManager = FindObjectOfType<BattleRoomManager>();
        LifeTransforms = LifeNum.GetComponentsInChildren<Image>();

        for (int i = 0; i < battleRoomManager.life; ++i)
        {
            LifeTransforms[i].color = new Color(255, 255, 255, 255);
        }

    }

    // debug
    private void Update()
    {
        if (Input.GetKey(KeyCode.L))
        {
            TakeDamage(10);
            //Debug.Log(GlobleVar.isPause);
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1 && !PhotonNetwork.CurrentRoom.IsOpen)
        {
            StartCoroutine(isVictrory());
        }
    }

    // 判断胜利
    private IEnumerator isVictrory()
    {
        yield return new WaitForSeconds(4);

        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            pauseMenuController.FinishMenu();
        }
    }

    public void TakeDamage(int damage)
    {
        //Debug.Log("takeDamage");

        if (GlobleVar.isOnline)
        {
            photonView.RpcSecure("RPCTakeDamage", RpcTarget.All, true, damage);
        }
        //LifeBarImage.fillAmount = (float)Health / maxHealth;
        //FlashScreen();
    }
    

    [PunRPC]
    private void RPCTakeDamage(int damage, PhotonMessageInfo info)
    {
        if (IsDeath() && photonView.IsMine)
        {
            //gameObject.SetActive(false);

            PhotonNetwork.Destroy(this.gameObject);

            if (GlobalCamera != null)
            {
                GlobalCamera.SetActive(true);   
            }
            battleRoomManager.life--;
            if (GlobalCanvas != null)
            {
                GlobalCanvas.SetActive(true);
            }
            
            if (battleRoomManager.life > 0)
            {
                Respawn?.Invoke(3);
            }
            return;
        }
        Health -= damage;
        if (photonView.IsMine)
        {
            LifeBarImage.fillAmount = (float)Health / maxHealth;
            FlashScreen();
        }

    }

    private bool IsDeath()
    {
        return Health <= 0;
    }

    // 启动协程 闪烁
    public void FlashScreen()
    {
        StartCoroutine(Flash());
    }

    // 受伤屏幕闪烁协程
    IEnumerator Flash()
    {
        ScreenFlashImage.color = FlashColor;
        yield return new WaitForSeconds(FlashTime);
        ScreenFlashImage.color = OriginColor;
    }

}
