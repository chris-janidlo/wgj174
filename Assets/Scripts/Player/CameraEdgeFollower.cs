using System.Collections;
using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

namespace Drepanoid
{
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
        
        // assumes camera is at a negative z position
        Vector2 cameraExtentsInWorldSpace =>
            Camera.ScreenToWorldPoint(new Vector3(cameraPixelDimensions.x, cameraPixelDimensions.y, -transform.position.z)) -           // top right of camera, at z position 0, in world space
            Camera.ScreenToWorldPoint(new Vector3(cameraPixelDimensions.x / 2, cameraPixelDimensions.y / 2, -transform.position.z));    // middle of camera, at z position 0, in world space

        Vector2 bottomLeftMoveLimit => (Vector2) BottomLeftBoundary.transform.position + cameraExtentsInWorldSpace;
        Vector2 topRightMoveLimit => (Vector2) TopRightBoundary.transform.position - cameraExtentsInWorldSpace;

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

            clampToBoundaries();

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

        bool onBoundary ()
        {
            // TODO: could be improved by taking movement room into account
            return
                (transform.position.x == bottomLeftMoveLimit.x || transform.position.x == topRightMoveLimit.x) &&
                (transform.position.y == bottomLeftMoveLimit.x || transform.position.y == topRightMoveLimit.y);
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
            var previousTargetScreenPos = Camera.WorldToScreenPoint(targetPreviousPosition);

            Vector3 cameraDelta = new Vector3
            (
                trackOnAxis(targetDelta.x, targetScreenPos.x, previousTargetScreenPos.x, edgeThicknessPixels.x, cameraPixelDimensions.x),
                trackOnAxis(targetDelta.y, targetScreenPos.y, previousTargetScreenPos.y, edgeThicknessPixels.y, cameraPixelDimensions.y)
            );

            transform.position += cameraDelta;
        }

        float trackOnAxis (float worldDelta, float currentScreenPos, float previousScreenPos, float edgePixels, float camPixelDimension)
        {
            float low = edgePixels, high = camPixelDimension - edgePixels;

            bool shouldTrack =
                (worldDelta < 0 && currentScreenPos <= low && previousScreenPos <= low) ||
                (worldDelta > 0 && currentScreenPos >= high && previousScreenPos >= high);

            return shouldTrack ? worldDelta : 0;
        }

        void clampToBoundaries ()
        {
            bool noRoomX = bottomLeftMoveLimit.x >= topRightMoveLimit.x;
            bool noRoomY = bottomLeftMoveLimit.y >= topRightMoveLimit.y;

            var boundaryCenter = (BottomLeftBoundary.transform.position + TopRightBoundary.transform.position) / 2;

            transform.position = new Vector3
            (
                noRoomX ? boundaryCenter.x : Mathf.Clamp(transform.position.x, bottomLeftMoveLimit.x, topRightMoveLimit.x),
                noRoomY ? boundaryCenter.y : Mathf.Clamp(transform.position.y, bottomLeftMoveLimit.y, topRightMoveLimit.y),
                transform.position.z
            );
        }
    }
}
