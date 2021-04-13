using UnityEngine;
using UnityEditor;

namespace Scripts.Weapon
{
    [CreateAssetMenu(menuName = "FPS/Firearms Audio Data")]
    public class FirearmAudioData : ScriptableObject
    {
        public AudioClip ShootingAudio;
        public AudioClip ReloadLeft;
        public AudioClip ReloadOutOf;
        public AudioClip AimIn;
        public AudioClip TakeOutWeapon;
    }
}