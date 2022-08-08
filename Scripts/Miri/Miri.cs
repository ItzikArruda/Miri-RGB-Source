using Experiments.MiriRGB.Managers;
using Experiments.Global.Managers;
using Experiments.Global.Audio;
using UnityEngine;

namespace Experiments.MiriRGB.Gameplay
{
    [RequireComponent(typeof(Rigidbody))]
    public class Miri : MonoBehaviour
    {
        public enum ColorStates { MIRI, R, G, B }
        public ColorStates ColorState;
        public float MovementSpeed;
        public float MovementRange;
        public float JumpHeight;
        float MovementDir;
        public static Color[] Colors = { Color.white, Color.red, Color.green, Color.blue };
        MeshRenderer meshRenderer;
        [HideInInspector]
        public bool Playing;
        Rigidbody Body;
        Vector3 BodRot;

        // Start is called before the first frame update
        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            Body = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            meshRenderer.sharedMaterial.color = Colors[(int)ColorState];

            Playing = (int)ColorState > 0;
            Body.isKinematic = !Playing;

            switch (ColorState)
            {
                case ColorStates.MIRI:
                {
                    if(Input.anyKeyDown)
                    {
                        Body.isKinematic = false;
                        MovementDir = (Random.value > 0.5f) ? 1f : -1f;
                        MiriManager.Instance.progress.GamesPlayed++;
                        AudioManager.Instance.InteractWithSFX("First Jump", SoundEffectBehaviour.Play);
                        AudioManager.Instance.InteractWithSFX("MIRI", SoundEffectBehaviour.Play);
                        Jump();
                    }

                    break;
                }
                case ColorStates.R:
                {
                    if(Input.GetKeyDown("r"))
                    {
                        Jump();
                        AudioManager.Instance.InteractWithSFX("R", SoundEffectBehaviour.Play);
                    }
                    else if(Input.GetKeyDown("g") ^ Input.GetKeyDown("b"))
                    {
                        MiriMeta();
                    }

                    break;
                }
                case ColorStates.G:
                {
                    if(Input.GetKeyDown("g"))
                    {
                        Jump();
                        AudioManager.Instance.InteractWithSFX("G", SoundEffectBehaviour.Play);
                    }
                    else if(Input.GetKeyDown("r") ^ Input.GetKeyDown("b"))
                    {
                        MiriMeta();
                    }

                    break;
                }
                case ColorStates.B:
                {
                    if(Input.GetKeyDown("b"))
                    {
                        Jump();
                        AudioManager.Instance.InteractWithSFX("B", SoundEffectBehaviour.Play);
                    }
                    else if(Input.GetKeyDown("g") ^ Input.GetKeyDown("r"))
                    {
                        MiriMeta();
                    }

                    break;
                }
            }

            if(!MiriManager.Instance.Cam.WithinCamRange(transform))
            {
                MiriMeta();
            }
        }
        void Jump()
        {
            Body.velocity = Vector3.zero;
            BodRot = Random.insideUnitSphere * 360f;
            Body.AddForce(Vector3.up * (JumpHeight * 100f));
            ColorState = (ColorStates)Random.Range(1, 4);
        }

        void FixedUpdate()
        {
            if(Mathf.Abs(Body.position.x) > MovementRange) { MovementDir *= -1f; }
            Body.velocity = new Vector3(MovementSpeed * MovementDir, Body.velocity.y, 0f);
            Body.angularVelocity = (Playing) ? BodRot : Vector3.zero;
        }

        void OnCollisionEnter(Collision other)
        {
            if(other.collider.tag == "Obstacles")
            {
                MiriMeta();
            }
        }
        void MiriMeta()
        {
            FXManager.Instance.SpawnFX("Miri Die", transform.position);
            AudioManager.Instance.InteractWithSFX("Die", SoundEffectBehaviour.Play);
            Destroy(gameObject);
            MiriManager.Instance.Die();
        }
    }
}