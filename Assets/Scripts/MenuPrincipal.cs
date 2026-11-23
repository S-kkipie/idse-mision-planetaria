using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Menu principal de MISION PLANETARIA. Construye toda la interfaz por codigo:
// poner este script en un GameObject vacio de la escena "Menu" y darle Play.
// Paneles: Principal, Historia (guion + TIF), Opciones (accesibilidad), Creditos.
// Se navega con mouse o con teclado (flechas + Enter, Esc para volver).
public class MenuPrincipal : MonoBehaviour
{
    [Header("Escena del Nivel 1 (agregarla en File > Build Profiles)")]
    public string escenaNivel1 = "Nivel1_Mercurio";

    [Header("Creditos: nombres completos de los autores (4 a 5)")]
    public string curso = "Ingeniería de Software - Desarrollo de Videojuegos";
    public string[] autores = {
        "Nombre Apellido Apellido 1",
        "Nombre Apellido Apellido 2",
        "Nombre Apellido Apellido 3",
        "Nombre Apellido Apellido 4",
        "Nombre Apellido Apellido 5",
    };
    public string docente = "Docente del curso";
    public string anio = "2026";

    // Paleta (las mismas claves PlayerPrefs que usa Accesibilidad.cs)
    static readonly string[] modosColor = { "Normal", "Protanopia / Deuteranopia", "Tritanopia", "Alto contraste" };
    static readonly Color[] acentos = {
        new Color(1f, 0.55f, 0.15f),   // naranja ODYSSEY
        new Color(0.2f, 0.55f, 1f),    // azul (seguro para rojo-verde)
        new Color(1f, 0.35f, 0.6f),    // rosado (seguro para azul-amarillo)
        Color.yellow,                  // alto contraste
    };

    Font fuente;
    Transform canvas;
    GameObject panelPrincipal, panelHistoria, panelOpciones, panelCreditos;
    GameObject panelActual;
    RectTransform rodilloCreditos;
    readonly System.Collections.Generic.List<Text> textos = new();
    readonly System.Collections.Generic.List<Image> acentuables = new();

    int Paleta { get => PlayerPrefs.GetInt("paleta", 0); set => PlayerPrefs.SetInt("paleta", value); }
    bool TextoGrande { get => PlayerPrefs.GetInt("textoGrande", 0) == 1; set => PlayerPrefs.SetInt("textoGrande", value ? 1 : 0); }
    bool Narracion { get => PlayerPrefs.GetInt("narracion", 1) == 1; set => PlayerPrefs.SetInt("narracion", value ? 1 : 0); }
    bool Lento { get => PlayerPrefs.GetInt("lento", 0) == 1; set => PlayerPrefs.SetInt("lento", value ? 1 : 0); }
    Color Acento => acentos[Paleta];

    void Start()
    {
        Time.timeScale = 1f;
        fuente = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        CrearCanvas();
        CrearFondoEstrellado();
        panelPrincipal = CrearPanelPrincipal();
        panelHistoria = CrearPanelHistoria();
        panelOpciones = CrearPanelOpciones();
        panelCreditos = CrearPanelCreditos();
        Mostrar(panelPrincipal);
        AplicarAccesibilidad();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && panelActual != panelPrincipal) Mostrar(panelPrincipal);

