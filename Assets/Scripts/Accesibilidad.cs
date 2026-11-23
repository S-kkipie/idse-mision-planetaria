using UnityEngine;

// HUD + opciones de accesibilidad:
//  F1 = modo daltonismo (Normal / Protanopia-Deuteranopia / Tritanopia / Alto contraste)
//  F2 = texto grande
//  F3 = juego lento (50%)
public class Accesibilidad : MonoBehaviour
{
    public NaveColisiones nave;
    public NaveControl control;
    public Renderer[] muestras, combustibles, obstaculos, plataformas;

    static readonly string[] modos = { "Normal", "Protanopia/Deuteranopia", "Tritanopia", "Alto contraste" };
    // Colores por modo: muestra, combustible, obstaculo, plataforma
    static readonly Color[,] paletas = {
        { new Color(0.2f,0.8f,0.3f), new Color(1f,0.8f,0f),  new Color(0.85f,0.2f,0.2f), Color.gray },
        { new Color(0.1f,0.45f,1f),  new Color(1f,0.85f,0.2f), new Color(1f,0.5f,0f),     Color.gray },
        { new Color(0f,0.8f,0.8f),   new Color(1f,0.4f,0.7f),  new Color(0.85f,0.1f,0.1f), Color.gray },
        { Color.white,                Color.yellow,              Color.magenta,              Color.black },
    };

    int modo;
    bool textoGrande, lento;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) { modo = (modo + 1) % modos.Length; AplicarPaleta(); Guardar(); }
        if (Input.GetKeyDown(KeyCode.F2)) { textoGrande = !textoGrande; Guardar(); }
        if (Input.GetKeyDown(KeyCode.F3)) { lento = !lento; Time.timeScale = lento ? 0.5f : 1f; Guardar(); }
    }

    void Start()
    {
        // Lo elegido en el menu de Opciones (MenuPrincipal) se aplica aqui.
        modo = PlayerPrefs.GetInt("paleta", 0) % modos.Length;
        textoGrande = PlayerPrefs.GetInt("textoGrande", 0) == 1;
        lento = PlayerPrefs.GetInt("lento", 0) == 1;
        Time.timeScale = lento ? 0.5f : 1f;
        AplicarPaleta();
    }

    void Guardar()
    {
        PlayerPrefs.SetInt("paleta", modo);
        PlayerPrefs.SetInt("textoGrande", textoGrande ? 1 : 0);
        PlayerPrefs.SetInt("lento", lento ? 1 : 0);
        PlayerPrefs.Save();
    }

    void AplicarPaleta()
    {
        Pintar(muestras, paletas[modo, 0]);
        Pintar(combustibles, paletas[modo, 1]);
        Pintar(obstaculos, paletas[modo, 2]);
        Pintar(plataformas, paletas[modo, 3]);
    }

    static void Pintar(Renderer[] rs, Color c)
    {
        foreach (var r in rs) if (r) r.material.color = c;
    }

    void OnGUI()
    {
        var st = new GUIStyle(GUI.skin.label) { fontSize = textoGrande ? 32 : 18, wordWrap = true };
        st.normal.textColor = Color.white;
        float w = Screen.width - 40;
        GUI.Label(new Rect(20, 20, w, 50), $"Combustible: {control.Combustible:0}%   Muestras: {nave.Muestras}/{nave.muestrasNecesarias}", st);
        GUI.Label(new Rect(20, 70, w, 120), nave.MensajeAstra, st);
        GUI.Label(new Rect(20, Screen.height - 50, w, 40),
            $"F1 Color: {modos[modo]} | F2 Texto grande | F3 Lento: {(lento ? "Sí" : "No")}", st);
    }
}
