using UnityEngine;
using UnityEditor;
using System.Collections;
using Photon.Pun;
using System.Collections.Generic;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace Scripts.Weapon
{
    public class Bullet : MonoBehaviour
    {
        public float BulletSpeed;

        public GameObject BulletImpactPrefab;
        public ImpactAudioData ImpactAudioData;
        public GameObject BulletPrefab;
        public int damage = 10;

        private Transform bulletTransform;
        private Vector3 prePosition;
        private bool isExist;
        private void Start()
        {
            bulletTransform = transform;
            prePosition = bulletTransform.position;
            isExist = true;
        }

        private void Update()
        {
            // 暂停
            if (GlobleVar.isPause || !isExist)
            {
                return;
            }

            // 子弹移动
            prePosition = bulletTransform.position;
            bulletTransform.Translate(0, 0, BulletSpeed * Time.deltaTime);

            // 子弹碰撞
            if (Physics.Raycast(prePosition,
                (bulletTransform.position - prePosition).normalized,
                out RaycastHit tmp_Hit,
                (bulletTransform.position - prePosition).magnitude)){

                var tmp_Damage = tmp_Hit.collider.GetComponent<IDamage>();
                if (tmp_Damage != null)
                {
                    tmp_Damage.TakeDamage(10);
                }

                if (!GlobleVar.isJoinRoom)   //单机
                {
                    BulletHitProcess(tmp_Hit);
                }
                else    //多人
                {
                    // 同步碰撞信息
                    Dictionary<byte, object> tmp_HitData = new Dictionary<byte, object>
                    {
                        { 0, tmp_Hit.point },
                        { 1, tmp_Hit.normal },
                        { 2, tmp_Hit.collider.tag }
                    };

                    RaiseEventOptions tmp_RaiseEventOptions = new RaiseEventOptions()
                    {
                        Receivers = ReceiverGroup.All
                    };
                    SendOptions tmp_SendOptions = SendOptions.SendReliable;
                    PhotonNetwork.RaiseEvent((byte)EventCode.HitGround, tmp_HitData, tmp_RaiseEventOptions, tmp_SendOptions);
                }

                // 对象池回收子弹
                StartCoroutine(AutoRecycle(BulletPrefab, 2.5f));
            }
        }

        private void BulletHitProcess(RaycastHit tmp_Hit)
        {
            //Debug.Log(tmp_Hit.transform.position);
            // HP减少
            HPManager hpManager = tmp_Hit.collider.GetComponent<HPManager>();
            if (hpManager != null)
            {
                hpManager.HPDown(damage);
            }

            Transform colliderTransform = tmp_Hit.collider.transform;

            // 敌人击中效果
            if (colliderTransform.CompareTag("Enemy"))
            {
                Enemy enemy = colliderTransform.parent.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Damage();
                }
            }
            
            // 玩家击中效果
            if (colliderTransform.CompareTag("Player"))
            {
                PlayerHpController playerHpController = colliderTransform.GetComponent<PlayerHpController>();
                playerHpController.Damage(0.1f);
            }

            // 子弹击中效果
            // 生成
            //var tmp_BulletEffect = Instantiate(BulletImpactPrefab, tmp_Hit.point, Quaternion.LookRotation(tmp_Hit.normal, Vector3.up));
            //Destroy(tmp_BulletEffect, 3);

            isExist = false;
            // 对象池获取
            GameObject tmp_BulletEffect = ObjectPool.GetInstance().GetObj(BulletImpactPrefab.name);
            tmp_BulletEffect.transform.position = tmp_Hit.point;
            tmp_BulletEffect.transform.rotation = Quaternion.LookRotation(tmp_Hit.normal, Vector3.up);
            // 对象池回收
            StartCoroutine(AutoRecycle(tmp_BulletEffect, 1.5f));


            // 声音
            AudioClip audioClip = ImpactAudioData.ImpactAudios[0];
            AudioSource.PlayClipAtPoint(audioClip, tmp_Hit.point);

            // 销毁子弹
            //Destroy(BulletPrefab);
        }

        /// 一定时间自动回收到对象池
        IEnumerator AutoRecycle(GameObject obj, float recycleTime)
        {
            //Debug.Log(obj.name);
            yield return new WaitForSeconds(recycleTime);
            
            ObjectPool.GetInstance().RecycleObj(obj);
        }
    }
}