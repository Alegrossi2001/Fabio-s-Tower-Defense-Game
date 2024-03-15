/// ---------------------------------------------
/// Rhythm Timeline
/// Copyright (c) Dyplsoom. All Rights Reserved.
/// https://www.dypsloom.com
/// ---------------------------------------------

namespace Dypsloom.RhythmTimeline.Scoring
{
    using UnityEngine;

    public class PopUp : MonoBehaviour
    {
        public void Pop(Vector3 position, GameObject Spriteprefab)
        {
            Instantiate(Spriteprefab, position, Quaternion.identity);

        }

    }
}
