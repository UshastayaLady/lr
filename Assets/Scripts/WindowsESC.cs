using UnityEngine;

public class WindowsESC : MonoBehaviour
{
    [SerializeField] private MovePlayer _plaer;
    [SerializeField] private InterfaceController _windowECS;
    private bool _openWindows;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            WorkESC();
        }
    }


    private void WorkESC()
    {
        _openWindows = !_openWindows;
        _plaer.enabled = !_plaer.enabled;
        _windowECS.gameObject.SetActive(!_windowECS.gameObject.activeSelf);
    }

}
