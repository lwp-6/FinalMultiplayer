using Photon.Pun;
using Scripts.Weapon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 武器系统
public class WeaponManager : MonoBehaviour
{
    public Firearms MainWeapon;
    public Firearms SecondaryWeapon;
    public Text AmmoCountTextLabel;
    public Image CurrentWeaponImage;
    public FPCharacterControllerMovement FPCharacterControllerMovement;

    private Firearms carriedWeapon;
    private PhotonView photonView;


    public List<WeaponInfo> WeaponInfos;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            MainWeapon = WeaponInfos[0].FP_Weapon;
        }
        else
        {
            MainWeapon = WeaponInfos[0].TP_Weapon;
        }

        carriedWeapon = MainWeapon;
        FPCharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);
    }

    public void UpdateAmmoInfo(int ammo, int remaningAmmo)
    {
        if (!AmmoCountTextLabel)
        {
            return;
        }
        AmmoCountTextLabel.text = ammo.ToString() + " / " + remaningAmmo.ToString();
    }

    private void Update()
    {        
        // 暂停
        if (GlobleVar.isPause || !photonView.IsMine)
        {
            return;
        }


        SwapWeapon();

        if (!carriedWeapon) return;

        if (Input.GetMouseButton(0) && carriedWeapon.IsAllowShooting())
        {
            if (carriedWeapon.GetCurrentAmmo() <= 0 && carriedWeapon.isallowShoot)
            {
                if (GlobleVar.isJoinRoom)
                {
                    photonView.RPC("RPCReloadAmmo", RpcTarget.All);
                }
                else
                {
                    carriedWeapon.ReloadAmmo();
                }
            }
            else
            {
                //carriedWeapon.HoldTrigger();
                //photonView.RPC("RPCHoldTrigger", RpcTarget.All);
                if (GlobleVar.isJoinRoom)
                {
                    photonView.RPC("RPCHoldTrigger", RpcTarget.All);
                }
                else
                {
                    carriedWeapon.HoldTrigger();
                }
            }
            
        }

        if (Input.GetMouseButtonUp(0))
        {
            //carriedWeapon.ReleaseTrigger(); 
            //photonView.RPC("RPCReleaseTrigger", RpcTarget.All);
            if (GlobleVar.isJoinRoom)
            {
                photonView.RPC("RPCReleaseTrigger", RpcTarget.All);
            }
            else
            {
                carriedWeapon.ReleaseTrigger();
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && 
            carriedWeapon.GetCurrentMaxAmmoCarried() > 0 && 
            carriedWeapon.GetCurrentAmmo() < carriedWeapon.AmmoInMag && 
            carriedWeapon.isallowShoot){

            //carriedWeapon.ReloadAmmo();
            //photonView.RPC("RPCReloadAmmo", RpcTarget.All);
            if (GlobleVar.isJoinRoom)
            {
                photonView.RPC("RPCReloadAmmo", RpcTarget.All);
            }
            else
            {
                carriedWeapon.ReloadAmmo();
            }
        }

        // 瞄准
        if (Input.GetMouseButtonDown(1))
        {
            // 瞄准的AudioSource和换弹的共用
            carriedWeapon.FirearmsReloadAudioSource.clip = carriedWeapon.FirearmAudioData.AimIn;
            carriedWeapon.FirearmsReloadAudioSource.Play();
            carriedWeapon.Aiming(true);
        }
        // 退出瞄准
        if (Input.GetMouseButtonUp(1))
        {
            carriedWeapon.Aiming(false);
        }

        UpdateAmmoInfo(carriedWeapon.GetCurrentAmmo(), carriedWeapon.GetCurrentMaxAmmoCarried());
    }

    private void SwapWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))      // 主武器
        {
            SwapWeaponHandler(MainWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) // 副武器
        {
            SwapWeaponHandler(SecondaryWeapon);
        }
    }

    private void SwapWeaponHandler(Firearms targetWeapon)
    {
        // 隐藏原武器
        carriedWeapon.gameObject.SetActive(false);
        carriedWeapon = targetWeapon;

        // 显示切换后武器
        carriedWeapon.gameObject.SetActive(true);

        // 拿出武器音效，共用换弹的AudioSource
        carriedWeapon.FirearmsReloadAudioSource.clip = carriedWeapon.FirearmAudioData.TakeOutWeapon;
        carriedWeapon.FirearmsReloadAudioSource.PlayDelayed(carriedWeapon.TakeOutWeaponAudioDelay);

        // 显示武器图标
        CurrentWeaponImage.sprite = carriedWeapon.WeaponIcon;

        FPCharacterControllerMovement.SetupAnimator(carriedWeapon.GunAnimator);
    }

    [PunRPC]
    private void RPCHoldTrigger()
    {
        carriedWeapon.HoldTrigger();
    }

    [PunRPC]
    private void RPCReleaseTrigger()
    {
        carriedWeapon.ReleaseTrigger();
    }

    [PunRPC]
    public void RPCReloadAmmo()
    {
        carriedWeapon.ReloadAmmo();
    }
}

[System.Serializable]
public class WeaponInfo
{
    public int WeaponId;
    public string WeaponName;
    public Firearms FP_Weapon;
    public Firearms TP_Weapon;
}