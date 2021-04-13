using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Impact Audio Data")]
    public class ImpactAudioData : ScriptableObject
    {
        public List<AudioClip> ImpactAudios = new List<AudioClip>();
    }
    
}