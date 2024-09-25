using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class CreateObject : EditorWindow
{
    private HouseDescriptor[] descriptorNotes;
    [MenuItem("Gorilla Homes/Home Exporter")]

    public static void ShowWindow()
    {
        GetWindow(typeof(CreateObject), false, "Home Exporter", false);
    }

    public void OnFocus()
    {
        descriptorNotes = FindObjectsOfType<HouseDescriptor>();
    }

    public Vector2 scrollPosition = Vector2.zero;
    public void OnGUI()
    {
        var window = GetWindow(typeof(CreateObject), false, "Home Exporter", false);

        int ScrollSpace = (16 + 20) + (16 + 17 + 17 + 20 + 20);
        foreach (var note in descriptorNotes)
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

        foreach (HouseDescriptor descriptorNote in descriptorNotes)
        {
            if (descriptorNote != null)
            {
                GUILayout.Label(descriptorNote.gameObject.name, EditorStyles.boldLabel, GUILayout.Height(16));
                descriptorNote.HouseName = EditorGUILayout.TextField("Home Name:", descriptorNote.HouseName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));
                descriptorNote.Author = EditorGUILayout.TextField("Author:", descriptorNote.Author, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(17));

                if (GUILayout.Button("Export " + descriptorNote.HouseName, GUILayout.Width(windowWidthIncludingScrollbar - 40), GUILayout.Height(20)))
                {
                    GameObject noteObject = descriptorNote.gameObject;
                    if (noteObject != null && descriptorNote != null)
                    {
                        if (descriptorNote.HouseName == "" || descriptorNote.Author == "")
                        {
                            EditorUtility.DisplayDialog("Export Failed", "It is required to fill in the Name, Author, and Description for your Home.", "OK");
                            return;
                        }

                        string path = EditorUtility.SaveFilePanel("Where will you build your home?", "", descriptorNote.HouseName + ".home", "home");

                        if (path != "")
                        {
                            Debug.ClearDeveloperConsole();
                            Debug.Log("Exporting Home");
                            EditorUtility.SetDirty(descriptorNote);
                            BuildAssetBundle(descriptorNote.gameObject, path);
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Export Failed", "Please include the path to where the Home will be exported at.", "OK");
                        }
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Export Failed", "The Home object couldn't be found.", "OK");
                    }
                }
                GUILayout.Space(20);
            }
        }
        GUI.EndScrollView();
    }

    static public void BuildAssetBundle(GameObject obj, string path)
    {
        GameObject selectedObject = obj;
        string assetBundleDirectoryTEMP = "Assets/ExportedHouses";

        HouseDescriptor descriptor = selectedObject.GetComponent<HouseDescriptor>();

        if (!AssetDatabase.IsValidFolder("Assets/ExportedHouses"))
        {
            AssetDatabase.CreateFolder("Assets", "ExportedHouses");
        }

        string HouseName = descriptor.HouseName;
        string HouseAuthor = descriptor.Author;
        // string HouseDescription = descriptor.Description;

        string prefabPathTEMP = "Assets/ExportedHouses/House.prefab";

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        PrefabUtility.SaveAsPrefabAsset(selectedObject.gameObject, prefabPathTEMP);
        GameObject contentsRoot = PrefabUtility.LoadPrefabContents(prefabPathTEMP);
        contentsRoot.name = "home.SandParent";

        Text player_info = contentsRoot.AddComponent<Text>();
        string split = "$";
        player_info.text = HouseName + split + HouseAuthor;

        Object.DestroyImmediate(contentsRoot.GetComponent<HouseDescriptor>());

        if (File.Exists(prefabPathTEMP))
        {
            File.Delete(prefabPathTEMP);
        }

        string newprefabPath = "Assets/ExportedHouses/" + contentsRoot.name + ".prefab";
        PrefabUtility.SaveAsPrefabAsset(contentsRoot, newprefabPath);
        PrefabUtility.UnloadPrefabContents(contentsRoot);
        AssetImporter.GetAtPath(newprefabPath).SetAssetBundleNameAndVariant("HouseAssetBundle", "");

        if (!Directory.Exists("Assets/ExportedHouses"))
        {
            Directory.CreateDirectory(assetBundleDirectoryTEMP);
        }

        string asset_new = assetBundleDirectoryTEMP + "/" + HouseName;
        if (File.Exists(asset_new + ".home"))
        {
            File.Delete(asset_new + ".home");
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectoryTEMP, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        if (File.Exists(newprefabPath))
        {
            File.Delete(newprefabPath);
        }

        string asset_temporary = assetBundleDirectoryTEMP + "/HouseAssetBundle";
        string metafile = asset_temporary + ".meta";
        if (File.Exists(asset_temporary))
        {
            File.Move(asset_temporary, asset_new + ".home");
        }

        AssetDatabase.Refresh();
        Debug.ClearDeveloperConsole();

        string path1 = assetBundleDirectoryTEMP + "/" + HouseName + ".home";
        string path2 = path;

        if (!File.Exists(path2)) // add
        {
            File.Move(path1, path2);
        }
        else // replace
        {
            File.Delete(path2);
            File.Move(path1, path2);
        }
        EditorUtility.DisplayDialog("Export Success", $"Your House was exported!", "OK");

        try
        {
            AssetDatabase.RemoveAssetBundleName("Houseassetbundle", true);
        }
        catch
        {

        }

        string HousePath = path + "/";
        EditorUtility.RevealInFinder(HousePath);

        if (AssetDatabase.IsValidFolder("Assets/ExportedHouses"))
        {
            AssetDatabase.DeleteAsset("Assets/ExportedHouses");
        }
    }
}