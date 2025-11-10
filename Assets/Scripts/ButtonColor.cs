using UnityEngine;

public class ButtonColor : MonoBehaviour
{
    [SerializeField] WagonMaterial material;

    public void ApplyColor()
    {
        CustomizeManager.Instance.SetMaterial(material);
    }
}

