using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UnityTemplateProijects.UI.Models
{
    [CreateAssetMenu(menuName = "FPS/Map Models")]
    public class MapModelsScriptableObject : ScriptableObject
    {
        public List<MapModel> MapModels;
    }
}