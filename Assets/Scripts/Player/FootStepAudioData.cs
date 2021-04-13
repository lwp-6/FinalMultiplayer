using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/FootStep Audio Data")]
public class FootStepAudioData : ScriptableObject
{
    public List<FootStepAudio> FootStepAudios = new List<FootStepAudio>();
}

[System.Serializable]
public class FootStepAudio
{
    public List<AudioClip> AudioClips = new List<AudioClip>();
    public string Tag;
    public float Delay;

}
