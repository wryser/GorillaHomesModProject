using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class CreateObject : EditorWindow
{

    static private string prefabPath;

    static public string filePath;

    private HouseDescriptor[] notes;
    [MenuItem("Window/Exporter")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CreateObject), false, "Exporter", false);
    }
    public Vector2 scrollPosition = Vector2.zero;

    private void OnFocus()
    {
        notes = FindObjectsOfType<HouseDescriptor>();
    }
    void OnGUI()
    {
        var window = GetWindow(typeof(CreateObject), false, "Exporter", false);

        int ScrollSpace = (16 + 20) + (16 + 17 + 17 + 20 + 20);
        foreach (HouseDescriptor note in notes)
        {
            if (note != null)
            {

                ScrollSpace += (16 + 17 + 17 + 20 + 20);

            }
        }
        float currentWindowWidth = EditorGUIUtility.currentViewWidth;
        float windowWidthIncludingScrollbar = currentWindowWidth;
        if (window.position.size.y >= ScrollSpace)
        {
            windowWidthIncludingScrollbar += 30;
        }
        scrollPosition = GUI.BeginScrollView(new Rect(0, 0, EditorGUIUtility.currentViewWidth, window.position.size.y), scrollPosition, new Rect(0, 0, EditorGUIUtility.currentViewWidth - 20, ScrollSpace), false, false);

        GUILayout.Label("Notes", EditorStyles.boldLabel, GUILayout.Height(16));
        GUILayout.Space(20);

        foreach (HouseDescriptor note in notes)
        {
            if (note != null)
            {
                GUILayout.Label("GameObject : " + note.gameObject.name, EditorStyles.boldLabel, GUILayout.Height(16));
                note.Author = EditorGUILayout.TextField("Object author", note.Author, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                note.HouseName = EditorGUILayout.TextField("Object name", note.HouseName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));

                if (GUILayout.Button("Export " + note.HouseName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(20)))
                {
                    GameObject noteObject = note.gameObject;
                    if (noteObject != null && note != null)
                    {
                        EditorUtility.SetDirty(note);

                        foreach (Collider collider in noteObject.GetComponentsInChildren<Collider>())
                        {
                            collider.gameObject.layer = 0;
                        }
                        noteObject.GetComponent<HouseDescriptor>().enabled = false;
                        BuildAssetBundle(note.gameObject);
                        EditorUtility.DisplayDialog("Home Exported", "Exportation Successful!", "YIPPEE");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Exportation Failed!", "GameObject is missing.", "OK");
                    }
                }
                GUILayout.Space(20);
            }
        }
        GUI.EndScrollView();
    }


    static public void BuildAssetBundle(GameObject obj)
    {
        string SandObjectName = obj.GetComponent<HouseDescriptor>().HouseName;
        string SandObjectAuthor = obj.GetComponent<HouseDescriptor>().Author;

        if (!AssetDatabase.IsValidFolder("Assets/HomeOutput"))
        {
            AssetDatabase.CreateFolder("Assets", "HomeOutput");
        }

        if (SandObjectName == null)
        {
            SandObjectName = obj.name;
        }

        prefabPath = "Assets/HomeOutput/" + SandObjectName + ".prefab";
        filePath = "Assets/HomeOutput";

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();



        var prefabAsset = PrefabUtility.SaveAsPrefabAsset(obj.gameObject, prefabPath);

        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPath);


        contentsRoot.name = "home.SandParent";

        string newprefabPath = "Assets/HomeOutput/" + contentsRoot.name + ".prefab";
        Text player_info = contentsRoot.AddComponent<Text>();
        string split = "$";
        player_info.text = SandObjectName + split + SandObjectAuthor;

        Object.DestroyImmediate(contentsRoot.GetComponent<HouseDescriptor>());

        PrefabUtility.SaveAsPrefabAsset(contentsRoot, newprefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);

        if (File.Exists(prefabPath))
        {
            File.Delete(prefabPath);
        }

        AssetImporter.GetAtPath(newprefabPath).SetAssetBundleNameAndVariant("home.assetbundle", "");



        string assetBundleDirectory = "Assets/HomeOutput";

        if (!Directory.Exists("Assets/HomeOutput"))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        string asset_new = filePath + "/" + SandObjectName;

        string asset_temp = filePath + "/home.assetbundle";

        if (File.Exists(asset_new + ".home"))
        {

            File.Delete(asset_new + ".home");
        }


        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        if (File.Exists(newprefabPath))
        {
            File.Delete(newprefabPath);
        }

        string asset_manifest = assetBundleDirectory + "/playermodel.assetbundle.manifest";
        Debug.Log(asset_manifest);
        if (File.Exists(asset_manifest))
        {
            File.Delete(asset_manifest);
        }

        string folder_manifest = assetBundleDirectory + "/PlayerModelOutput";
        //Debug.Log(folder_manifest);
        if (File.Exists(folder_manifest))
        {
            File.Delete(folder_manifest);

            File.Delete(folder_manifest + ".manifest");
        }



        string metafile = asset_temp + ".meta";
        if (File.Exists(asset_temp))
        {

            Debug.Log("Created " + SandObjectName);
            File.Move(asset_temp, asset_new + ".home");
            Debug.Log(metafile);
        }
        AssetDatabase.Refresh();
        Debug.ClearDeveloperConsole();
    }
}