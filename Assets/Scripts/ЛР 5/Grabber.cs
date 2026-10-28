using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class Grabber : MonoBehaviour
{
    [SerializeField] private RaycastSelector selector;
    [SerializeField] private Transform viewCamera;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwImpulse = 3f;

    private Rigidbody heldBody;
    private Vector3 heldOffset;
    private Quaternion heldRotation;

    private Collider[] heldColliders;
    private bool[] colliderStates;

    private bool originalGravity;
    private bool originalKinematic;
    private CollisionDetectionMode originalCollisionMode;

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        InputInteract();
    }

    private void LateUpdate()
    {
        if (heldBody != null)
            MoveToHoldPoint();
    }

    private void MoveToHoldPoint()
    {
        heldBody.transform.SetPositionAndRotation(
            holdPoint.TransformPoint(heldOffset),
            holdPoint.rotation * heldRotation);
    }

    private void PlaceOnPalm(Transform item)
    {
        Renderer[] renderers = item.GetComponentsInChildren<Renderer>();
        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;
        bool foundBounds = false;

        foreach (Renderer itemRenderer in renderers)
        {
            if (!itemRenderer.enabled || !itemRenderer.gameObject.activeInHierarchy)
                continue;

            Bounds bounds = itemRenderer.localBounds;

            // восемь углов каждого меша в координатах точки руки
            for (int i = 0; i < 8; i++)
            {
                Vector3 corner = bounds.center + Vector3.Scale(
                    bounds.extents,
                    new Vector3(
                        (i & 1) == 0 ? -1f : 1f,
                        (i & 2) == 0 ? -1f : 1f,
                        (i & 4) == 0 ? -1f : 1f));

                Vector3 worldCorner = itemRenderer.transform.TransformPoint(corner);
                Vector3 localCorner = holdPoint.InverseTransformPoint(worldCorner);
                min = Vector3.Min(min, localCorner);
                max = Vector3.Max(max, localCorner);
                foundBounds = true;
            }
        }

        if (!foundBounds)
            return;

        Vector3 center = (min + max) * 0.5f;
        // Нижний край модели касается поверхности руки
        heldOffset = new Vector3(-center.x, -min.y, -center.z);
        MoveToHoldPoint();
    }

    public void InputInteract()
    {
        if (!Input.GetKeyDown(KeyCode.E))
            return;

        if (heldBody != null)
            Release(true);
        else
            TryGrab();
    }    

    private void TryGrab()
    {
        if (selector == null || holdPoint == null)
            return;

        GameObject target = selector.SelectedObject;

        if (target == null)
            return;

        // Брать можно только объекты слоя Interact
        int interactLayer = LayerMask.NameToLayer("Interact");

        if (interactLayer == -1 || target.layer != interactLayer)
            return;

        Rigidbody body = target.GetComponent<Rigidbody>();

        if (body == null)
            return;

        // Для броска нужен предмет с динамическим Rigidbody
        if (body.isKinematic)
        {
            Debug.LogWarning(
                "Для подбора отключи Is Kinematic у предмета.",
                target);

            return;
        }

        heldBody = body;

        originalGravity = body.useGravity;
        originalKinematic = body.isKinematic;
        originalCollisionMode = body.collisionDetectionMode;

        heldColliders = body.GetComponentsInChildren<Collider>(true);
        colliderStates = new bool[heldColliders.Length];

        // Останавливаем движение перед взятием
        body.linearVelocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;

        body.collisionDetectionMode = CollisionDetectionMode.Discrete;
        body.useGravity = false;
        body.isKinematic = true;

        for (int i = 0; i < heldColliders.Length; i++)
        {
            colliderStates[i] = heldColliders[i].enabled;
            heldColliders[i].enabled = false;
        }

        // Следуем за точкой в мировых координатах, сохраняя масштаб предмета.
        // Родитель остаётся прежним, поэтому отключение камеры безопасно.
        heldOffset = Vector3.zero;
        heldRotation = Quaternion.Inverse(holdPoint.rotation) * body.transform.rotation;
        MoveToHoldPoint();
        PlaceOnPalm(body.transform);
    }

    private void Release(bool throwObject)
    {
        if (heldBody == null)
            return;

        Rigidbody body = heldBody;

        for (int i = 0; i < heldColliders.Length; i++)
        {
            if (heldColliders[i] != null)
                heldColliders[i].enabled = colliderStates[i];
        }

        body.isKinematic = originalKinematic;
        body.useGravity = originalGravity;
        body.collisionDetectionMode = originalCollisionMode;

        if (!body.isKinematic)
        {
            body.WakeUp();

            if (throwObject)
            {
                Vector3 direction = viewCamera != null
                    ? viewCamera.forward
                    : transform.forward;

                body.AddForce(
                    direction * throwImpulse,
                    ForceMode.Impulse);
            }
        }

        heldBody = null;
        heldColliders = null;
        colliderStates = null;
    }

    private void OnDisable()
    {
        // При отключении скрипта возвращаем предмету физику
        Release(false);
    }
}
