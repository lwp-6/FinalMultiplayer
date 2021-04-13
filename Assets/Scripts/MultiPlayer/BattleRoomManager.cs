using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoomManager : MonoBehaviour
{
    public string PlayerPrefabName;

    private Transform[] RespawnPoints;
    private int RespawnIndex;
    public int life;
    private void Start()
    {
        StartSpawn(0);
        Player.Respawn += StartSpawn;

        RespawnPoints = transform.GetComponentsInChildren<Transform>();
        life = 3;
    }

    /*private void Update()
    {
        if (Input.GetKey(KeyCode.O))
        {
            Debug.Log();
        }
    }*/

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
        yield return new WaitForSeconds(timeToSpawn);
        RespawnIndex = Random.Range(0, 8);
        // 生成角色
        PhotonNetwork.Instantiate(PlayerPrefabName, RespawnPoints[RespawnIndex].position, Quaternion.identity);
    }

}
