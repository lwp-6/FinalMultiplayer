using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoomManager : MonoBehaviour
{
    public string PlayerPrefabName;

    private Transform[] RespawnPoints;

    private void Start()
    {
        StartSpawn(0);
        Player.Respawn += StartSpawn;

        RespawnPoints = transform.GetComponentsInChildren<Transform>();
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

        // 生成角色
        PhotonNetwork.Instantiate(PlayerPrefabName, transform.position, Quaternion.identity);
    }
}
