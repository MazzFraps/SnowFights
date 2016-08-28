using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Player
{
    public class Movement : NetworkBehaviour
    {
        // COMPONENTS
        private Rigidbody2D rb2D;
        private Animator myAnim;
        [HideInInspector]
        public GameObject progressBar;
        private ProgressBar pBar;

        public int playerId;

        [HideInInspector]
        public GameObject snowBallHand;
        [HideInInspector]	
        public GameObject snowBall;


        // GROUND CHECK
        [HideInInspector]
        public Transform groundCheck;
        [HideInInspector]
        public LayerMask whatIsGround;
        private float groundCheckRadius = 1.9f;


        // MOVEMENT / CONTROL
        public float speed;
        public float jumpPower;

        // Controls
        public string horizontalAxis;
        public string verticalAxis;
        public string jumpButton;
        public string fire1;

        // STATES
        [SyncVar]
        public bool isDucking;
        [SyncVar]
        public bool isGrounded;
        [SyncVar]
        public bool isJumping = false;
        [SyncVar]
        private int actionNumber = 0;
        [SyncVar]
        private bool hasSnowBall = false;
        [HideInInspector]
        [SyncVar]
        public bool direction;
        [SyncVar]
        public bool gottenRekt = false;


        // THROW SNOWBALL
        private int prevNum;
        private bool ended = true; // UNUSED
        private float duckDelay = 0f;

        public float projectileSpeed;
        [HideInInspector]
        public Transform snowBallLaunchPoint;
        public GameObject trail;

        //private float xSpeed;
        //private float ySpeed;

        //private float verticalDirection;



        // Use this for initialization
        void Start () {
            rb2D = GetComponent<Rigidbody2D>();
            myAnim = GetComponent<Animator>();
            pBar = GetComponent<ProgressBar>();
            progressBar.SetActive(false);
            prevNum = actionNumber;

            // Key mapping based on player ID
            /*
            horizontalAxis = "Horizontal_P" + playerId;
            verticalAxis = "Vertical_P" + playerId;
            jumpButton = "Jump_P" + playerId;
            fire1 = "Fire1_P" + playerId;
            */

            // Universal key mapping
            horizontalAxis = "Horizontal"; // A + D
            verticalAxis = "Vertical"; // W + S
            jumpButton = "Jump"; // Space
            fire1 = "Fire1"; // Left Shift
        }

        public override void OnStartLocalPlayer()
        {
            SpriteRenderer[] components = GetComponentsInChildren<SpriteRenderer>();

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i].sprite.name == "characterBody")
                {
                    components[i].color = Color.green;
                }
            }
        }

        // Update is called once per frame
        void Update ()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            // CHECKS AND STUFF
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
		
            // MOVEMENT
            PlayerMovement();
            
            // JUMP HANDLER
            HandleJumping();

            // DUCK HANDLER / MAKE SNOWBALL
            HandleDucking_MakeSnowball();

            // THROW SNOWBALL
            ThrowSnowball();
            
            // STATE HANDLER
            HandleStates();
        }

        void PlayerMovement()
        {
            if (Input.GetAxisRaw(horizontalAxis) > 0 && !isDucking && !gottenRekt)
            {
                rb2D.velocity = new Vector3(speed, rb2D.velocity.y, 0f);
                transform.localScale = new Vector3(1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(1f, 1f, 1f);

                direction = true;

                // Inform server, that direction has changed
                CmdDirection(direction);

            }
            else if (Input.GetAxisRaw(horizontalAxis) < 0 && !isDucking && !gottenRekt)
            {
                rb2D.velocity = new Vector3(-speed, rb2D.velocity.y, 0f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(-1f, 1f, 1f);

                direction = false;

                // Inform server, that direction has changed
                CmdDirection(direction);

            }
            else if (!gottenRekt)
            {
                rb2D.velocity = new Vector3(0f, rb2D.velocity.y, 0f);
            }
        }

        void ThrowSnowball()
        {
            float xSpeed = 0f;
            float ySpeed = 0f;

            if (Input.GetButtonDown(fire1) && hasSnowBall && !gottenRekt)
            {
                prevNum = actionNumber;
                actionNumber = 2;

                trail.SetActive(true); // Hand trail activate
                StartCoroutine(TrailTimer(0.15f)); // Hand trail deactivate

                if (Input.GetAxisRaw(horizontalAxis) != 0)
                {
                    if (Input.GetAxisRaw(verticalAxis) != 0)
                    {
                        xSpeed = projectileSpeed * 0.7071f;
                        ySpeed = projectileSpeed * 0.7071f;
                    }
                    else
                    {
                        xSpeed = projectileSpeed;
                        ySpeed = 0f;
                    }
                }

                if (Input.GetAxisRaw(horizontalAxis) == 0)
                {
                    if (Input.GetAxisRaw(verticalAxis) != 0)
                    {
                        xSpeed = 0;
                        ySpeed = projectileSpeed;
                    }
                    else
                    {
                        xSpeed = projectileSpeed;
                        ySpeed = 0;
                    }
                }

                //GameObject sBall = Instantiate(snowBall, snowBallLaunchPoint.transform.position, Quaternion.identity) as GameObject;
                //sBall.GetComponent<SnowBall>().playerNum = playerId;

                float verticalDirection;

                if (Input.GetAxisRaw(verticalAxis) < 0)
                {
                    verticalDirection = -1f;
                }
                else if (Input.GetAxisRaw(verticalAxis) > 0)
                {
                    verticalDirection = 1f;
                }
                else
                {
                    verticalDirection = 0f;
                }

                //sBall.GetComponent<Rigidbody2D>().velocity = new Vector3(xSpeed * transform.localScale.x, ySpeed * verticalDirection, 0);
                CmdFire(xSpeed, ySpeed, verticalDirection);

                hasSnowBall = false;
                snowBallHand.SetActive(false);

            }
            if (Input.GetButtonUp(fire1))
            {
                actionNumber = prevNum;
            }
        }

        void HandleStates()
        {
            myAnim.SetFloat("Speed", Mathf.Abs(rb2D.velocity.x));
            myAnim.SetFloat("VSpeed", rb2D.velocity.y);
            myAnim.SetBool("Grounded", isGrounded);
            myAnim.SetBool("Duck", isDucking);
            myAnim.SetInteger("Action", actionNumber);

            if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                myAnim.SetBool("GottenRekt", false);
            }
        }

        void HandleDucking_MakeSnowball()
        {
            if (Input.GetAxisRaw(verticalAxis) < 0 && isGrounded && duckDelay <= 0f && !gottenRekt /* && !hasSnowBall */ )
            {
                isDucking = true;

                if (!hasSnowBall)
                {
                    actionNumber = 1;
                    progressBar.SetActive(true);
                    pBar.currentPoints += 50f * Time.deltaTime;

                    if (pBar.currentPoints >= pBar.maxPoints)
                    {
                        hasSnowBall = true;
                        snowBallHand.SetActive(true);
                        isDucking = false;
                        progressBar.SetActive(false);
                        pBar.currentPoints = 0f;
                        duckDelay = 0.4f;
                    }
                }
                else
                {
                    actionNumber = 0;
                }

            }
            else if (Input.GetAxisRaw(verticalAxis) >= 0 || !isGrounded)
            {
                isDucking = false;
                progressBar.SetActive(false);
                pBar.currentPoints = 0f;
            }

            if (duckDelay > 0f)
            {
                duckDelay -= Time.deltaTime;
            }
        }

        void HandleJumping()
        {
            if (Input.GetButtonDown(jumpButton) && !gottenRekt && isGrounded)
            {
                rb2D.velocity = new Vector3(rb2D.velocity.x, jumpPower, 0f);
            }
        }

        [Command]
        public void CmdDirection(bool dir)
        {
            if (dir)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(1f, 1f, 1f);

                // Inform other players, that player direction has changed
                RpcDirection(dir);
            }
            else
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(-1f, 1f, 1f);

                // Inform other players, that player direction has changed
                RpcDirection(dir);
            }
        }

        [ClientRpc]
        public void RpcDirection(bool dir)
        {
            if (dir)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }

        [Command]
        public void CmdFire(float xSpeed, float ySpeed, float verticalDirection)
        {
            GameObject sBall = Instantiate(snowBall, snowBallLaunchPoint.transform.position, Quaternion.identity) as GameObject;
            sBall.GetComponent<SnowBall>().playerNum = playerId;

            sBall.GetComponent<Rigidbody2D>().velocity = new Vector3(xSpeed * transform.localScale.x, ySpeed * verticalDirection, 0);

            NetworkServer.Spawn(sBall);
            snowBallHand.SetActive(false);
        }

        public void Hit() {
            myAnim.SetBool("GottenRekt", true);
            gottenRekt = true;
            gameObject.layer = 11;
            StartCoroutine(RespawnTimer());
        }

        public void PushBack(float forceX, float forceY) {
            rb2D.velocity = new Vector3(rb2D.velocity.x + forceX, forceY, 0f);
            rb2D.drag = 3f;
        }

        public void Respawn() {
            gottenRekt = false;
            gameObject.layer = 9;
            rb2D.drag = 0f;
            myAnim.SetBool("Respawn", true);
        }

        IEnumerator RespawnTimer() {
            yield return new WaitForSeconds(2);
            Respawn();
        }

        IEnumerator TrailTimer(float time) {
            yield return new WaitForSeconds(time);
            trail.SetActive(false);
        }
    }
}

    
