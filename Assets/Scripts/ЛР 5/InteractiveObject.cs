using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class InteractiveObject : MonoBehaviour, IInteractive
{
    [SerializeField] private string objectName;
    [SerializeField] private GameObject namePanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Vector3 nameOffset = new Vector3(0f, 0.5f, 0f);

    private bool selected;
    private Canvas nameCanvas;
    private Transform viewCamera;

    public string Name => string.IsNullOrWhiteSpace(objectName)
            ? gameObject.name
            : objectName;

    private void Awake()
    {
        if (namePanel != null)
            nameCanvas = namePanel.GetComponentInParent<Canvas>(true);

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
            viewCamera = mainCamera.transform;
    }

    private void LateUpdate()
    {
        if (selected)
            UpdateNamePosition();
    }

    private void UpdateNamePosition()
    {
        if (nameCanvas == null || nameCanvas.renderMode != RenderMode.WorldSpace)
            return;

        nameCanvas.transform.position = transform.position + nameOffset;

        if (viewCamera != null)
            nameCanvas.transform.rotation = viewCamera.rotation;
    }   

    public void Interact()
    {
        if (namePanel == null || nameText == null)
            return;

        selected = true;
        nameText.text = Name;
        UpdateNamePosition();
        namePanel.SetActive(true);
    }

    public void Revert()
    {
        if (!selected)
            return;

        selected = false;

        if (namePanel != null)
            namePanel.SetActive(false);
    }

    private void OnDisable()
    {
        Revert();
    }
}
