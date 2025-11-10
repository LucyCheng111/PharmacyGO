using UnityEngine;
using UnityEditor;
using System.IO;

public class MedicationExternalCSVImporter : EditorWindow
{
    private string baseOutputFolder = "Assets/Resources/MedicationData";  // Where to save .asset files
    private string medImageFolder = "Assets/Art/MedIndexImage";  // If you still want to load images from inside the project

    [MenuItem("Tools/Medication CSV Importer (External)")]
    public static void ShowWindow()
    {
        GetWindow<MedicationExternalCSVImporter>("Medication CSV Importer (External)");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import CSV from Outside Unity", EditorStyles.boldLabel);

        baseOutputFolder = EditorGUILayout.TextField("Output Folder:", baseOutputFolder);
        medImageFolder = EditorGUILayout.TextField("Medication Image Folder:", medImageFolder);

        if (GUILayout.Button("Select and Import CSV"))
        {
            // Opens a native file panel
            string filePath = EditorUtility.OpenFilePanel(
                "Select Medication CSV",
                "",   // Default directory to open
                "csv" // Extension filter
            );

            if (!string.IsNullOrEmpty(filePath))
            {
                ImportMedicationsFromCSV(filePath, baseOutputFolder, medImageFolder);
            }
        }
    }

    private void ImportMedicationsFromCSV(string csvPath, string folder, string imageFolder)
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogError($"CSV file not found at path: {csvPath}");
            return;
        }

        // Optionally ensure your output folder exists
        EnsureFolderExists(folder);

        // Read all lines from the CSV file
        string[] lines = File.ReadAllLines(csvPath);

        // Skip the header (assuming row 0 is header)
        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');
            if (columns.Length < 9)
            {
                Debug.LogWarning($"Line {i+1} has an unexpected number of columns ({columns.Length}). Skipping.");
                continue;
            }

            // Example column mapping
            string medicationName         = columns[0].Trim();
            string level                  = columns[1].Trim();
            string treatmentOrgan         = columns[2].Trim();
            string pharmacologicClass     = columns[3].Trim();
            string mechanismOfAction      = columns[4].Trim();
            string therapeuticUses        = columns[5].Trim();
            string sideEffects            = columns[6].Trim();
            string majorDrugInteractions  = columns[7].Trim();
            string funFact                = columns[8].Trim();

            // Create the Medication ScriptableObject
            Medication newMed = ScriptableObject.CreateInstance<Medication>();
            newMed.medicationName        = medicationName;
            newMed.level                 = level;
            newMed.treatmentOrgan        = treatmentOrgan;
            newMed.pharmacologicClass    = pharmacologicClass;
            newMed.mechanismOfAction     = mechanismOfAction;
            newMed.therapeuticUses       = therapeuticUses;
            newMed.sideEffects           = sideEffects;
            newMed.majorDrugInteractions = majorDrugInteractions;
            newMed.funFact               = funFact;

            // If you want to assign images from inside your project by name:
            newMed.image = LoadSpriteByName(medicationName, imageFolder);

            // Build the final folder path for this level (e.g. "Assets/Resources/MedicationData/Level1")
            // Attempt to parse the "level" string to an integer
            if (!int.TryParse(newMed.level, out int levelValue))
            {
                Debug.LogError($"'{newMed.level}' is not a valid numeric level for medication '{medicationName}'");
                continue; // Skip creating this asset
            }

            // Check the range
            if (levelValue < 1 || levelValue > 4)
            {
                Debug.LogError($"Invalid 'level' value (must be 1-4) for medication '{medicationName}': {newMed.level}");
                continue;
            }

            // Construct the final folder path, e.g. "Assets/Resources/MedicationData/Level1"
            string levelFolderPath = Path.Combine(folder, "Level" + levelValue);

            levelFolderPath = levelFolderPath.Replace("\\", "/");

            // Check if the level folder actually exists
            if (!AssetDatabase.IsValidFolder(levelFolderPath))
            {
                Debug.LogError($"The folder '{levelFolderPath}' does not exist. Please create it or check the 'level' value.");
                continue; // Skip creating this asset
            }

            // Construct the final .asset path inside the existing level folder
            string assetPath = Path.Combine(levelFolderPath, $"{medicationName}.asset");
            assetPath = assetPath.Replace("\\", "/");

            // Create the asset
            AssetDatabase.CreateAsset(newMed, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Medication CSV import completed.");
    }

    // Example method to load sprite by medication name from a known folder
    private Sprite LoadSpriteByName(string medName, string folder)
    {
        string[] searchFolders = new string[] { folder };
        string[] guids = AssetDatabase.FindAssets($"{medName} t:Sprite", searchFolders);
        if (guids.Length > 0)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        else
        {
            Debug.LogWarning($"No sprite found named '{medName}' in '{folder}'.");
            return null;
        }
    }

    // Ensures folder structure within Assets
    private void EnsureFolderExists(string folderPath)
    {
        folderPath = folderPath.Replace("\\", "/");
        if (folderPath.EndsWith("/"))
            folderPath = folderPath.Substring(0, folderPath.Length - 1);

        string[] parts = folderPath.Split('/');
        if (parts.Length < 2 || parts[0] != "Assets")
        {
            Debug.LogError($"Invalid folder path: {folderPath}. Must start with 'Assets'.");
            return;
        }

        string current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            string subFolder = parts[i];
            string combined = $"{current}/{subFolder}";
            if (!AssetDatabase.IsValidFolder(combined))
            {
                AssetDatabase.CreateFolder(current, subFolder);
            }
            current = combined;
        }
    }
}
