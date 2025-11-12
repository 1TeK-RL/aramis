using UnityEngine;

public class ButtonWagon : MonoBehaviour
{
    [SerializeField] private int WagonMeshIndex;

    public void ChangeWagon()
    {
        CustomizeManager.Instance.ChangeWagon(WagonMeshIndex);
    }
}
