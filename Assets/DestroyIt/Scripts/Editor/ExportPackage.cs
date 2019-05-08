using UnityEditor;

public class ExportPackage
{
    [MenuItem ("Window/DestroyIt/Export Package + Project Settings")]
    public static void Export()
    {
        AssetDatabase.ExportPackage (AssetDatabase.GetAllAssetPaths(),PlayerSettings.productName + ".unitypackage",ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies | ExportPackageOptions.IncludeLibraryAssets);
    }
}