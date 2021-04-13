using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 角色移动声音
public class PlayerFootStepListener : MonoBehaviour
{
    public FootStepAudioData FootStepAudioData;
    public AudioSource FootStepAudioSource;

    private CharacterController characterController;
    //private Transform footStepTransform;
    //private float velocity;
    private float nextPlayTime;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //footStepTransform = transform;
        nextPlayTime = FootStepAudioData.FootStepAudios[0].Delay;
}

    private void FixedUpdate()
    {
        // 暂停
        if (GlobleVar.isPause)
        {
            return;
        }

        //var tmp_Velocity = characterController.velocity;
        //velocity = new Vector3(tmp_Velocity.x, 0, tmp_Velocity.z).magnitude;
        // 在地面上才播放脚步声
        if (characterController.isGrounded && 
            (Input.GetKey(KeyCode.W) || 
            Input.GetKey(KeyCode.A) || 
            Input.GetKey(KeyCode.S) || 
            Input.GetKey(KeyCode.D)))
        {
            // 记录间隔
            nextPlayTime += Time.deltaTime;
            AudioClip tmp_FootStepAudioClip;
            tmp_FootStepAudioClip = FootStepAudioData.FootStepAudios[0].AudioClips[0];
            if (nextPlayTime >= FootStepAudioData.FootStepAudios[0].Delay)
            {
                FootStepAudioSource.clip = tmp_FootStepAudioClip;
                FootStepAudioSource.Play();
                nextPlayTime = 0;
            }

        }
        else
        {
            FootStepAudioSource.Stop();
            // 停下来后初始化为Delay而不是0，保证再次移动时马上播放脚步声，而不是等一个delay再播放
            nextPlayTime = FootStepAudioData.FootStepAudios[0].Delay;
        }
    }
}
