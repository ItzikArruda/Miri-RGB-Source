using System.Collections;
using Experiments.Global.Managers;
using UnityEngine;

namespace Experiments.MiriRGB.Managers
{
    public class SpawnManager : Manager<SpawnManager>
    {
        [Space]
        public GameObject[] ObstaclePrefabs;
        public Transform ObstacleContainer;
        public int RenderDistance;
        public float StartSpawnGaps;
        public float MinSpawnGaps;
        public int MaxSpawnAmount;
        public float XRange;
        float SpawnGaps;
        float SpawnGapRange;
        float SpawnGapIntervals;
        float YPos;

        // Start is called before the first frame update
        void Start()
        {
            Init(this);

            SpawnGaps = StartSpawnGaps;
            SpawnGapRange = StartSpawnGaps - MinSpawnGaps;
            SpawnGapIntervals = SpawnGapRange / MaxSpawnAmount;

            for (int S = 0; S < RenderDistance; S++)
            {
                SpawnObstacle();
            }
        }

        public void SpawnObstacle()
        {
            float XPos = Random.Range(-XRange, XRange);
            YPos += SpawnGaps;
            int OBJIndex = Random.Range(0, ObstaclePrefabs.Length);
            Instantiate(ObstaclePrefabs[OBJIndex], new Vector3(XPos, YPos, 0f), Quaternion.identity, ObstacleContainer);

            if(SpawnGaps > MinSpawnGaps) { SpawnGaps -= SpawnGapIntervals; }
        }
    }
}