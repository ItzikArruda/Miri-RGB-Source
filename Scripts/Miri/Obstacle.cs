using System.Collections;
using Experiments.MiriRGB.Managers;
using UnityEngine;

namespace Experiments.MiriRGB.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Obstacle : MonoBehaviour
    {
        Rigidbody Body;
        public enum ObstacleTypes { Regular, Moving }
        public ObstacleTypes ObstacleType;
        public float MovementSpeed;
        float MovementDir;
        Vector3 RotVel;

        // Start is called before the first frame update
        void Start()
        {
            Body = GetComponent<Rigidbody>();
            Body.constraints = RigidbodyConstraints.FreezePositionY;

            MovementDir = (Random.value > 0.5f) ? 1f : -1f;
            RotVel = Random.insideUnitSphere * 360f;
        }

        // Update is called once per frame
        void Update()
        {
            if(!MiriManager.Instance.Cam.WithinCamRange(transform))
            {
                Destroy(gameObject);
                SpawnManager.Instance.SpawnObstacle();
            }
        }

        void FixedUpdate()
        {
            if(Mathf.Abs(Body.position.x) >= SpawnManager.Instance.XRange) { MovementDir *= -1f; }
            Body.velocity = (ObstacleType == ObstacleTypes.Regular) ? Vector3.zero : Vector3.right * (MovementSpeed * MovementDir);
            Body.angularVelocity = (ObstacleType == ObstacleTypes.Regular) ? RotVel : Vector3.zero;
        }
    }
}