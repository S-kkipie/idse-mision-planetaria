using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Unity.exe -batchmode -quit -projectPath <proyecto> -executeMethod ConstruirMenu.Ejecutar
// Crea Assets/Scenes/Menu.unity con el menu, la registra y compila Build/MisionPlanetaria.exe
public static class ConstruirMenu
{
    [MenuItem("Mision Planetaria/Crear escena Menu y compilar")]
    public static void Ejecutar()
    {
        var escena = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        var go = new GameObject("Menu");
        go.AddComponent<MenuPrincipal>();
        go.AddComponent<Capturador>();

        System.IO.Directory.CreateDirectory("Assets/Scenes");
        const string ruta = "Assets/Scenes/Menu.unity";
        EditorSceneManager.SaveScene(escena, ruta);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ruta, true) };

        PlayerSettings.productName = "Mision Planetaria";
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1920;
        PlayerSettings.defaultScreenHeight = 1080;
        PlayerSettings.resizableWindow = true;

        var r = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = new[] { ruta },
            locationPathName = "Build/MisionPlanetaria.exe",
            target = BuildTarget.StandaloneWindows64,
        });
        Debug.Log("BUILD_RESULT " + r.summary.result);
        if (Application.isBatchMode) EditorApplication.Exit(r.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? 0 : 1);
    }
}
