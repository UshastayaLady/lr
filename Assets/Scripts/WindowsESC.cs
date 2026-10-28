using UnityEngine;

public class WindowsESC : MonoBehaviour
{
    [SerializeField] private MovePlayer _plaer;
    [SerializeField] private InterfaceController _windowECS;
    private bool _openWindows;

    private void Start()
    {
        SetMenuOpen(_windowECS.gameObject.activeSelf);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetMenuOpen(!_openWindows);
        }
    }

    private void SetMenuOpen(bool open)
    {
        _openWindows = open;
        _windowECS.gameObject.SetActive(open);
        _plaer.enabled = !open;
    }

}
