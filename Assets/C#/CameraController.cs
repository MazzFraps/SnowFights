using UnityEngine;


    public class CameraController : MonoBehaviour
    {
        private Vector3 basePosition;

        // Use this for initialization
        void Start () {
            basePosition = transform.position;
        }

        // Update is called once per frame
        void Update () {
	
        }

        ///// Shaking and stuff
        private float shakeAmaunt;

        public void CameraShake(float shakeAmt, float duration) {
            shakeAmaunt = shakeAmt;
            InvokeRepeating("StartShaking", 0, 0.01f);
            Invoke("StopShaking", duration);

        }

        void StartShaking() {
            float quakeAmt = Random.value*shakeAmaunt*2 - shakeAmaunt;
            Vector3 pp = transform.position;
            pp.y += quakeAmt;
            pp.x += quakeAmt; // can also add to x and/or z
            transform.position = pp;
        }

        void StopShaking() {
            CancelInvoke("StartShaking");
            transform.position = basePosition;
        }
    }
