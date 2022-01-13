using GameCreator.Elements;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GameCreator.Game
{
    /// <summary>
    /// Simple player controller based on <see cref="https://docs.unity3d.com/ScriptReference/CharacterController.Move.html"/>
    /// </summary>
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class JumperPlayer : GameElement
    {
        [SerializeField] private ThirdPersonCamera viewCamera;

        [SerializeField] private float playerSpeed = 2.0f;
        [SerializeField] private float jumpHeight = 1.0f;
        [SerializeField] private float gravityValue = -9.81f;
        [SerializeField] private Animator animator;

        private CharacterController controller;
        private PlayerInput input;
        private Vector3 playerVelocity;
        private bool isGrounded;

        private System.Action updateBehaviour;

        private void Start()
        {
            animator.Play("Idle");
        }

        protected override void OnBeforeSpawn()
        {
            base.OnBeforeSpawn();

            controller = GetComponent<CharacterController>();
            input = GetComponent<PlayerInput>();
        }

        protected override void OnSpawnInCreator()
        {
            base.OnSpawnInCreator();

            controller.enabled = false;
            input.enabled = false;
            updateBehaviour = null;
        }

        protected override void OnSpawnInGame()
        {
            base.OnSpawnInGame();


            controller.enabled = true;
            input.enabled = true;

            GameCamera gameCam = GameController.instance._camera;
            ThirdPersonCamera cam = gameCam.GetComponent<ThirdPersonCamera>();

            if (cam != null)
            {
                viewCamera = cam;
                viewCamera.SetTarget(transform);
            }

            updateBehaviour = GameUpdate;
        }

        private void Update()
        {
            updateBehaviour?.Invoke();
        }

        private void GameUpdate()
        {
            isGrounded = controller.isGrounded;
            if (isGrounded && playerVelocity.y < 0)
            {
                animator.SetBool("Jump", false);
                playerVelocity.y = 0f;
            }

            Vector2 moveInput = input.actions["Move"].ReadValue<Vector2>();

            Vector3 forward = viewCamera.flatForward;
            Vector3 right = Vector3.Cross(forward, Vector3.up);
            Vector3 offset = (right * -moveInput.x) + (forward * moveInput.y);

            Vector3 move = offset;
            if (move != Vector3.zero)
            {
                gameObject.transform.forward = move;
                animator.SetFloat("Speed", move.magnitude);
            }
            else
            {
                animator.SetFloat("Speed", 0);
            }
            controller.Move(move * Time.deltaTime * playerSpeed);

            // Changes the height position of the player..
            if (input.actions["Jump"].triggered && isGrounded)
            {
                animator.SetBool("Jump", true);
                playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            }

            playerVelocity.y += gravityValue * Time.deltaTime;
            controller.Move(playerVelocity * Time.deltaTime);
        }
    }
}