﻿using System.Collections;
using UnityEngine;


    public class Movement : MonoBehaviour
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

        public int whiteSnowPickup;
        public int yellowSnowPickup;



        // STATES
        public bool isDucking = false;
        public bool isGrounded;
        public bool isJumping = false;
        private int actionNumber = 0;

        private bool hasSnowBall = false;
        public bool yellowSnow = false;
        [HideInInspector]
        public float direction = -1f;
        public bool gottenRekt = false;


        // THROW SNOWBALL

        private int prevNum;
        private bool ended = true; // UNUSED
        private float duckDelay = 0f;
        public float projectileSpeed;
        [HideInInspector]
        public Transform snowBallLaunchPoint;
        public GameObject trail;

        private float xSpeed;
        private float ySpeed;

        private float verticalDirection;



        // Use this for initialization
        void Start () {
            rb2D = GetComponent<Rigidbody2D>();
            myAnim = GetComponent<Animator>();
            pBar = GetComponent<ProgressBar>();
            progressBar.SetActive(false);
            prevNum = actionNumber;

            horizontalAxis = "Horizontal_P" + playerId;
            verticalAxis = "Vertical_P" + playerId;
            jumpButton = "Jump_P" + playerId;
            fire1 = "Fire1_P" + playerId;

        }
	
        // Update is called once per frame
        void Update () {

            // CHECKS AND STUFF
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
		


            // MOVEMENT

            if(Input.GetAxisRaw(horizontalAxis) > 0 && !isDucking && !gottenRekt) {
                rb2D.velocity = new Vector3(speed, rb2D.velocity.y, 0f);
                transform.localScale = new Vector3(1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(1f, 1f, 1f);
                direction = 1f;
                // isMovingLeft = false;
            } else if(Input.GetAxisRaw(horizontalAxis) < 0 && !isDucking && !gottenRekt) {
                rb2D.velocity = new Vector3(-speed, rb2D.velocity.y, 0f);
                transform.localScale = new Vector3(-1f, 1f, 1f);
                progressBar.transform.localScale = new Vector3(-1f, 1f, 1f);
                direction = -1f;
                // isMovingLeft = true;
            }	else if(!gottenRekt) {
                rb2D.velocity = new Vector3(0f, rb2D.velocity.y, 0f);
            }



            // JUMP HANDLER

            if(Input.GetButtonDown(jumpButton) && !gottenRekt && isGrounded) {
                rb2D.velocity = new Vector3(rb2D.velocity.x, jumpPower, 0f);
            }




            // DUCK HANDLER / MAKE SNOWBALL

            if(Input.GetAxisRaw(verticalAxis) < 0 && isGrounded && duckDelay <= 0f && !gottenRekt /* && !hasSnowBall */ ) {
                isDucking = true;
                if(!hasSnowBall) {
                if (yellowSnow)
                {
                   // actionNumber = 1;
                    progressBar.SetActive(true);
                    pBar.currentPoints += yellowSnowPickup * Time.deltaTime;
                }
                else {
                   // actionNumber = 1;
                    progressBar.SetActive(true);
                    pBar.currentPoints += whiteSnowPickup * Time.deltaTime;
                }
                    if(pBar.currentPoints >= pBar.maxPoints) {
                    if (yellowSnow)
                    {

                    }
                    else {
                        hasSnowBall = true;
                        snowBallHand.SetActive(true);
                        isDucking = false;
                        progressBar.SetActive(false);
                        pBar.currentPoints = 0f;
                        duckDelay = 0.4f;
                    }
                    }
                } else {
                    actionNumber = 0;
                }
		
            } else if(Input.GetAxisRaw(verticalAxis) >= 0 || !isGrounded) {
                isDucking = false;
                progressBar.SetActive(false);
                pBar.currentPoints = 0f;
            }

            if(duckDelay > 0f) {
                duckDelay -= Time.deltaTime;
            }



            // THROW SNOWBALL


            if(Input.GetButtonDown(fire1) && hasSnowBall && !gottenRekt) {
                prevNum = actionNumber;
                actionNumber = 2;

                trail.SetActive(true); // Hand trail activate
                StartCoroutine(TrailTimer(0.15f)); // Hand trail deactivate

                if(Input.GetAxisRaw(horizontalAxis) != 0) {
                    if(Input.GetAxisRaw(verticalAxis) != 0) {
                        xSpeed = projectileSpeed * 0.7071f;
                        ySpeed = projectileSpeed * 0.7071f;
                    } else {
                        xSpeed = projectileSpeed;
                        ySpeed = 0f;
                    }
                }
                if(Input.GetAxisRaw(horizontalAxis) == 0) {
                    if(Input.GetAxisRaw(verticalAxis) != 0) {
                        xSpeed = 0;
                        ySpeed = projectileSpeed;
                    } else {
                        xSpeed = projectileSpeed;
                        ySpeed = 0;
                    }
                }



                GameObject sBall = Instantiate(snowBall, snowBallLaunchPoint.transform.position, Quaternion.identity) as GameObject;
                sBall.GetComponent<SnowBall>().playerNum = playerId;


                if(Input.GetAxisRaw(verticalAxis) < 0) {
                    verticalDirection = -1f;
                } else if (Input.GetAxisRaw(verticalAxis) > 0) {
                    verticalDirection = 1f;
                } else {
                    verticalDirection = 0f;
                }

                sBall.GetComponent<Rigidbody2D>().velocity = new Vector3(xSpeed * transform.localScale.x ,ySpeed * verticalDirection ,0);
                hasSnowBall = false;
                snowBallHand.SetActive(false);

            }
            if(Input.GetButtonUp(fire1)) {
                actionNumber = prevNum;
            }




            //rb2D.velocity = Vector3.ClampMagnitude(rb2D.velocity, 200f);



            // STATE HANDLER
            myAnim.SetFloat("Speed", Mathf.Abs(rb2D.velocity.x));
            myAnim.SetFloat("VSpeed", rb2D.velocity.y);
            myAnim.SetBool("Grounded", isGrounded);
            myAnim.SetBool("Duck", isDucking);
            myAnim.SetInteger("Action", actionNumber);


            if (this.myAnim.GetCurrentAnimatorStateInfo(0).IsName("Death")) {
                myAnim.SetBool("GottenRekt", false);
            }
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

    
