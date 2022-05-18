using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;

public class SidewaysCamera : MonoBehaviour
{
    public static float CameraSpeed = .15f;
    public static float DefaultCameraZoom = 14f;
    public static float CameraBorders = 33f;
    public static float CameraSlide = .3f;
    public static float CameraSlideShort = .2f;
    public static SidewaysCamera active;

    Camera cam { get => gameObject.GetComponent<Camera>(); }
    public PixelPerfectCamera ppc { get => gameObject.GetComponent<PixelPerfectCamera>(); }
    
    public Vector2 CameraOrder = Vector2.zero;
    public Vector2 CameraOrigin = Vector2.zero;
    public Vector2 CameraTime = Vector2.zero;
    public Rect CameraBounds;

    private void Awake()
    {
        UpdateCameraBounds(Vector2.zero, new Vector2(20, 20));
        CameraOrder = Vector2.zero;
        CameraOrigin = Vector2.zero;
        CameraTime = Vector2.one;
        active = this;
    }

    void ClearOrder()
    {
        CameraOrder = new Vector2(-1, -1);
        CameraTime = Vector2.zero;
    }

    public void Update()
    {
        if (WorldController.active.currentPhase == WorldController.GamePhase.GameRunning)
        {
            HandleCameraMovement();
        }
    }
    public void FollowMob(Mob mob)
    {
        float borders = cam.orthographicSize * .66f;
        Transform follower = mob.transform;
        Vector2 delta = follower.position - transform.position;

        if (Mathf.Abs(delta.x) > borders * cam.aspect)
        {
            delta.x = (Mathf.Abs(delta.x) - borders * cam.aspect) * (delta.x < 0 ? -1 : 1);
        }
        else
        {
            delta.x = 0;
        }

        if (Mathf.Abs(delta.y) > borders)
        {
            delta.y = (Mathf.Abs(delta.y) - borders) * (delta.y < 0 ? -1 : 1);
        }
        else
        {
            delta.y = 0;
        }
        if (delta.x != 0 || delta.y != 0) { IssueOrder(new Vector3(transform.position.x + delta.x, transform.position.y + delta.y, DefaultCameraZoom), CameraSlideShort,true); }
    }
    public void HandleCameraMovement()
    {
        if (Input.mousePosition.x < CameraBorders)
        {
            Move(-1, 0);
        }
        if (Input.mousePosition.x > Screen.width - CameraBorders)
        {
            Move(1, 0);
        }
        if (Input.mousePosition.y < CameraBorders)
        {
            Move(0, -1);
        }
        if (Input.mousePosition.y > Screen.height - CameraBorders)
        {
            Move(0, 1);
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
                    CameraSlideShort,
                    false
                );
            }
            else
            {
                IssueOrder(
                    new Vector2(
                    CameraOrder.x + X * CameraSpeed,
                    CameraOrder.y + Y * CameraSpeed),
                    CameraSlideShort,
                    false
                );
            }
        }
    }

    public void FixedUpdate()
    {
        if (CameraTime.x + CameraTime.y > Time.time)
        {
            float delta = 1f / (CameraTime.x) * Time.deltaTime;
            if (CameraOrder.x != -1 && CameraOrder.y != 1)
            {
                cam.transform.position = new Vector3(
                    cam.transform.position.x + (CameraOrder.x - CameraOrigin.x) * delta,
                    cam.transform.position.y + (CameraOrder.y - CameraOrigin.y) * delta,
                    cam.transform.position.z);
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
        ClearOrder();
    }

    public void IssueOrder(Vector2 position, bool ignoreBounds)
    {
        IssueOrder(position, CameraSlide, ignoreBounds);
    }
    public void IssueOrder(Vector2 position, float time,bool ignoreBounds)
    {
        Debug.Log("[entityCamera] Move camera at " + position + " over " + time);
        CameraOrigin = new Vector3(transform.position.x, transform.position.y, cam.orthographicSize);
        CameraOrder = position;
        CameraTime = new Vector2(time, Time.time);
        float H = cam.orthographicSize;
        float W = H * cam.aspect;
        Debug.Log("[SidewaysCamera] SnapToBounds " + W + ";" + H);

        if (!ignoreBounds)
        {
            if (W * 2 > CameraBounds.width)
            {
                CameraOrder.x = CameraBounds.center.x;
            }
            else
            {
                CameraOrder.x = Mathf.Clamp(CameraOrder.x, CameraBounds.xMin + W, CameraBounds.xMax - W);
            }
        }

        if (ignoreBounds)
        {
            CameraOrder.y = Mathf.Max(CameraOrder.y, CameraBounds.yMax - H);
        }
        else  if (H * 2 > CameraBounds.height)
        {
            CameraOrder.y = CameraBounds.center.y;
        }
        else
        {
            
                CameraOrder.y = Mathf.Clamp(CameraOrder.y, CameraBounds.yMin + H, CameraBounds.yMax - H);

        }
        if (time < 0)
        { Snap(); }
    }

    public void UpdateCameraBounds(Vector2 min, Vector2 max)
    {
        Vector2 dims = (max - min);
        CameraBounds = new Rect(-dims.x * .5f, -dims.y * .5f, dims.x, dims.y);
        //cam.orthographicSize = Mathf.Min(cam.orthographicSize, CameraBounds.width * .5f, CameraBounds.height * .5f);
        Debug.Log("[entityCamera] Update Camera Bounds " + CameraBounds);
    }
    public void FollowAction()
    {
        for (int iMob = WorldController.active.MobsInMotion.Count-1; iMob>=0; iMob--)
        {
            Mob selMob = WorldController.active.MobsInMotion[iMob];
            if (selMob!=null && selMob.gameObject.activeInHierarchy && selMob.IsInMotion())
            {
                IssueOrder(selMob.transform.position,true);
            }
        }
    }
}