using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dypsloom.RhythmTimeline.SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] protected int m_SceneBuildIndex;

        [ContextMenu("LoadScene")]
        public void LoadScene()
        {
            LoadScene(m_SceneBuildIndex);
        }

        private void LoadScene(int buildIndex)
        {
            SceneManager.LoadScene(buildIndex);
        }
    }
}
