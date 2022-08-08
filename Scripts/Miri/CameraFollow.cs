using System.Collections;
using Experiments.MiriRGB.Gameplay;
using Experiments.MiriRGB.Managers;
using UnityEngine;

namespace Experiments.MiriRGB.Visuals
{
    [RequireComponent(typeof(Camera))]
    public class CameraFollow : MonoBehaviour
    {
        public Miri Miri;
        public Vector3 Offset;
        public float SmoothTime;
        public float PosOffset;
        public float CamRange;
        Vector3 Velocity;
        float MiriY;
        Camera Cam;
        [HideInInspector]
        public float FlashRate;

        void Start()
        {
            Cam = GetComponent<Camera>();
        }

        void Update()
        {
            if(Miri != null) { MiriY = Miri.transform.position.y; }

            if(Miri == null)
            {
                FlashRate -= 1f * Time.deltaTime;
                FlashRate = Mathf.Clamp01(FlashRate);
                Cam.backgroundColor = Color.Lerp(Color.black, Color.white, FlashRate);
            }
            else if(!Miri.Playing)
            {
                Cam.backgroundColor = Color.black;
            }
            else
            {
                Cam.backgroundColor = Miri.Colors[(int)MiriManager.Instance.miri.ColorState];
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if(MiriY > transform.position.y - PosOffset && MiriPlaying())
            {
                Vector3 TargetPos = (Vector3.up * (MiriY + PosOffset)) + Offset;
                Vector3 SmoothedPos = Vector3.SmoothDamp(transform.position, TargetPos, ref Velocity, SmoothTime);
                transform.position = SmoothedPos;
            }
        }
        bool MiriPlaying()
        {
            if(Miri == null)
            {
                return true;
            }

            return Miri.Playing;
        }

        public bool WithinCamRange(Transform OBJ)
        {
            return OBJ.position.y >= transform.position.y - CamRange;
        }
    }
}