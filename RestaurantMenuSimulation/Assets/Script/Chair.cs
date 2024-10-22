using UnityEngine;

public class Chair : MonoBehaviour
{
    public bool IsOccupied { get; private set; } = false;

    public void OnCustomerSit()
    {
        IsOccupied = true;
    }

    public void OnCustomerLeave()
    {
        IsOccupied = false;
    }
}
