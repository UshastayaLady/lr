using System.Collections.Generic;
using UnityEngine;

public class RaycastSelector : MonoBehaviour
{
    [SerializeField] private float distance = 1.5f;
    [SerializeField] private LayerMask layerMask;

    private GameObject selectedObject;

    private readonly List<IInteractive> interactions =
        new List<IInteractive>();

    public GameObject SelectedObject => selectedObject;

    private void Update()
    {
        GameObject target = null;

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Ray ray = new Ray(
                transform.position,
                transform.forward);

            // Сначала находим ближайшее препятствие, взаимодействие сквозь стены запрещено
            if (Physics.Raycast(
                ray,
                out RaycastHit hit,
                distance,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore))
            {
                bool correctLayer =
                    (layerMask.value &
                    (1 << hit.collider.gameObject.layer)) != 0;

                if (correctLayer)
                {
                    InteractiveObject display = hit.collider.GetComponentInParent<InteractiveObject>();

                    if (display != null)
                        target = display.gameObject;
                }
            }
        }

        if (target != selectedObject)
            ChangeSelection(target);

        foreach (IInteractive interaction in interactions)
            interaction.Interact();
    }

    private void ChangeSelection(GameObject target)
    {
        // Скрываем название предыдущего предмета
        foreach (IInteractive interaction in interactions)
            interaction.Revert();

        interactions.Clear();
        selectedObject = target;

        if (selectedObject == null)
            return;

        // Находим компоненты предмета, реализующие наш интерфейс
        MonoBehaviour[] components = selectedObject.GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            if (component is IInteractive interaction)
                interactions.Add(interaction);
        }
    }

    private void OnDisable()
    {
        ChangeSelection(null);
    }
}