using UnityEngine;
using UnityEngine.UI;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Image customCursor;

    private void Awake()
    {
        if (menuPanel == null || customCursor == null)
        {
            Debug.LogError(
                this
            );

            enabled = false;
            return;
        }

        customCursor.raycastTarget = false;
    }

    private void LateUpdate()
    {
        bool menuOpen = menuPanel.activeInHierarchy;

        if (customCursor.gameObject.activeSelf == menuOpen)
            customCursor.gameObject.SetActive(!menuOpen);

        if (menuOpen)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
