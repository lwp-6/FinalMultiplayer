using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityTemplateProijects.UI.Models;


public class UIManager : MonoBehaviour
{
    public MapModelsScriptableObject MapModelsScriptableObject;

    public CanvasGroup MenuItemCanvasGroup;
    public Button PlayButton;
    public Button CreateGameButton;
    public Button JoinGameButton;
    public Button ExitGameButton;

    [Space] // 创建房间菜单
    public Animator CreateGamePanelAnimator;
    public Button CancelCreateGameButton;
    public Button ConfirmCreateGameButton;

    public InputField RoomName;
    public InputField RoomPassword;
    public Dropdown MaxPlayer;
    public string MapName;

    [Space] // 加入房间菜单
    public Animator JoinGamePanelAnimator;
    public Button CancelJoinGameButton;
    public Button ComfirmJoinGameButton;
    public Image SelectedMapImage;
    public Text SelectedRoomName;
    public GameObject PasswordInput;
    public Text WrongPsssword;
    public Text RoomPlayerFull;
    public Text RoomPlaying;

    public Launcher launcher;

    private string SelectedroomPassword;
    private void Start()
    {
        // 单机模式
        PlayButton.onClick.AddListener(() => {
            SceneManager.LoadScene("Game0");
        });

        // 创建房间
        CreateGameButton.onClick.AddListener(() => {
            CreateGamePanelAnimator.SetTrigger("FadeIn");
            MenuItemCanvasGroup.interactable = false;
        });
        // 取消创建房间菜单
        CancelCreateGameButton.onClick.AddListener(() => {
            CreateGamePanelAnimator.SetTrigger("FadeOut");
            MenuItemCanvasGroup.interactable = true;

        });
        // 弹出加入房间菜单
        JoinGameButton.onClick.AddListener(() => {
            JoinGamePanelAnimator.SetTrigger("FadeIn");
            MenuItemCanvasGroup.interactable = false;
        });
        // 取消加入房间菜单
        CancelJoinGameButton.onClick.AddListener(() => {
            JoinGamePanelAnimator.SetTrigger("FadeOut");
            MenuItemCanvasGroup.interactable = true;
            SelectedRoomName.text = null;
            SelectedMapImage.sprite = null;

            // 恢复
            RoomPlayerFull.gameObject.SetActive(false);
            RoomPlaying.gameObject.SetActive(false);
            ComfirmJoinGameButton.gameObject.SetActive(true);
        });
        // 确认创建房间
        ConfirmCreateGameButton.onClick.AddListener(() =>
        {
            //Debug.Log(int.Parse(MaxPlayer.options[MaxPlayer.value].text));
            launcher.CreateRoom(RoomName.text, (byte)(int.Parse(MaxPlayer.options[MaxPlayer.value].text)), RoomPassword.text, MapName);
        });
        // 确认加入房间
        ComfirmJoinGameButton.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(SelectedRoomName.text))
            {
                return;
            }
            string passwrod = PasswordInput.GetComponentInChildren<InputField>().text;
            if (passwrod != SelectedroomPassword)
            {
                WrongPsssword.gameObject.SetActive(true);
                StartCoroutine(PasswordWrongVanish());
                return;
            }

            PhotonNetwork.JoinRoom(SelectedRoomName.text);
        });

        // 退出游戏
        ExitGameButton.onClick.AddListener(() =>
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        });

    }

    public void SetSelectedRoomDetails(RoomInfo roomInfo)
    {
        Debug.Log(roomInfo.CustomProperties);
        var tmp_MatchModels = MapModelsScriptableObject.MapModels.Find(mapModel => {
            return mapModel.MapName.CompareTo(roomInfo.CustomProperties["Map"]) == 0;
        });
        SelectedRoomName.text = roomInfo.Name;
        SelectedMapImage.sprite = tmp_MatchModels.MapSprite;
        SelectedroomPassword = (string)roomInfo.CustomProperties["psw"];
        if ((string)roomInfo.CustomProperties["psw"] != "")
        {
            PasswordInput.SetActive(true);
        }
        else
        {
            PasswordInput.SetActive(false);
        }

        // 玩家人数上限
        if (roomInfo.PlayerCount == roomInfo.MaxPlayers)
        {
            //PhotonNetwork.CurrentRoom.IsOpen = false;
            ComfirmJoinGameButton.gameObject.SetActive(false);
            RoomPlayerFull.gameObject.SetActive(true);
            return;
        }
        else
        {
            ComfirmJoinGameButton.gameObject.SetActive(true);
            RoomPlayerFull.gameObject.SetActive(false);
        }

        // 房间游戏中(不开放)
        if (!roomInfo.IsOpen)
        {
            ComfirmJoinGameButton.gameObject.SetActive(false);
            RoomPlaying.gameObject.SetActive(true);
        }
        else
        {
            ComfirmJoinGameButton.gameObject.SetActive(true);
            RoomPlaying.gameObject.SetActive(false);
        }
    }

    IEnumerator PasswordWrongVanish()
    {
        yield return new WaitForSeconds(1.0f);
        WrongPsssword.gameObject.SetActive(false);
    }
}
