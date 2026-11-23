using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// Solo se activa si el juego se abre con:  MisionPlanetaria.exe -capturas <carpeta>
// Recorre los paneles del menu, guarda un PNG de cada uno y cierra el juego.
public class Capturador : MonoBehaviour
{
    string carpeta;

    void Awake()
    {
        var args = System.Environment.GetCommandLineArgs();
        int i = System.Array.IndexOf(args, "-capturas");
        if (i < 0 || i + 1 >= args.Length) { enabled = false; return; }
        carpeta = args[i + 1];
        Directory.CreateDirectory(carpeta);
        PlayerPrefs.DeleteAll(); // capturas con la configuracion por defecto
    }

    IEnumerator Start()
    {
        if (!enabled) yield break;
        var menu = FindAnyObjectByType<MenuPrincipal>();
        yield return new WaitForSecondsRealtime(1.5f);

        yield return Foto("01_menu_principal");

        menu.MostrarPanel("Historia");
        yield return Foto("02_historia");

        menu.MostrarPanel("Opciones");
        yield return Foto("03_opciones");

        // Mismo panel con paleta Alto contraste + texto grande (boton 1 x3, boton 2 x1)
        var botones = GameObject.Find("Opciones").GetComponentsInChildren<Button>();
        for (int k = 0; k < 3; k++) botones[0].onClick.Invoke();
        botones[1].onClick.Invoke();
        yield return Foto("04_opciones_alto_contraste_texto_grande");

        // Volver a la configuracion normal antes de los creditos
        botones[0].onClick.Invoke();
        botones[1].onClick.Invoke();
        menu.MostrarPanel("Creditos");
        yield return new WaitForSecondsRealtime(6f);
        yield return Foto("05_creditos");
        yield return new WaitForSecondsRealtime(6f);
        yield return Foto("06_creditos_continuacion");

        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    IEnumerator Foto(string nombre)
    {
        yield return new WaitForSecondsRealtime(0.6f);
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot(Path.Combine(carpeta, nombre + ".png"));
        yield return new WaitForSecondsRealtime(0.4f);
    }
}
