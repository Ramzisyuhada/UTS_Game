using RengeGames.HealthBars;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Invector.vCharacterController
{
    public class vThirdPersonController : vThirdPersonAnimator
    {

        private float Score;
        public string parentName = "Player";
        public string healthBarName = "Primary";
        public GameObject _h;
        private float Waktu = 60f;
        public TMP_Text WaktuText;
        public TMP_Text ScoreText;
        public TMP_Text TotalScore;

        public GameObject Image;
        public float health = 1;
        private static bool playerExists;

        private void Awake()
        {
            Time.timeScale = 1f;

            //if (!playerExists)
            //{
            //    DontDestroyOnLoad(gameObject);
            //    playerExists = true;
            //}
            //else
            //{
            //    Destroy(gameObject); // Hapus duplicate
            //}
        }
        void Start()
        {
            StatusBarsManager.SetPercent(parentName, healthBarName, health);

            StartCoroutine(WaktuMulai());
        }
        IEnumerator WaktuMulai()
        {
            while (Waktu > 0)
            {
                WaktuText.text = "Sisa Waktu : "+Waktu.ToString();
                yield return new WaitForSeconds(1f);
                Waktu--;

            }
            WaktuText.text = "Selesai   ";
            Time.timeScale = 0f; 

            TotalScore.text = "Kamu Mendapatkan Score : " + Score.ToString();
            Image.SetActive(true);
        }
        private void Update()
        {
            if(health <     0.1)
            {
                Time.timeScale = 0f;
                Image.SetActive(true);
                TotalScore.text = "Kamu Mendapatkan Score : " + Score.ToString();

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Merah"))
            {
                Score += 20;
                Debug.Log("Merah");
            }
            else if (other.gameObject.CompareTag("Kuning"))
            {
                Score += 30;
                Debug.Log("Kuning");
                GameObject[] obj = GameObject.FindGameObjectsWithTag("Lampu");
                foreach (GameObject obj2 in obj) { 
                    obj2.GetComponent<Light>().enabled = true;
                }
            }
            else if (other.gameObject.CompareTag("Hijau"))
            {
                Score += 100;
                Debug.Log("Hijau");
            }
            else if (other.gameObject.CompareTag("Biru"))
            {
                health += 0.1f;


            }
            else if (other.gameObject.CompareTag("Hitam"))
            {
                health -= 0.2f;


            }
            StatusBarsManager.SetPercent(parentName, healthBarName, health);

            Destroy(other.gameObject);
            ScoreText.text = "Score : "+ Score.ToString();
        }
        public virtual void ControlAnimatorRootMotion()
        {
            if (!this.enabled) return;

            if (inputSmooth == Vector3.zero)
            {
                transform.position = animator.rootPosition;
                transform.rotation = animator.rootRotation;
            }

            if (useRootMotion)
                MoveCharacter(moveDirection);
        }

        public virtual void ControlLocomotionType()
        {
            if (lockMovement) return;

            if (locomotionType.Equals(LocomotionType.FreeWithStrafe) && !isStrafing || locomotionType.Equals(LocomotionType.OnlyFree))
            {
                SetControllerMoveSpeed(freeSpeed);
                SetAnimatorMoveSpeed(freeSpeed);
            }
            else if (locomotionType.Equals(LocomotionType.OnlyStrafe) || locomotionType.Equals(LocomotionType.FreeWithStrafe) && isStrafing)
            {
                isStrafing = true;
                SetControllerMoveSpeed(strafeSpeed);
                SetAnimatorMoveSpeed(strafeSpeed);
            }

            if (!useRootMotion)
                MoveCharacter(moveDirection);
        }

        public virtual void ControlRotationType()
        {
            if (lockRotation) return;

            bool validInput = input != Vector3.zero || (isStrafing ? strafeSpeed.rotateWithCamera : freeSpeed.rotateWithCamera);

            if (validInput)
            {
                // calculate input smooth
                inputSmooth = Vector3.Lerp(inputSmooth, input, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);

                Vector3 dir = (isStrafing && (!isSprinting || sprintOnlyFree == false) || (freeSpeed.rotateWithCamera && input == Vector3.zero)) && rotateTarget ? rotateTarget.forward : moveDirection;
                RotateToDirection(dir);
            }
        }

        public virtual void UpdateMoveDirection(Transform referenceTransform = null)
        {
            if (input.magnitude <= 0.01)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, (isStrafing ? strafeSpeed.movementSmooth : freeSpeed.movementSmooth) * Time.deltaTime);
                return;
            }

            if (referenceTransform && !rotateByWorld)
            {
                //get the right-facing direction of the referenceTransform
                var right = referenceTransform.right;
                right.y = 0;
                //get the forward direction relative to referenceTransform Right
                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
            }
            else
            {
                moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
            }
        }

        public virtual void Sprint(bool value)
        {
            var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded &&
                !(isStrafing && !strafeSpeed.walkByDefault && (horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f)));

            if (value && sprintConditions)
            {
                if (input.sqrMagnitude > 0.1f)
                {
                    if (isGrounded && useContinuousSprint)
                    {
                        isSprinting = !isSprinting;
                    }
                    else if (!isSprinting)
                    {
                        isSprinting = true;
                    }
                }
                else if (!useContinuousSprint && isSprinting)
                {
                    isSprinting = false;
                }
            }
            else if (isSprinting)
            {
                isSprinting = false;
            }
        }

        public virtual void Strafe()
        {
            isStrafing = !isStrafing;
        }

        public virtual void Jump()
        {
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;
            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
        }
    }
}