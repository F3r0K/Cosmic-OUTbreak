using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSpawner : MonoBehaviour
{
    [SerializeField]
    Transform m_Player;

    [Space]
    [SerializeField]
    GameObject m_VisionPrefab;
    [SerializeField]
    GameObject m_WallPrefab;
    [SerializeField]
    GameObject m_EnemyPrefab;

    public void SpawnVision() {
        Instantiate(m_VisionPrefab, m_Player.position, Quaternion.identity);
    }

    public void SpawnWall() {
        Instantiate(m_WallPrefab, m_Player.position, Quaternion.identity);
    }

    public void SpawnEnemy() {
        Instantiate(m_EnemyPrefab, m_Player.position, Quaternion.identity);
    }
}
