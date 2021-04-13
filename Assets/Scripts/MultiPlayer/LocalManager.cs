using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class LocalManager : MonoBehaviour
{
    public List<MonoBehaviour> LocalScripts;
    public Camera GunCamera;
    public Camera MainCamera;

    public List<Renderer> TPRenderers;
    public GameObject FPArms;

    public Canvas canvas;

    private PhotonView photonView;

    private void Start()
    {
        // 本地客户端
        photonView = GetComponent<PhotonView>();
        if (photonView.IsMine)
        {
            gameObject.AddComponent<AudioListener>();
            return;
        }

        // 远程客户端
        FPArms.SetActive(false);
        GunCamera.enabled = false;
        MainCamera.enabled = false;
        canvas.gameObject.SetActive(false);

        foreach (MonoBehaviour behaviour in LocalScripts)
        {
            behaviour.enabled = false;
        }

        foreach (Renderer tpRenderer in TPRenderers)
        {
            tpRenderer.shadowCastingMode = ShadowCastingMode.On;
        }
    }
}
