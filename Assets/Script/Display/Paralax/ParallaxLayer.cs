using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector2 parallaxFactor = new Vector2(0.5f, 0.5f);
    public bool loopHorizontal;
    public bool loopVertical;

    private Vector3 lastCameraPosition;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPosition = cameraTransform.position;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            textureUnitSizeX = sr.sprite.bounds.size.x * transform.lossyScale.x;
            textureUnitSizeY = sr.sprite.bounds.size.y * transform.lossyScale.y;
        }
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;
        Vector3 movement = new Vector3(delta.x * parallaxFactor.x, delta.y * parallaxFactor.y, 0f);
        transform.position += movement;
        lastCameraPosition = cameraTransform.position;

        if (loopHorizontal && textureUnitSizeX > 0f)
        {
            float distanceX = cameraTransform.position.x - transform.position.x;
            if (Mathf.Abs(distanceX) >= textureUnitSizeX)
            {
                float offsetX = distanceX % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetX, transform.position.y, transform.position.z);
            }
        }

        if (loopVertical && textureUnitSizeY > 0f)
        {
            float distanceY = cameraTransform.position.y - transform.position.y;
            if (Mathf.Abs(distanceY) >= textureUnitSizeY)
            {
                float offsetY = distanceY % textureUnitSizeY;
                transform.position = new Vector3(transform.position.x, cameraTransform.position.y + offsetY, transform.position.z);
            }
        }
    }
}