using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Scripts.Weapon
{
    public class AssualtRifle : Firearms
    {
        public GameObject BulletImpactPrefab;
        public ImpactAudioData ImpactAudioData;

        // 判断换弹动画是否结束的协程
        private IEnumerator reloadAmmoCheckerCoroutine;


        private FPMouseLook mouseLook;
        protected override void Awake()
        {
            base.Awake();
            reloadAmmoCheckerCoroutine = CheckReloadAmmoAnimationEnd();

            mouseLook = FindObjectOfType<FPMouseLook>();
            TakeOutWeaponAudioDelay = 0.5f;
            // 拿出武器音效，共用换弹的AudioSource
            FirearmsReloadAudioSource.clip = FirearmAudioData.TakeOutWeapon;
            FirearmsReloadAudioSource.PlayDelayed(TakeOutWeaponAudioDelay);
        }

        /*private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                DoAttack();
            }

            if (Input.GetKeyDown(KeyCode.R) && CurrentMaxAmmoCarried > 0)
            {
                Reload();
            }

            // 瞄准
            if (Input.GetMouseButtonDown(1))
            {
                IsAiming = true;
                Aim();
            }
            // 退出瞄准
            if (Input.GetMouseButtonUp(1))
            {
                IsAiming = false;
                Aim();
            }
        }*/

        protected override void Shooting()
        {
            // 控制射击间隔，换弹时不能射击     // 没子弹时不能射击
            if (!IsAllowShooting() || !isallowShoot || CurrentAmmo <= 0)
            {
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

            // 抛出子弹壳
            DropBulletsParticle.Play();

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

        /*protected override void Aim()
        {
            GunAnimator.SetBool("Aim", IsAiming);
            if (doAimCoroutine == null)
            {
                doAimCoroutine = DoAim();
                StartCoroutine(doAimCoroutine);
            }
            else
            {
                StopCoroutine(doAimCoroutine);
                doAimCoroutine = null;
                doAimCoroutine = DoAim();
                StartCoroutine(doAimCoroutine);
            }
        }*/


        // 生成子弹
        protected GameObject CreateBullet()
        {
            //******对象池
            // 普通生成prefab
            //GameObject tmp_Bullet = Instantiate(BulletPrefab, MuzzlePoint.position, MuzzlePoint.rotation);
            // 对象池方式生成prefab
            //Debug.Log(BulletPrefab.name.GetType());
            GameObject tmp_Bullet = ObjectPool.GetInstance().GetObj(BulletPrefab.name);
            tmp_Bullet.transform.position = MuzzlePoint.position;
            tmp_Bullet.transform.rotation = MuzzlePoint.rotation;
            //********

            // 设置参数
            tmp_Bullet.transform.eulerAngles += CalculateSpreadOffset();

            var tmp_BulletScript = tmp_Bullet.AddComponent<Bullet>();
            tmp_BulletScript.BulletImpactPrefab = BulletImpactPrefab;
            tmp_BulletScript.BulletSpeed = BulletSpeed;
            tmp_BulletScript.BulletPrefab = tmp_Bullet;
            tmp_BulletScript.ImpactAudioData = ImpactAudioData;

            //var tmp_BulletRigibody = tmp_Bullet.AddComponent<Rigidbody>();
            //tmp_BulletRigibody.velocity = tmp_Bullet.transform.forward * BulletSpeed;
            return tmp_Bullet;
        }
        // 判断换弹动画结束，补充子弹
        /*private IEnumerator CheckReloadAmmoAnimationEnd()
        {
            while (true)
            {
                yield return null;
                // 索引2，即Reload Layer
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(2);

                // 获取换弹动画状态
                if (GunStateInfo.IsTag("ReloadAmmo"))
                {
                    if (GunStateInfo.normalizedTime >= 0.9f)
                    {
                        int tmp_NeedAmmoCount = AmmoInMag - CurrentAmmo;
                        int tmp_RemaingAmmo = CurrentMaxAmmoCarried - tmp_NeedAmmoCount;
                        if (tmp_RemaingAmmo <= 0)
                        {
                            CurrentAmmo += CurrentMaxAmmoCarried;
                            CurrentMaxAmmoCarried = 0;
                        }
                        else
                        {
                            CurrentAmmo = AmmoInMag;
                            CurrentMaxAmmoCarried = tmp_RemaingAmmo;
                        }

                        // 换弹结束可以射击
                        allowShoot = true;
                        yield break;
                    }
                }
            }
        }

        private IEnumerator DoAim()
        {
            while (true)
            {
                yield return null;

                float tmp_CurrentFOV = 0;
                EyeCamera.fieldOfView = Mathf.SmoothDamp(EyeCamera.fieldOfView, 
                    IsAiming ? 26 : OriginFOV, 
                    ref tmp_CurrentFOV, 
                    Time.deltaTime * 2);
            }
        }*/
    }
}