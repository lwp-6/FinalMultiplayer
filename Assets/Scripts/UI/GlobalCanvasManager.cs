using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GlobalCanvasManager : MonoBehaviour
{
    public Transform LifeNum;
    public Text CountDownText;
    public GameObject FailMenu;

    private Button MenuButton;
    private Image[] LifeTransforms;
    private BattleRoomManager battleRoomManager;

    private void Awake()
    {
        LifeTransforms = LifeNum.GetComponentsInChildren<Image>();
        battleRoomManager = FindObjectOfType<BattleRoomManager>();
        MenuButton = FailMenu.GetComponentInChildren<Button>();

        MenuButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("MenuScene");
            ObjectPool.GetInstance().ClearObjectPool();

            if (GlobleVar.isJoinRoom)
            {
                PhotonNetwork.LeaveRoom();
                GlobleVar.isJoinRoom = false;
            }
        });
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        CountDownText.text = "3";

        LifeTransforms[Mathf.Clamp(battleRoomManager.life, 0, 2)].color = new Color(255, 255, 255, 0);
        FailMenu.SetActive(false);
        
        if (battleRoomManager.life == 0)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FailMenu.SetActive(true);
        }
    }
}
