using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Scripts.Weapon
{
    public class HandGun : Firearms
    {
        public GameObject BulletImpactprefab;


        // 判断换弹动画是否结束的协程
        private IEnumerator reloadAmmoCheckerCoroutine;


        private FPMouseLook mouseLook;
        protected override void Awake()
        {
            base.Awake();
            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();

            mouseLook = FindObjectOfType<FPMouseLook>();
            TakeOutWeaponAudioDelay = 0.0f;
            // 拿出武器音效，共用换弹的AudioSource
            FirearmsReloadAudioSource.clip = FirearmAudioData.TakeOutWeapon;
            FirearmsReloadAudioSource.PlayDelayed(TakeOutWeaponAudioDelay);
        }


        protected override void Shooting()
        {
            if (!IsAllowShooting() || !isallowShoot)
            {
                return;
            }

            if (CurrentAmmo <= 0)
            {
                if (CurrentMaxAmmoCarried > 0)
                {
                    Reload();
                }
                return;
            }

            --CurrentAmmo;

            // 枪震动 动画
            GunAnimator.Play("Fire", IsAiming ? 1 : 0, 0);

            // 枪口火光
            MuzzleParticle.Play();
            CasingParticle.Play();

            // 子弹
            CreateBullet();
            lastFireTime = Time.time;

            // 声音

            FirearmsShootingAudioSource.clip = FirearmAudioData.ShootingAudio;
            FirearmsShootingAudioSource.Play();

            // 后坐力
            mouseLook.FringForTest();
        }
        protected override void Reload()
        {
            isallowShoot = false;

            // 设置动画层， reload层索引2，权重设置为1
            GunAnimator.SetLayerWeight(2, 1);
            GunAnimator.SetTrigger(CurrentAmmo > 0 ? "ReloadLeft" : "ReloadOutOf");

            // 判断当前是否在播放动画
            if (reloadAmmoCheckerCoroutine == null)
            {
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }
            else
            {
                StopCoroutine(reloadAmmoCheckerCoroutine);
                reloadAmmoCheckerCoroutine = null;
                reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();
                StartCoroutine(reloadAmmoCheckerCoroutine);
            }

            // 声音

            FirearmsReloadAudioSource.clip = CurrentAmmo > 0 ? FirearmAudioData.ReloadLeft : FirearmAudioData.ReloadOutOf;
            FirearmsReloadAudioSource.Play();
        }



        // 生成子弹
        protected GameObject CreateBullet()
        {
            //GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            GameObject tmp_Bullet = ObjectPool.GetInstance().GetObj(BulletPrefab.name);
            tmp_Bullet.transform.position = MuzzlePoint.position;
            tmp_Bullet.transform.rotation = MuzzlePoint.rotation;

            // 设置参数
            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();

            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            tmp_BulletScript.BulletImpactPrefab = BulletImpactprefab;
            tmp_BulletScript.BulletSpeed = BulletSpeed;
            tmp_BulletScript.BulletPrefab = tmp_Bullet;

            //var tmp_BulletRigibody = tmp_Bullet.AddComponent<Rigidbody>();
            //tmp_BulletRigibody.velocity = tmp_Bullet.transform.forward * BulletSpeed;
            return tmp_Bullet;
        }

    }
}