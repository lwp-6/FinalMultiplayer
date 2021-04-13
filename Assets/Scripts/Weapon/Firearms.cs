using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Weapon
{
    // 枪械
    public abstract class Firearms:MonoBehaviour, IWeapon
    {
        // 枪口、子弹特效位置
        public Transform MuzzlePoint;
        public Transform CasingPoint;

        // 枪口特效
        public ParticleSystem MuzzleParticle;
        public ParticleSystem CasingParticle;
        public ParticleSystem DropBulletsParticle;

        // 射速(1s多少子弹)、子弹速度
        public float FireRate;
        public float BulletSpeed;

        // 弹夹
        public int AmmoInMag = 30;
        public int MaxAmmoCarried = 120;
        // 子弹
        public GameObject BulletPrefab;

        // 声音 (射击、换弹)
        public AudioSource FirearmsShootingAudioSource;
        public AudioSource FirearmsReloadAudioSource;
        public FirearmAudioData FirearmAudioData;

        // 倍镜(通过拉近摄像机实现)
        public Camera EyeCamera;
        // 散射角度
        public float SpreadAngle;

        // 换枪音效延迟
        public float TakeOutWeaponAudioDelay;

        // 武器图标
        public Sprite WeaponIcon;

        // 开枪动画
        public Animator GunAnimator;
        protected AnimatorStateInfo GunStateInfo;

        // 换弹时不允许射击
        public bool isallowShoot = true;

        // 当前子弹
        protected int CurrentAmmo;
        protected int CurrentMaxAmmoCarried;
        
        // 记录上一次射击时间，控制射击间隔
        protected float lastFireTime;

        // 原始倍镜
        protected float OriginFOV;

        // 是否瞄准状态
        protected bool IsAiming;
        // 是否在按住射击
        protected bool IsHoldingTrigger;
        
        // 瞄准协程（更平滑）
        private IEnumerator doAimCoroutine;

        protected virtual void Awake()
        {
            CurrentAmmo = AmmoInMag;
            CurrentMaxAmmoCarried = MaxAmmoCarried;
            if (GunAnimator == null)
            {
                GunAnimator = GetComponent<Animator>();
            }

            if (EyeCamera != null)
            {
                OriginFOV = EyeCamera.fieldOfView;
            }
            doAimCoroutine = DoAim();
        }

        public void DoAttack()
        {
            Shooting();

        }

        // 不同的枪不一样，抽象射击、换弹函数
        protected abstract void Shooting();
        protected abstract void Reload();

        //protected abstract void Aim();
        public void Aiming(bool isAiming)
        {
            IsAiming = isAiming;
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
        }

        public void HoldTrigger()
        {
            DoAttack();
            IsHoldingTrigger = true;
        }

        public void ReleaseTrigger()
        {
            IsHoldingTrigger = false;
        }

        public void ReloadAmmo()
        {
            Reload();
        }

        // 通过射击间隔判断是否能射击
        public bool IsAllowShooting()
        {
            return (Time.time - lastFireTime) > (1 / FireRate);
        }

        // 子弹散射
        protected Vector3 CalculateSpreadOffset()
        {
            float tmp_SpreadPercent = SpreadAngle / EyeCamera.fieldOfView;

            return tmp_SpreadPercent * Random.insideUnitCircle;
        }

        // 判断换弹动画结束，补充子弹
        protected IEnumerator CheckReloadAmmoAnimationEnd()
        {
            while (true)
            {
                yield return null;
                // Reload Layer
                GunStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(GunAnimator.GetLayerIndex("Reload Layer"));

                // 获取换弹动画状态
                if (GunStateInfo.IsTag("ReloadAmmo"))
                {
                    if (GunStateInfo.normalizedTime >= 0.9f)
                    {
                        // 待补充的弹药数量
                        int tmp_NeedAmmoCount = AmmoInMag - CurrentAmmo;
                        // 补充后剩余弹药数量
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
                        isallowShoot = true;
                        yield break;
                    }
                }
            }
        }

        // 瞄准
        protected IEnumerator DoAim()
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
        }

        public int GetCurrentMaxAmmoCarried()
        {
            return CurrentMaxAmmoCarried;
        }

        public int GetCurrentAmmo()
        {
            return CurrentAmmo;
        }
    }
}