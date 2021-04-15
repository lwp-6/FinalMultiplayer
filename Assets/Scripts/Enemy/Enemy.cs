using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Scripts.Weapon;

public abstract class Enemy : MonoBehaviour
{
    // 敌人基本参数
    public Transform MuzzlePoint;
    public AudioSource audioSource;
    public GameObject BulletPrefab;
    public int BulletSpeed;
    public float FireRate;
    public float WalkSpeed;
    public float RunSpeed;
    public GameLevelController gameLevelController;
    public int AmmoInMag;
    public float SpreadAngle;
    public Camera EyeCamera;

    // 玩家敌人距离
    protected float Distance;
    protected float lastFireTime;
    // 导航代理
    protected NavMeshAgent agent;
    // 玩家
    protected Transform playerTransform;
    protected AudioSource shootAudioSource;
    protected Animator EnemyAnimator;
    // 修正敌人看玩家视角
    protected Vector3 LookAt_fix;

    // 敌人受伤动画控制
    protected bool isDamage;
    
    // 敌人生命值管理
    protected HPManager hpManager;

    // 敌人感知范围
    // 视觉
    protected float SightRange;
    protected float SightAngle;
    // 听觉
    protected float SoundRange;
    public GameObject EnemyHead;

    //死亡过程
    protected bool isDied;

    // 状态机
    protected abstract void FSMController();

    //****** 敌人状态 *******//
    protected abstract void Died();
    //protected abstract void Reload();
    protected abstract void Attack();
    public abstract void Damage();
    protected abstract void Chase();
    protected abstract void Patrol();
    //***********************//

    // ***** 敌人感知 *******//
    protected abstract bool isPlayerbeSeen();
    protected abstract bool isPlayerbeHeard();
    protected abstract bool isEnemybeHit();
    //***********************//

    // 生成子弹
    protected abstract GameObject CreateBullet();

    // 子弹散射
    protected abstract Vector3 CalculateSpreadOffset();
}
