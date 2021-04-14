using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleRoomManager : MonoBehaviour
{
    public string PlayerPrefabName;
    public Text CountDownText;

    public int life;
    private Transform[] RespawnPoints;
    private int RespawnIndex;
    
    private void Start()
    {
        StartSpawn(0);
        Player.Respawn += StartSpawn;

        RespawnPoints = transform.GetComponentsInChildren<Transform>();
        life = 3;

        // 判断房间满人
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
    }

    private void OnDisable()
    {
        Player.Respawn -= StartSpawn;
    }

    private void StartSpawn(float timeToSpawn)
    {
        StartCoroutine(WaitToInstantiatePlayer(timeToSpawn));
    }

    private IEnumerator WaitToInstantiatePlayer(float timeToSpawn)
    {
        CountDownText.gameObject.SetActive(true);
        while (timeToSpawn > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeToSpawn--;
            CountDownText.text = timeToSpawn.ToString();
        }
        CountDownText.gameObject.SetActive(false);

        yield return null;
        RespawnIndex = Random.Range(0, 8);
        // 生成角色
        PhotonNetwork.Instantiate(PlayerPrefabName, RespawnPoints[RespawnIndex].position, Quaternion.identity);
    }

}
