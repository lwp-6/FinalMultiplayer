using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPMouseLook : MonoBehaviour
{
    public float MouseSensitivity;
    public Vector2 MaxMinAngle;
    public Transform characterTransform;
    
    // 后坐力曲线
    public AnimationCurve RecoilCurve;
    // 初始化后坐力
    public Vector2 RecoilRange;
    public float RecoilFadeOutTime;
    
    private float currentRecoilTime;
    private Vector2 currentRecoil;


    private Transform cameraTransform;
    
    private Vector3 cameraRotation;

    public CameraSpring cameraSpring;

    private void Start()
    {
        cameraTransform = transform;
        currentRecoil = RecoilRange;
        cameraSpring = GetComponentInChildren<CameraSpring>();
    }

    // Update is called once per frame
    void Update()
    {
        // 暂停
        if (GlobleVar.isPause)
        {
            return;
        }

        // 获取鼠标输入
        var tmp_MouseX = Input.GetAxis("Mouse X");
        var tmp_MouseY = Input.GetAxis("Mouse Y");

        // 改变摄像机角度
        cameraRotation.y += tmp_MouseX * MouseSensitivity;
        cameraRotation.x -= tmp_MouseY * MouseSensitivity;

       

        // 后坐力 
        CalculateRecoilOffset();
        //Debug.Log(currentRecoil);
        cameraRotation.y += currentRecoil.y;
        cameraRotation.x -= currentRecoil.x;
        //Debug.Log(currentRecoil);

        cameraRotation.x =  Mathf.Clamp(cameraRotation.x, MaxMinAngle.x, MaxMinAngle.y);

        // 转动摄像机和角色
        cameraTransform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
        characterTransform.rotation = Quaternion.Euler(0, cameraRotation.y, 0);
    }

    // 计算后坐力偏移
    private void CalculateRecoilOffset()
    {
        currentRecoilTime += Time.deltaTime;
        float tmp_RecoilFration = currentRecoilTime / RecoilFadeOutTime;
        float tmp_RecoilValue = RecoilCurve.Evaluate(tmp_RecoilFration);
        currentRecoil = Vector2.Lerp(Vector2.zero, RecoilRange, tmp_RecoilValue);
    }

    // 射击时调用函数，实现后坐力
    public void FringForTest()
    {
        //currentRecoil += RecoilRange;
        // 重置时间，根据曲线，不重置时间时后坐力为0
        currentRecoilTime = 0;

        cameraSpring.StartCameraSpring();
    }
}
