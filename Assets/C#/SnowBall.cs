using Assets.Player;
using UnityEngine;

namespace Assets
{
    public class SnowBall : MonoBehaviour {

        public GameObject snowShatter;
        public Transform shatterPosition;
        public Movement movement;
        public new CameraController camera;
        public float pushForce;
        public int playerNum;

        private float pushForceX;
        private float scaleX;

        // Use this for initialization
        void Start () {
            movement = FindObjectOfType<Movement>();
            camera = FindObjectOfType<CameraController>();
        }
	
        // Update is called once per frame
        void Update () {
            scaleX = movement.direction;
            transform.localScale = new Vector3(scaleX, 1f, 1f);
        }

        void OnTriggerEnter2D(Collider2D other) {
		
            if(other.tag == "Player" && other.GetComponent<Movement>().playerId == playerNum) {

            } else {
                if(other.transform.position.x >= transform.position.x) {
                    pushForceX = pushForce * 1f;
                } else {
                    pushForceX = pushForce * -1f;
                }
                if(other.tag == "Player" && other.GetComponent<Movement>().playerId != playerNum && other.GetComponent<Movement>().gottenRekt == false) {
                    other.GetComponent<Movement>().Hit();
                    other.GetComponent<Movement>().PushBack(pushForceX, pushForce);
                    camera.CameraShake(0.2f, 0.1f);
                }
                Destroy(gameObject);
                GameObject sShatter = Instantiate(snowShatter, shatterPosition.transform.position, Quaternion.identity) as GameObject; // UNUSED
            }
        }
    }
}
