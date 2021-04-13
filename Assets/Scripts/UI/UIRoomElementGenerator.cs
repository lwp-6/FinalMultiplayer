using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System;

public class UIRoomElementGenerator : MonoBehaviour
{
    public GameObject RoomUIElementPrefab;
    public Transform RoomUIElementRoot;
    public UIRoomListManager uiRoomListManager;

    public Dictionary<string, GameObject> roomUIElementDictionary = new Dictionary<string, GameObject>();

    private bool isDisplaying;

    // 生成房间列表的每一行
    public void GenerateRoomUI(RoomInfo roomInfo)
    {
        if (roomUIElementDictionary.ContainsKey(roomInfo.Name))
        {
            return;
        }
        var tmp_RoomUI = Instantiate(RoomUIElementPrefab, RoomUIElementRoot);
        var tmp_Transform = tmp_RoomUI.transform;
        tmp_Transform.localPosition = Vector3.zero;
        tmp_Transform.localScale = Vector3.one;
        tmp_Transform.localRotation = Quaternion.identity;

        // 缓存当前房间列表的每一行GameObject
        roomUIElementDictionary.Add(roomInfo.Name, tmp_RoomUI);

        // 设置房间信息
        var tmp_UIRoomElementScript = tmp_RoomUI.GetComponent<UIRoomElement>();
        tmp_UIRoomElementScript.SetElementDetails(roomInfo);

        // 房间信息显示在左边
        tmp_UIRoomElementScript.OnSelectedRoom = FindObjectOfType<UIManager>().SetSelectedRoomDetails;

    }

    public void RemovedRoomUI(RoomInfo roomInfo)
    {

        if (roomUIElementDictionary.TryGetValue(roomInfo.Name, out GameObject tmp_GameObject))
        {
            Destroy(tmp_GameObject);
            roomUIElementDictionary.Remove(roomInfo.Name);
        }
    }


    public void StartGenerateRoomUI()
    {
        isDisplaying = true;
        foreach (KeyValuePair<string, RoomInfo> tmp_Info in uiRoomListManager.GetRoomList)
        {
            GenerateRoomUI(tmp_Info.Value);
        }
    }

    public void RoomGeneratorHiding()
    {
        isDisplaying = false;
        foreach (KeyValuePair<string, GameObject> tmp_RoomUI in roomUIElementDictionary)
        {
            Destroy(tmp_RoomUI.Value);
        }
        roomUIElementDictionary.Clear();
    }

    private void OnEnable()
    {
        uiRoomListManager.OnRoomRemoved += OnRoomRemoved;
        uiRoomListManager.OnRoomAdded += OnRoomAdded;
    }

    private void OnRoomAdded(RoomInfo roomInfo)
    {
        if (!isDisplaying)
        {
            return;
        }
        GenerateRoomUI(roomInfo);
    }

    private void OnRoomRemoved(RoomInfo roomInfo)
    {
        if (!isDisplaying)
        {
            return;
        }
        RemovedRoomUI(roomInfo);
    }
}
