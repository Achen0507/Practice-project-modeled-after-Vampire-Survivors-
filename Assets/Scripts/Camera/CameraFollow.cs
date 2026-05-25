using UnityEngine;
using UnityEngine.Tilemaps;

namespace Survivor.Cam
{  
    public class CameraFollow : MonoBehaviour
    {
        public Tilemap groundTilemap;

        [SerializeField] private Transform target;    
        [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);  // main相机偏移
        [SerializeField] private float smoothSpeed = 5f;  // 相机跟随平滑度

        private float minX, maxX, minY, maxY;
        private bool hasBounds = false;

        private void Start()
        {
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

        private void LateUpdate()
        {
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null) target = player.transform;
                return;
            }

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

            // 应用边界限制
            if (hasBounds) {
                float halfHeight = Camera.main.orthographicSize;
                float halfWidth = halfHeight * Camera.main.aspect;
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX + halfWidth, maxX - halfWidth);
                smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY + halfHeight, maxY - halfHeight);
            }
            transform.position = smoothedPosition;
        }
    }
}
