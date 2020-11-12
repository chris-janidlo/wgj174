using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
    // TODO: this is super buggy
    public class CameraEdgeFollower : MonoBehaviour
    {
        public Vector2Variable TargetPosition;
        public float ChaseTime;

        public Vector2 EdgeThickness; // how far from the edge you have to be to trigger following. 0 is exactly on the edge, 1 is in the center of the screen.
        public CameraBoundsMarker BottomLeftBoundary, TopRightBoundary;

        public Camera Camera;

        Vector2 targetPreviousPosition;
        Vector2 chaseVelocity;

        Vector2 cameraPixelDimensions => new Vector2(Camera.pixelWidth, Camera.pixelHeight);
        Vector2 edgeThicknessPixels => new Vector2
        (
            EdgeThickness.x * cameraPixelDimensions.x / 2,
            EdgeThickness.y * cameraPixelDimensions.y / 2
        );

        Vector2 bottomLeftMoveLimit =>
            BottomLeftBoundary.transform.position + transform.position - Camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 topRightMoveLimit =>
            TopRightBoundary.transform.position - transform.position + Camera.ScreenToWorldPoint(cameraPixelDimensions);

        bool chasing;

        void Update ()
        {
            if (shouldChase())
            {
                chase();
            }
            else
            {
                chaseVelocity = Vector2.zero;
                track();
            }

            transform.position = new Vector3
            (
                Mathf.Clamp(transform.position.x, bottomLeftMoveLimit.x, topRightMoveLimit.x),
                Mathf.Clamp(transform.position.y, bottomLeftMoveLimit.y, topRightMoveLimit.y),
                transform.position.z
            );

            targetPreviousPosition = TargetPosition.Value;
        }

        bool shouldChase ()
        {
            var targetScreenPos = Camera.WorldToScreenPoint(TargetPosition.Value);

            // only start chasing if completely outside the camera, and only stop chasing once inside the edge
            Vector2 chaseLowerBounds = chasing ? edgeThicknessPixels : Vector2.zero;
            Vector2 chaseUpperBounds = chasing ? cameraPixelDimensions - edgeThicknessPixels : cameraPixelDimensions;

            return chasing = !onBoundary() &&
                (targetScreenPos.x < chaseLowerBounds.x || targetScreenPos.x > chaseUpperBounds.x ||
                targetScreenPos.y < chaseLowerBounds.y || targetScreenPos.y > chaseUpperBounds.y);
        }

        void chase ()
        {
            var newPos = Vector2.SmoothDamp(transform.position, TargetPosition.Value, ref chaseVelocity, ChaseTime);
            transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
        }

        void track ()
        {
            var targetDelta = TargetPosition.Value - targetPreviousPosition;
            var targetScreenPos = Camera.WorldToScreenPoint(TargetPosition.Value);

            Vector3 cameraDelta = new Vector3
            (
                trackOnAxis(targetDelta.x, targetScreenPos.x, edgeThicknessPixels.x, cameraPixelDimensions.x),
                trackOnAxis(targetDelta.y, targetScreenPos.y, edgeThicknessPixels.y, cameraPixelDimensions.y)
            );

            transform.position += cameraDelta;
        }

        bool onBoundary ()
        {
            return
                (transform.position.x == bottomLeftMoveLimit.x || transform.position.x == topRightMoveLimit.x) &&
                (transform.position.y == bottomLeftMoveLimit.x || transform.position.y == topRightMoveLimit.y);
        }

        float trackOnAxis (float delta, float screenPos, float edgePixels, float camPixelDimension)
        {
            bool shouldTrack =
                (delta < 0 && screenPos <= edgePixels) ||
                (delta > 0 && screenPos >= camPixelDimension - edgePixels);

            return shouldTrack ? delta : 0;
        }
    }
}
