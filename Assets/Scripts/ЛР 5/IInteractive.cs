using UnityEngine;

public interface IInteractive
{
    public string Name { get; }

    void Interact();
    void Revert();
}
