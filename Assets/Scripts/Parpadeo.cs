using UnityEngine;
using UnityEngine.UI;

// Parpadeo suave de las estrellas del fondo.
public class Parpadeo : MonoBehaviour
{
    public float fase;
    Image img;
    void Awake() => img = GetComponent<Image>();
    void Update()
    {
        var c = img.color;
        c.a = 0.35f + 0.65f * Mathf.Abs(Mathf.Sin(Time.unscaledTime * 0.8f + fase));
        img.color = c;
    }
}
