using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class AimingController : MonoBehaviour, IPunObservable
{
    public Transform Arms;
    public Transform AimTarget;
    public float AimTargetDistance = 5f;

    private Vector3 LocalPosition;
    private Quaternion LocalRotation;

    private PhotonView photonView;

    private void Start()
    {
        photonView = GetComponent<PhotonView>();
        LocalPosition = AimTarget.position;
    }

    // Update is called once per frame
    void Update()
    {
        // 本地客户端
        if (photonView.IsMine)
        {
            LocalRotation = Arms.localRotation;
            LocalPosition = LocalRotation * Vector3.forward * AimTargetDistance;
        }

        AimTarget.localPosition = Vector3.Lerp(AimTarget.localPosition, LocalPosition, Time.deltaTime * 20);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 发送数据
            stream.SendNext(LocalPosition);
        }
        else
        {
            // 接受数据
            LocalPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
