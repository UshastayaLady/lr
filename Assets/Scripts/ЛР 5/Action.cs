using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
public class Action : MonoBehaviour, IInteractive
{
    [SerializeField, Min(0f)]
    private float gravity = 10f;

    [SerializeField, Min(0.01f)]
    private float gravityStep = 1f;

    private Rigidbody body;
    private bool originalUseGravity;

    public string Name => gameObject.name;
    public float Gravity => gravity;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        originalUseGravity = body.useGravity;

        // Отключаем стандартную гравитацию: свою прикладываем в FixedUpdate
        body.useGravity = false;
    }

    private void FixedUpdate()
    {
        // В руке предмет кинематический — прикладывать к нему силу не нужно
        if (body.isKinematic)
            return;

        body.AddForce(
            Vector3.down * gravity,
            ForceMode.Acceleration);
    }

    public void Interact()
    {
        if (Input.GetKeyDown(KeyCode.T))
            ChangeGravity(-gravityStep);

        if (Input.GetKeyDown(KeyCode.Y))
            ChangeGravity(gravityStep);
    }

    public void Revert()
    {
        // При отведении взгляда сохраняем
        // установленное значение гравитации.
    }

    private void ChangeGravity(float delta)
    {
        // Ниже нуля не опускаемся: ноль соответствует отсутствию гравитации
        gravity = Mathf.Max(0f, gravity + delta);

        if (!body.isKinematic)
            body.WakeUp();

        Debug.Log(
            $"{Name}: ускорение гравитации = {gravity:F1} м/с²",
            this);
    }

    private void OnDisable()
    {
        if (body != null)
            body.useGravity = originalUseGravity;
    }
}