using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;

public class SidewaysCamera : MonoBehaviour
{
    public static float CameraSpeed = .15f;
    public static float DefaultCameraZoom = 7f;
    public static float CameraBorders = 33f;
    public static float CameraSlide = .1f;
    public static float CameraSlideShort = .15f;
    public static SidewaysCamera active;

    Camera cam { get => gameObject.GetComponent<Camera>(); }
    public PixelPerfectCamera ppc { get => gameObject.GetComponent<PixelPerfectCamera>(); }

    public Vector2 CameraOrder = Vector2.zero;
    public Vector2 CameraRotation = Vector2.zero;
    public Vector2 CameraOrigin = Vector2.zero;
    public Vector2 CameraTime = Vector2.zero;

    private void Awake()
    {
        CameraOrder = Vector2.zero;
        CameraOrigin = Vector2.zero;
        CameraRotation = Vector2.zero;
        CameraTime = Vector2.one;
        active = this;
    }

    void ClearOrder()
    {
        CameraOrder = new Vector2(-1, -1);
        CameraTime = Vector2.zero;
    }
    bool IsIdle()
    {
        return CameraOrder.x <= 0;
    }

    Mob followMob;
    public void FollowMob(Mob mob)
    {
        if ( mob != null && mob.gameObject.activeInHierarchy )
        {
            followMob = mob;
        }
    }

    public void Move(float X, float Y)
    {
        if (X != 0 || Y != 0)
        {
            if (CameraTime.x == 0)
            {
                IssueOrder(
                    new Vector2(
                    transform.position.x + X * CameraSpeed,
                    transform.position.y + Y * CameraSpeed),
                    CameraSlideShort
                );
            }
            else
            {
                IssueOrder(
                    new Vector2(
                    CameraOrder.x + X * CameraSpeed,
                    CameraOrder.y + Y * CameraSpeed),
                    CameraSlideShort
                );
            }
        }
    }

    public void Update()
    {
        if (followMob != null )
        {
            cam.transform.position = new Vector3(
                followMob.transform.position.x,
                followMob.transform.position.y,
                    cam.transform.position.z
                );
            cam.transform.rotation = followMob.transform.rotation;
        }
        else if (CameraTime.x + CameraTime.y > Time.time)
        {
            float delta = (Time.time - CameraTime.y) / CameraTime.x; //1f / (CameraTime.x) * Time.fixedDeltaTime;
            if (CameraOrder.x != -1 && CameraOrder.y != 1)
            {
                cam.transform.position = new Vector3(
                    CameraOrigin.x + (CameraOrder.x - CameraOrigin.x) * delta,
                    CameraOrigin.y + (CameraOrder.y - CameraOrigin.y) * delta,
                    cam.transform.position.z);
            }
            if (CameraRotation.x != CameraRotation.y)
            {
                cam.transform.rotation *= Quaternion.Euler(0,0, Mathf.DeltaAngle(CameraRotation.y , CameraRotation.x) * delta);
            }
        }
        else
        {
            Snap();
        }
    }

    public void Snap()
    {
        if (CameraOrder.x != -1 && CameraOrder.y != 1)
        {
            cam.transform.position = new Vector3(CameraOrder.x, CameraOrder.y, cam.transform.position.z);
        }
        if (CameraRotation.x != CameraRotation.y)
        {
            cam.transform.rotation = Quaternion.Euler(0, 0, CameraRotation.x);
        }
        ClearOrder();
    }

    public void IssueOrder(Vector2 position, float rotation)
    {
        IssueOrder(position,  rotation, CameraSlide);
    }
    public void IssueOrder(Vector2 position, float rotation, float time)
    {
        Debug.Log("[entityCamera] Move camera at " + position + " over " + time);
        CameraOrigin = new Vector3(transform.position.x, transform.position.y, cam.orthographicSize);
        CameraOrder = position;
        CameraRotation = new Vector2(rotation, transform.rotation.eulerAngles.z);
        CameraTime = new Vector2(time, Time.time);
        float H = cam.orthographicSize;
        float W = H * cam.aspect;
        Debug.Log("[SidewaysCamera] SnapToBounds " + W + ";" + H);

        if (time < 0)
        { Snap(); }
    }

}