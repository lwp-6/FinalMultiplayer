using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class UIRoomListManager : MonoBehaviourPunCallbacks
{
    // 缓存房间列表
    private Dictionary<string, RoomInfo> roomInfoDictionary = new Dictionary<string, RoomInfo>();
    public Dictionary<string, RoomInfo> GetRoomList => roomInfoDictionary;

    public event Action<RoomInfo> OnRoomRemoved;
    public event Action<RoomInfo> OnRoomAdded;

    // 房间更新回调
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        foreach(RoomInfo roomInfo in roomList)
        {
            // 房间删除
            if (roomInfo.RemovedFromList)
            {
                if (roomInfoDictionary.ContainsKey(roomInfo.Name))
                {
                    roomInfoDictionary.Remove(roomInfo.Name);
                    OnRoomRemoved?.Invoke(roomInfo);
                }
                return;
            }

            // 更新已有房间信息
            if (roomInfoDictionary.ContainsKey(roomInfo.Name))
            {                
                if (roomInfo.PlayerCount == roomInfo.MaxPlayers || 
                    (bool)roomInfoDictionary[roomInfo.Name].CustomProperties["isPlaying"])
                {
                    roomInfo.CustomProperties["isPlaying"] = true;
                }
                roomInfoDictionary[roomInfo.Name] = roomInfo;

                continue;
            }

            // 新增房间
            roomInfoDictionary.Add(roomInfo.Name, roomInfo);
            OnRoomAdded?.Invoke(roomInfo);
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (GUILayout.Button("Debug Room List"))
        {
            foreach (KeyValuePair<string, RoomInfo> roomInfo in roomInfoDictionary)
            {
                Debug.Log(roomInfo.Key);
            }
        }
    }
#endif
}
