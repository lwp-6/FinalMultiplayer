using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EventCode = Scripts.Weapon.EventCode;

public class HitManager : MonoBehaviour, IOnEventCallback
{

    public GameObject BulletImpactPrefab;
    public ImpactAudioData ImpactAudioData;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch ((EventCode)photonEvent.Code)
        {
            case EventCode.HitGround:
                Dictionary<byte, object> tmp_HitData = (Dictionary<byte, object>)photonEvent.CustomData;
                var tmp_HitPoint = (Vector3)tmp_HitData[0];
                var tmp_HitNormal = (Vector3)tmp_HitData[1];
                var tmp_HitTag = (string)tmp_HitData[2];

                // 对象池获取
                GameObject tmp_BulletEffect = ObjectPool.GetInstance().GetObj(BulletImpactPrefab.name);
                tmp_BulletEffect.transform.position = tmp_HitPoint;
                tmp_BulletEffect.transform.rotation = Quaternion.LookRotation(tmp_HitNormal, Vector3.up);
                // 对象池回收
                StartCoroutine(AutoRecycle(tmp_BulletEffect, 1.5f));

                // 声音
                AudioClip audioClip = ImpactAudioData.ImpactAudios[0];
                AudioSource.PlayClipAtPoint(audioClip, tmp_HitPoint);
 
                break;
        }
    }

    /// 一定时间自动回收到对象池
    IEnumerator AutoRecycle(GameObject obj, float recycleTime)
    {
        //Debug.Log(obj.name);
        yield return new WaitForSeconds(recycleTime);

        ObjectPool.GetInstance().RecycleObj(obj);
    }
}
