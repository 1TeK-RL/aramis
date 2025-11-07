using UnityEngine;

public class ButtonWagon : MonoBehaviour
{
    [SerializeField] private WagonCustomize wagon;

    public void ChangeWagon()
    {
        PersonnalisationManager.Instance.ChangeWagon(wagon);
    }
}
