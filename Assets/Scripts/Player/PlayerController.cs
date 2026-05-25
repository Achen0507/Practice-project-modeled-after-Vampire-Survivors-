using UnityEngine;
using UnityEngine.Tilemaps;

namespace Survivor.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        public Tilemap groundTilemap;

        private Rigidbody2D rb;
        private Animator animator;
        private Vector2 moveInput;
        private float minX, maxX, minY, maxY;
        private bool hasBounds = false;
        private int lastFacingDirection = 1;  // 1=右, -1=左
        public Vector2 FacingDirection { get; private set; } = Vector2.right;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();

            // 获取 Tilemap 边界
            if (groundTilemap != null)
            {
                Bounds bounds = groundTilemap.localBounds;
                minX = bounds.min.x;
                maxX = bounds.max.x;
                minY = bounds.min.y;
                maxY = bounds.max.y;
                hasBounds = true;
            }
        }
        private void Update()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");

            // 归一化
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            UpdateAnimation();
        }

        private void FixedUpdate() {
            Vector2 newPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

            // 限制玩家边界
            if (hasBounds)
            {
                newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
            }

            rb.MovePosition(newPos);
        }

        private void UpdateAnimation() {
            if (animator == null) return;
            bool isMoving = moveInput.magnitude > 0.1f;

            animator.SetBool("IsMoving", isMoving);

            if (moveInput.x != 0)
            {
                lastFacingDirection = moveInput.x > 0 ? 1 : -1;
                UpdateSpriteFacing();
            }
            else if (isMoving) {
                UpdateSpriteFacing();
            }
        }

        private void UpdateSpriteFacing() {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * lastFacingDirection;
            transform.localScale = scale;

            FacingDirection = lastFacingDirection == 1 ? Vector2.right : Vector2.left;
        }

        public void SetMoveSpeed(float speed)
        {
            moveSpeed = speed;
        }
        public float GetMoveSpeed() => moveSpeed;
    }
}
