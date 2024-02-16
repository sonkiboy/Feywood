using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(fileName = "List of Spirte Assets", menuName = "List of Sprite Assets", order = 0)]

    public class ListofTmpAssets : ScriptableObject
    {
        public List<TMP_SpriteAsset> SpriteAssets;
    }
}
