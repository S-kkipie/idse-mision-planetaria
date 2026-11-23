using UnityEngine;
using UnityEngine.SceneManagement;

// Deteccion por Tags (Lab 09) + nuevo Tag "Muestra".
// Crear en Tags & Layers: ColisionSegura, Combustible, Muestra, Aterrizaje.
public class NaveColisiones : MonoBehaviour
{
    public float recargaCombustible = 40f;
    public int muestrasNecesarias = 3;
    [TextArea] public string fichaPlaneta =
        "Mercurio: es el planeta más cercano al Sol y el más pequeño del sistema solar.";

    public int Muestras { get; private set; }
    public string MensajeAstra { get; private set; } = "ASTRA: Recoge las muestras de roca de Mercurio.";

    NaveControl nave;
    bool terminado;

    void Start() { nave = GetComponent<NaveControl>(); }

    void OnCollisionEnter(Collision c)
    {
        if (terminado) return;
        switch (c.gameObject.tag)
        {
            case "ColisionSegura":
                break;
            case "Combustible":
                nave.Recargar(recargaCombustible);
                MensajeAstra = "ASTRA: Combustible recargado.";
                Destroy(c.gameObject);
                break;
            case "Muestra":
                Muestras++;
                MensajeAstra = $"ASTRA: Muestra {Muestras}/{muestrasNecesarias}. {fichaPlaneta}";
                Destroy(c.gameObject);
                break;
            case "Aterrizaje":
                if (Muestras >= muestrasNecesarias)
                {
                    terminado = true;
                    MensajeAstra = "ASTRA: ¡Misión cumplida en este planeta! Muestras enviadas a la Tierra.";
                }
                else
                {
                    MensajeAstra = $"ASTRA: Faltan muestras ({Muestras}/{muestrasNecesarias}).";
                }
                break;
            default:
                terminado = true;
                MensajeAstra = "ASTRA: ¡Impacto! Reiniciando el nivel...";
                Invoke(nameof(Reiniciar), 2f);
                break;
        }
    }

    void Reiniciar() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
