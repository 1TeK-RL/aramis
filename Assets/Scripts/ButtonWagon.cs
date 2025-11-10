using UnityEngine;

public class ButtonWagon : MonoBehaviour
{
    [SerializeField] private WagonCustomize wagon;

    public void ChangeWagon()
    {
        CustomizeManager.Instance.ChangeWagon(wagon);
    }
}
