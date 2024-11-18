# nullable enable
using MultiPlayerGame.Network;
using Network.Protocol;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MultiPlayerGame.Game
{
    public class PlayerController : MonoBehaviour
    {
        public float MoveSpeed = 5f;

        public float JumpForce = 5f;

        public int PlayerID { get; set; }

        [SerializeField] Vector2 _groundDetectionRange;

        [SerializeField] Vector2 _groundDetectionOffset;

        private Rigidbody2D rb = null!;

        private int _remainningJumpCount = 2;

        private float _hasMovementBuffer;

        private bool _hasJumpBuffer;

        private bool _hasFireBuffer;

        private bool _hasDownBuffer;

        private Client Client { get; set; }

        private SyncPlayerPositionRequest _syncPlayerPositionRequest = null!;

        private void Awake() {
            rb = GetComponentInChildren<Rigidbody2D>();
            Client = Services.Instance.GetService<Client>();
            _syncPlayerPositionRequest = new();
            _syncPlayerPositionRequest.PlayerPosition = new();
            _syncPlayerPositionRequest.PlayerRotation = new();
        }

        private void FixedUpdate() {
            var hit = Physics2D.OverlapBox((Vector2)transform.position + _groundDetectionOffset, _groundDetectionOffset, angle: 0);
            if (hit != null && rb.velocity.y <= 0) {
                _remainningJumpCount = 2;
            }
            HandleMovement();
            HandleJump();
            HandleFire();
            HandleCrossPlatform();

            _syncPlayerPositionRequest.RequesterID = PlayerID;
            _syncPlayerPositionRequest.PlayerPosition.Set(transform.position);
            _syncPlayerPositionRequest.PlayerRotation.Set(transform.eulerAngles);
            Client.SendMessageAsync(_syncPlayerPositionRequest);
        }

        private void Update() {
            _hasMovementBuffer = Input.GetAxis("Horizontal");
            _hasJumpBuffer |= Input.GetKeyDown(KeyCode.K);
            _hasFireBuffer |= Input.GetKeyDown(KeyCode.J);
            _hasDownBuffer |= Input.GetKeyDown(KeyCode.S);
        }

        private void HandleMovement() {
            if (_hasMovementBuffer == 0) return;

            if (_hasMovementBuffer != 0) {
                transform.eulerAngles = new Vector3(0, _hasMovementBuffer >= 0 ? 0 : 180);
                transform.position += Vector3.right * _hasMovementBuffer * MoveSpeed * Time.deltaTime; ;
            }
            _hasMovementBuffer = 0;
        }

        private void HandleJump() {
            if (_hasJumpBuffer == false) return;

            if (_remainningJumpCount > 0) {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
                _remainningJumpCount--;
            }

            _hasJumpBuffer = false;
        }

        private void HandleFire() {
            if (_hasFireBuffer == false) return;

            var fireRequest = new FireRequest() {
                RequesterID = PlayerID
            };
            Client.SendMessageAsync(fireRequest);

            _hasFireBuffer = false;
        }

        private void HandleCrossPlatform() {
            if (_hasDownBuffer == false) return;

            var request = new CrossPlatformRequest() {
                RequesterID = PlayerID
            };
            Client.SendMessageAsync(request);

            _hasDownBuffer = false;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + (Vector3)_groundDetectionOffset, _groundDetectionRange);
        }
    }
}
