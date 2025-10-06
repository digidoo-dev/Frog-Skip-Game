using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera usedCamera;

    [SerializeField] private float zoomedInOrthographicSize = 5f;
    [SerializeField] private float halfZoomOrthographicSize = 10f;
    [SerializeField] private float zoomedOutOrthographicSize = 15f;

    [SerializeField] private float zoomChangeSpeed = 1f;

    private Transform transformToFollow;

    private enum ZoomState
    {
        ZoomedIn,
        HalfZoom,
        ZoomedOut
    }

    private ZoomState zoomState = ZoomState.ZoomedIn;
    private bool zoomInProgress = false;



    public void SetFollowedTransform(Transform transformToFollow)
    {
        this.transformToFollow = transformToFollow;
    }

    public void ChangeZoom()
    {
        zoomInProgress = true;
        if (zoomState == ZoomState.ZoomedIn) zoomState = ZoomState.HalfZoom;
        else if (zoomState == ZoomState.HalfZoom) zoomState = ZoomState.ZoomedOut;
        else if (zoomState == ZoomState.ZoomedOut) zoomState = ZoomState.ZoomedIn;
    }

    private void LateUpdate()
    {
        if (transformToFollow != null)
        {
            transform.position = transformToFollow.position;
        }


        if (!zoomInProgress) return;

        var orthoSize = usedCamera.orthographicSize;

        if (zoomState == ZoomState.ZoomedIn)
        {
            if (orthoSize != zoomedInOrthographicSize)
            {
                usedCamera.orthographicSize -= zoomChangeSpeed * Time.deltaTime;
                if (usedCamera.orthographicSize < zoomedInOrthographicSize) usedCamera.orthographicSize = zoomedInOrthographicSize;
            }
            else zoomInProgress = false;
        }
        else if (zoomState == ZoomState.HalfZoom)
        {
            if (orthoSize < halfZoomOrthographicSize)
            {
                usedCamera.orthographicSize += zoomChangeSpeed * Time.deltaTime;
                if (usedCamera.orthographicSize > halfZoomOrthographicSize) usedCamera.orthographicSize = halfZoomOrthographicSize;
            }
            else if (orthoSize > halfZoomOrthographicSize)
            {
                usedCamera.orthographicSize -= zoomChangeSpeed * Time.deltaTime;
                if (usedCamera.orthographicSize > halfZoomOrthographicSize) usedCamera.orthographicSize = halfZoomOrthographicSize;
            }
            else zoomInProgress = false;
        }
        else if (zoomState == ZoomState.ZoomedOut)
        {
            if (orthoSize != zoomedOutOrthographicSize)
            {
                usedCamera.orthographicSize += zoomChangeSpeed * Time.deltaTime;
                if (usedCamera.orthographicSize > zoomedOutOrthographicSize) usedCamera.orthographicSize = zoomedOutOrthographicSize;
            }
            else zoomInProgress = false;
        }
    }
}
