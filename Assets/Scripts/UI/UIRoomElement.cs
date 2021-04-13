using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRoomElement : MonoBehaviour, IPointerDownHandler
{
    //public Image PwdIcon;
    public Text RoomName;
    public Text NumOfPlayer;
    public Toggle HasPassword;

    public Action<RoomInfo> OnSelectedRoom;
    private RoomInfo roomInfo;

    // 点击回调
    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log(RoomName);

        if (OnSelectedRoom != null)
        {
            OnSelectedRoom.Invoke(roomInfo);
        }
    }

    public void SetElementDetails(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        RoomName.text = roomInfo.Name;
        NumOfPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";

        if ((string)roomInfo.CustomProperties["psw"] != "")  // 有密码
        {
            HasPassword.isOn = true;
        }
        else
        {
            HasPassword.isOn = false;
        }
    }
}
