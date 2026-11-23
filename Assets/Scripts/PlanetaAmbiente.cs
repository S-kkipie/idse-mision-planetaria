using UnityEngine;

// Skybox y luz propios de cada planeta. Poner en un objeto vacio de la escena.
public class PlanetaAmbiente : MonoBehaviour
{
    public Material skyboxPlaneta;          // ej. Skybox_Mercurio (Sol gigante, cielo negro)
    public Light luzSol;                    // Directional Light (Lab 02)
    public Color colorSol = Color.white;    // Mercurio: blanco intenso
    public float intensidadSol = 2f;

    void Awake()
    {
        if (skyboxPlaneta) RenderSettings.skybox = skyboxPlaneta;
        if (luzSol) { luzSol.color = colorSol; luzSol.intensity = intensidadSol; }
        DynamicGI.UpdateEnvironment();
    }
}
