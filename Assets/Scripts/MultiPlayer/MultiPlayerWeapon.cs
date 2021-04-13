using Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayerWeapon : Firearms
{

    // 判断换弹动画是否结束的协程
    private IEnumerator reloadAmmoCheckerCoroutine;
    protected override void Shooting()
    {
        // 控制射击间隔，换弹时不能射击  // 没子弹时不能射击
        if (/*!IsAllowShooting() || */!isallowShoot || CurrentAmmo <= 0)
        {
            return;
        }
        
        --CurrentAmmo;
        //Debug.LogError(CurrentAmmo);

        // 枪震动 动画
        GunAnimator.Play("Fire", 0, 0);

        // 抛出子弹壳
        DropBulletsParticle.Play();

        // 枪口火光
        MuzzleParticle.Play();
        CasingParticle.Play();

        lastFireTime = Time.time;
    }

    protected override void Reload()
    {
        isallowShoot = false;

        // 设置动画层， reload层索引2，权重设置为1
        GunAnimator.SetLayerWeight(1, 1);
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
    }

}
