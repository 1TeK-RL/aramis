using UnityEngine;

public class PersonnalisationManager : MonoBehaviour
{
    [Header("Référence du wagon à personnaliser")]
    public Renderer wagonRenderer; // le MeshRenderer ou SkinnedMeshRenderer du wagon

    [Header("Index du matériau à changer")]
    public int materialIndex = 0; // si ton wagon a plusieurs matériaux, choisis lequel modifier

    // Fonction appelée par les boutons

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
}