        if (panelActual == panelCreditos && rodilloCreditos)
        {
            rodilloCreditos.anchoredPosition += Vector2.up * 60f * Time.unscaledDeltaTime;
            if (rodilloCreditos.anchoredPosition.y > rodilloCreditos.sizeDelta.y + 700f)
                rodilloCreditos.anchoredPosition = new Vector2(0, -500f);
        }
    }

    // ---------- Paneles ----------

    GameObject CrearPanelPrincipal()
    {
        var p = Panel("Principal");
        Texto(p, "MISIÓN PLANETARIA", 78, new Vector2(0, 300), FontStyle.Bold, true);
        Texto(p, "Exploradores del Sistema Solar", 34, new Vector2(0, 225));
        Texto(p, "Nave de exploración ODYSSEY-01  ·  Año 2150", 22, new Vector2(0, 175)).color = new Color(1, 1, 1, 0.6f);

        float y = 70;
        Boton(p, "INICIAR MISIÓN", y, Jugar);
        Boton(p, "HISTORIA", y -= 85, () => Mostrar(panelHistoria));
        Boton(p, "OPCIONES / ACCESIBILIDAD", y -= 85, () => Mostrar(panelOpciones));
        Boton(p, "CRÉDITOS", y -= 85, () => Mostrar(panelCreditos));
        Boton(p, "SALIR", y -= 85, Salir);

        Texto(p, "Flechas + Enter para navegar  ·  Esc para volver", 20, new Vector2(0, -385)).color = new Color(1, 1, 1, 0.5f);
        return p;
    }

    GameObject CrearPanelHistoria()
    {
        var p = Panel("Historia");
        Texto(p, "LA MISIÓN", 56, new Vector2(0, 400), FontStyle.Bold, true);
        var cuerpo = Texto(p,
            "En el año 2150 la humanidad ya puede viajar entre planetas, pero todavía no conoce bien su propio vecindario. " +
            "La organización espacial terrestre envía a la nave científica ODYSSEY-01 a recorrer el sistema solar.\n\n" +
            "Tú eres el piloto. Visitarás Mercurio, Venus, Marte, Júpiter, Saturno, Urano y Neptuno, esquivarás obstáculos, " +
            "cuidarás el combustible y recogerás una muestra de cada planeta para enviarla a la Tierra.\n\n" +
            "ASTRA, la inteligencia artificial de la nave, te contará lo que la ciencia sabe de cada planeta cuando recojas sus muestras. " +
            "Al final volverás a casa para aterrizar y entregar todo lo aprendido.\n\n" +
            "Esta misión es para todos: el juego incluye paletas para personas con daltonismo, texto grande, narración de ASTRA " +
            "y un modo lento, para que nadie se quede en tierra.",
            28, new Vector2(0, 20), FontStyle.Normal, false, new Vector2(1300, 600));
        cuerpo.alignment = TextAnchor.UpperLeft;
        Boton(p, "VOLVER", -400, () => Mostrar(panelPrincipal));
        return p;
    }

    GameObject CrearPanelOpciones()
    {
        var p = Panel("Opciones");
        Texto(p, "ACCESIBILIDAD", 56, new Vector2(0, 400), FontStyle.Bold, true);
        Texto(p, "Los cambios se guardan y se aplican en todos los niveles.", 24, new Vector2(0, 340)).color = new Color(1, 1, 1, 0.7f);

        var muestra = Texto(p, "", 24, new Vector2(0, -170), FontStyle.Normal, false, new Vector2(1300, 120));
        Button bColor = null, bTexto = null, bVoz = null, bLento = null;
        bColor = Boton(p, "", 200, () => { Paleta = (Paleta + 1) % modosColor.Length; Refrescar(); });
        bTexto = Boton(p, "", 115, () => { TextoGrande = !TextoGrande; Refrescar(); });
        bVoz = Boton(p, "", 30, () => { Narracion = !Narracion; Refrescar(); });
        bLento = Boton(p, "", -55, () => { Lento = !Lento; Refrescar(); });

        Boton(p, "VOLVER", -400, () => Mostrar(panelPrincipal));

        void Refrescar()
        {
            PlayerPrefs.Save();
            Etiqueta(bColor, $"Paleta de color: {modosColor[Paleta]}");
            Etiqueta(bTexto, $"Texto grande: {(TextoGrande ? "Sí" : "No")}");
            Etiqueta(bVoz, $"Narración de ASTRA: {(Narracion ? "Sí" : "No")}");
            Etiqueta(bLento, $"Modo lento (50%): {(Lento ? "Sí" : "No")}");
            muestra.text = Paleta switch
            {
                1 => "Rojos y verdes se cambian por azules y naranjas.",
                2 => "Azules y amarillos se cambian por rosados y celestes.",
                3 => "Colores fuertes y bordes marcados.",
                _ => "Colores originales del juego.",
            } + "\nMuestras, combustible y peligros también se distinguen por su forma.";
            AplicarAccesibilidad();
        }
        Refrescar();
        return p;
    }

    GameObject CrearPanelCreditos()
    {
        var p = Panel("Creditos");
        var mascara = new GameObject("Mascara", typeof(RectTransform), typeof(RectMask2D));
        var rm = (RectTransform)mascara.transform;
        rm.SetParent(p.transform, false);
        rm.anchorMin = rm.anchorMax = new Vector2(0.5f, 0.5f);
        rm.sizeDelta = new Vector2(1400, 760);
        rm.anchoredPosition = new Vector2(0, 40);

        var rodillo = new GameObject("Rodillo", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
        rodilloCreditos = (RectTransform)rodillo.transform;
        rodilloCreditos.SetParent(rm, false);
        rodilloCreditos.anchorMin = rodilloCreditos.anchorMax = new Vector2(0.5f, 1f);
        rodilloCreditos.pivot = new Vector2(0.5f, 1f);
        rodilloCreditos.sizeDelta = new Vector2(1400, 0);
        var vl = rodillo.GetComponent<VerticalLayoutGroup>();
        vl.childAlignment = TextAnchor.UpperCenter; vl.spacing = 14;
        vl.childControlHeight = true; vl.childControlWidth = true; vl.childForceExpandHeight = false;
        rodillo.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        void Linea(string t, int tam, bool acento = false, FontStyle fs = FontStyle.Normal)
        {
            var tx = Texto(rodilloCreditos, t, tam, Vector2.zero, fs, acento);
            tx.gameObject.AddComponent<LayoutElement>().preferredHeight = tam * 1.5f;
        }

        Linea("MISIÓN PLANETARIA", 64, true, FontStyle.Bold);
        Linea("Exploradores del Sistema Solar", 30);
        Linea("", 30);
        Linea("DESARROLLADO POR", 30, true, FontStyle.Bold);
        foreach (var a in autores) Linea(a, 34);
        Linea("", 30);
        Linea("DOCENTE", 30, true, FontStyle.Bold);
        Linea(docente, 30);
        Linea("", 30);
        Linea("CURSO", 30, true, FontStyle.Bold);
        Linea(curso, 28);
        Linea("", 30);
        Linea("PERSONAJES", 30, true, FontStyle.Bold);
        Linea("Piloto de la ODYSSEY-01: tú", 28);
        Linea("ASTRA: inteligencia artificial de la nave", 28);
        Linea("", 30);
        Linea("RESPONSABILIDAD SOCIAL", 30, true, FontStyle.Bold);
        Linea("Inclusión: paletas para daltonismo, texto grande, narración y modo lento", 26);
        Linea("Educación: datos reales de los ocho planetas al recoger muestras", 26);
        Linea("", 30);
        Linea("RECURSOS", 30, true, FontStyle.Bold);
        Linea("Motor: Unity 6", 26);
        Linea("Sonido de propulsión: \"Rocket Boost Engine Loop\" por qubodup (Freesound)", 26);
        Linea("", 30);
        Linea($"Universidad Nacional de San Agustín de Arequipa · {anio}", 26);
        Linea("Gracias por jugar", 34, true, FontStyle.Bold);

        Boton(p, "VOLVER", -400, () => Mostrar(panelPrincipal));
        return p;
    }

    // ---------- Acciones ----------

    // Acceso publico a los paneles: "Principal", "Historia", "Opciones", "Creditos".
    public void MostrarPanel(string nombre)
    {
        var panel = nombre switch
        {
            "Historia" => panelHistoria,
            "Opciones" => panelOpciones,
            "Creditos" => panelCreditos,
            _ => panelPrincipal,
        };
        Mostrar(panel);
    }

    void Mostrar(GameObject panel)
    {
        foreach (var g in new[] { panelPrincipal, panelHistoria, panelOpciones, panelCreditos })
            if (g) g.SetActive(g == panel);
        panelActual = panel;
        if (panel == panelCreditos && rodilloCreditos) rodilloCreditos.anchoredPosition = new Vector2(0, -500f);
        var primero = panel.GetComponentInChildren<Button>();
        if (primero) StartCoroutine(Seleccionar(primero.gameObject));
    }

    IEnumerator Seleccionar(GameObject g)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(g);
    }

    void Jugar()
    {
        // Rubrica: puntaje 100% y combustible 10 al iniciar el Nivel 1.
        PlayerPrefs.SetFloat("puntaje", 100f);
        PlayerPrefs.SetFloat("combustibleAcumulado", 0f);
        PlayerPrefs.Save();
        if (Application.CanStreamedLevelBeLoaded(escenaNivel1)) SceneManager.LoadScene(escenaNivel1);
        else Debug.LogWarning($"Agrega la escena '{escenaNivel1}' en File > Build Profiles > Scene List.");
    }

    void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void AplicarAccesibilidad()
    {
        float escala = TextoGrande ? 1.25f : 1f;
        foreach (var t in textos)
        {
            if (!t) continue;
            var baseTam = t.GetComponent<TamBase>();
            t.fontSize = Mathf.RoundToInt(baseTam.valor * escala);
            if (baseTam.acento) t.color = Acento;
        }
        foreach (var i in acentuables) if (i) i.color = Acento;
    }

    // ---------- Construccion de UI ----------

    void CrearCanvas()
    {
        var go = new GameObject("CanvasMenu", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var cs = go.GetComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 0.5f;
        canvas = go.transform;

        if (!FindAnyObjectByType<EventSystem>())
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    void CrearFondoEstrellado()
    {
        var fondo = NuevaImagen(canvas, "Fondo", new Color(0.02f, 0.02f, 0.06f));
        Estirar(fondo.rectTransform);

        var rnd = new System.Random(2150);
        for (int i = 0; i < 160; i++)
        {
            var e = NuevaImagen(fondo.transform, "Estrella", Color.white);
            float s = (float)rnd.NextDouble() * 3f + 1f;
            e.rectTransform.sizeDelta = new Vector2(s, s);
            e.rectTransform.anchorMin = e.rectTransform.anchorMax =
                new Vector2((float)rnd.NextDouble(), (float)rnd.NextDouble());
            e.gameObject.AddComponent<Parpadeo>().fase = (float)rnd.NextDouble() * 6f;
        }

        // Los ocho destinos de la mision, como una fila de planetas al pie del menu.
        (string, Color, float)[] planetas = {
            ("Mercurio", new Color(0.6f,0.58f,0.55f), 26), ("Venus", new Color(0.9f,0.75f,0.45f), 40),
            ("Tierra", new Color(0.25f,0.5f,0.95f), 42), ("Marte", new Color(0.8f,0.35f,0.2f), 32),
            ("Júpiter", new Color(0.85f,0.65f,0.45f), 90), ("Saturno", new Color(0.9f,0.8f,0.55f), 76),
            ("Urano", new Color(0.55f,0.85f,0.85f), 56), ("Neptuno", new Color(0.25f,0.4f,0.9f), 54),
        };
        var circulo = CirculoSprite();
        float x = -760;
        foreach (var (nombre, color, tam) in planetas)
        {
            var pl = NuevaImagen(fondo.transform, nombre, color);
            pl.sprite = circulo;
            pl.rectTransform.anchorMin = pl.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            pl.rectTransform.sizeDelta = new Vector2(tam, tam) * 0.55f;
            pl.rectTransform.anchoredPosition = new Vector2(x, 38);
            x += 210;
        }
    }

    GameObject Panel(string nombre)
    {
        var go = new GameObject(nombre, typeof(RectTransform));
        var rt = (RectTransform)go.transform;
        rt.SetParent(canvas, false);
        Estirar(rt);
        return go;
    }

    Text Texto(Transform padre, string contenido, int tam, Vector2 pos, FontStyle estilo = FontStyle.Normal,
               bool acento = false, Vector2? caja = null)
    {
        var go = new GameObject("Texto", typeof(RectTransform), typeof(Text), typeof(TamBase));
        var rt = (RectTransform)go.transform;
        rt.SetParent(padre, false);
        rt.sizeDelta = caja ?? new Vector2(1600, tam * 1.6f);
        rt.anchoredPosition = pos;
        var t = go.GetComponent<Text>();
        t.font = fuente; t.text = contenido; t.fontSize = tam; t.fontStyle = estilo;
        t.alignment = TextAnchor.MiddleCenter; t.color = acento ? Acento : Color.white;
        t.horizontalOverflow = HorizontalWrapMode.Wrap; t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        var tb = go.GetComponent<TamBase>(); tb.valor = tam; tb.acento = acento;
        textos.Add(t);
        return t;
    }

    Text Texto(GameObject padre, string c, int tam, Vector2 pos, FontStyle e = FontStyle.Normal,
               bool acento = false, Vector2? caja = null) => Texto(padre.transform, c, tam, pos, e, acento, caja);

    Button Boton(GameObject padre, string etiqueta, float y, UnityEngine.Events.UnityAction accion)
    {
        var img = NuevaImagen(padre.transform, "Boton", Acento);
        acentuables.Add(img);
        img.rectTransform.sizeDelta = new Vector2(620, 68);
        img.rectTransform.anchoredPosition = new Vector2(0, y);
        var b = img.gameObject.AddComponent<Button>();
        var colores = b.colors;
        colores.normalColor = new Color(1, 1, 1, 0.35f);
        colores.highlightedColor = new Color(1, 1, 1, 0.75f);
        colores.selectedColor = new Color(1, 1, 1, 0.9f);
        colores.pressedColor = Color.white;
        b.colors = colores;
        b.onClick.AddListener(accion);

        var t = Texto(img.transform, etiqueta, 30, Vector2.zero, FontStyle.Bold, false, new Vector2(600, 68));
        t.name = "Etiqueta";
        var borde = t.gameObject.AddComponent<Outline>();
        borde.effectColor = new Color(0, 0, 0, 0.85f);
        borde.effectDistance = new Vector2(2, -2);
        return b;
    }

    static void Etiqueta(Button b, string texto) => b.transform.Find("Etiqueta").GetComponent<Text>().text = texto;

    static Image NuevaImagen(Transform padre, string nombre, Color color)
    {
        var go = new GameObject(nombre, typeof(RectTransform), typeof(Image));
        go.transform.SetParent(padre, false);
        var img = go.GetComponent<Image>();
        img.color = color;
        img.raycastTarget = nombre == "Boton";
        return img;
    }

    static void Estirar(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static Sprite CirculoSprite()
    {
        const int n = 128;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        float r = n / 2f;
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float d = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), new Vector2(r, r));
                tex.SetPixel(x, y, new Color(1, 1, 1, Mathf.Clamp01(r - d)));
            }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, n, n), new Vector2(0.5f, 0.5f));
    }
}
