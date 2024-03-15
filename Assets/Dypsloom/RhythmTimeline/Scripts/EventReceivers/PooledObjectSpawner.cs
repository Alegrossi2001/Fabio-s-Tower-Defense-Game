namespace Dypsloom.RhythmTimeline.Effects
{
    using Dypsloom.Shared.Utility;
    using UnityEngine;

    public class PooledObjectSpawner : MonoBehaviour
    {
        [Tooltip("The prefab to spawn.")]
        [SerializeField] protected GameObject m_Prefab;
        [Tooltip("The time before the spawned object should be destroyed. Don't destroy if < 0.")]
        [SerializeField] protected float m_ScheduledDestroy = -1;
        [Tooltip("The parent of the spawned object.")]
        [SerializeField] protected Transform m_Parent;
        [Tooltip("The spawn position, in case it is different from the parent.")]
        [SerializeField] protected Transform m_SpawnPosition;

        public void Spawn()
        {
            GameObject spawnedGameObject;
            if (m_SpawnPosition != null) {
                spawnedGameObject = PoolManager.Instantiate(m_Prefab, m_SpawnPosition.position, m_SpawnPosition.rotation, m_Parent);
            } else {
                spawnedGameObject = PoolManager.Instantiate(m_Prefab, m_Parent);
            }

            if (m_ScheduledDestroy > 0) {
                SchedulerManager.Schedule(() =>
                {
                    PoolManager.Destroy(spawnedGameObject);
                },m_ScheduledDestroy);
            }
        }
    }
}