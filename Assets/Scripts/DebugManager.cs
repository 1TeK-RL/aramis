using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }
    [SerializeField] private List<TextAsset> wagonCsvFiles = new();
    [SerializeField] public bool isDebugModeActive = false;
    [SerializeField] public List<Station> stations = new();
    [SerializeField] public List<Material> availableMaterials = new();

    [Header("Debug UI")]
    [SerializeField] public GameObject debugCanvas;
    [SerializeField] public TMP_InputField InputField;
    private bool IsDebugCanvasActive = false;
    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        debugCanvas.SetActive(false);
       
    }
    public void DebugButton()
    {
        if (IsDebugCanvasActive == true)
        {
            IsDebugCanvasActive = false;
            debugCanvas.SetActive(false);
            return;
        }
        else
        {
            IsDebugCanvasActive = true;
            debugCanvas.SetActive(true);
        }
    }

    public void LoadButton()
    {
        if(InputField.text == "1212")
        {
            LoadDebugmode();
        }
    }
    public void CancelButton()
    {
        IsDebugCanvasActive = false;
        InputField.text = "";
        debugCanvas.SetActive(false);
    }

    public void LoadDebugmode()
    {
        GhostManager.Instance.DestroyAllGhost();
        GhostManager.Instance.SetMaxGhosts(10);
        foreach (var csv in wagonCsvFiles)
        {
            WagonData data = LoadFromCSV(csv.text);
            GhostManager.Instance.CreateGhost(data);
        }
        InputField.text = "Loaded";
    }
    public void SaveToCSV(WagonData data, string fileName)
    {
        if (data == null || data.paths == null || data.paths.Count == 0)
            return;

        string folderPath = Path.Combine(Application.persistentDataPath, "WagonRecords");
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = Path.Combine(folderPath, fileName + ".csv");

        StringBuilder sb = new StringBuilder();

        // =========================
        // WAGON METADATA
        // =========================
        sb.AppendLine("#WAGON_DATA");
        sb.AppendLine($"wagonName,{data.wagonName}");
        sb.AppendLine($"wagonMeshIndex,{data.wagonMeshIndex}");

        // Materials
        sb.AppendLine($"primaryMaterial,{GetMaterialName(data.wagonMaterial?.PrimaryMaterial)}");
        sb.AppendLine($"secondaryMaterial,{GetMaterialName(data.wagonMaterial?.SecondaryMaterial)}");

        // Station
        if (data.startingStation != null)
        {
            sb.AppendLine($"stationId,{data.startingStation.GetStationID()}");
            sb.AppendLine($"stationSplineId,{data.startingStation.GetSplineID()}");
        }
        else
        {
            sb.AppendLine("stationId,-1");
            sb.AppendLine("stationSplineId,-1");
        }

        sb.AppendLine();

        // =========================
        // PATH DATA
        // =========================
        sb.AppendLine("#PATH_DATA");
        sb.AppendLine("time,posX,posY,posZ,rotX,rotY,rotZ,rotW");

        foreach (var p in data.paths)
        {
            sb.AppendLine(
                $"{p.elapsedTime.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.position.x.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.position.y.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.position.z.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.rotation.x.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.rotation.y.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.rotation.z.ToString(CultureInfo.InvariantCulture)}," +
                $"{p.rotation.w.ToString(CultureInfo.InvariantCulture)}"
            );
        }

        File.WriteAllText(filePath, sb.ToString());
        Debug.Log($"WagonData sauvegardé en CSV : {filePath}");
    }

    private string GetMaterialName(Material mat)
    {
        return mat != null ? mat.name : "NULL";
    }

    public WagonData LoadFromCSV(string csvText)
    {
        WagonData data = new WagonData();
        data.paths = new List<WagonData.PathPoint>();
        data.wagonMaterial = new WagonMaterial();

        string[] lines = csvText.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        bool readingPath = false;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();

            if (line.StartsWith("#"))
            {
                readingPath = line == "#PATH_DATA";
                continue;
            }

            string[] tokens = line.Split(',');

            if (!readingPath)
            {
                // ======================
                // METADATA
                // ======================
                switch (tokens[0])
                {
                    case "wagonName":
                        data.wagonName = tokens[1];
                        break;

                    case "wagonMeshIndex":
                        data.wagonMeshIndex = int.Parse(tokens[1]);
                        break;

                    case "primaryMaterial":
                        if (!string.IsNullOrEmpty(tokens[1]) && tokens[1] != "NULL")
                            data.wagonMaterial.PrimaryMaterial = FindMaterial(tokens[1]);
                        break;

                    case "secondaryMaterial":
                        if (!string.IsNullOrEmpty(tokens[1]) && tokens[1] != "NULL")
                            data.wagonMaterial.SecondaryMaterial = FindMaterial(tokens[1]);
                        break;

                    case "stationId":
                        int stationId = int.Parse(tokens[1]);
                        if (stationId != -1)
                            data.startingStation = FindStationByID(stationId);
                            GameManager.Instance.AddStartingStation(data.startingStation);
                        break;
                }
            }
            else
            {
                // ======================
                // PATH DATA
                // ======================
                if (tokens[0] == "time") // header
                    continue;

                data.paths.Add(new WagonData.PathPoint
                {
                    elapsedTime = ParseFloat(tokens[0]),
                    position = new Vector3(
                        ParseFloat(tokens[1]),
                        ParseFloat(tokens[2]),
                        ParseFloat(tokens[3])
                    ),
                    rotation = new Quaternion(
                        ParseFloat(tokens[4]),
                        ParseFloat(tokens[5]),
                        ParseFloat(tokens[6]),
                        ParseFloat(tokens[7])
                    )
                });
            }
        }
        Debug.Log($"WagonData chargé depuis CSV : {data.wagonName}, {data.paths.Count} points de chemin");
        return data;
    }

    private float ParseFloat(string value)
    {
        return float.Parse(value, CultureInfo.InvariantCulture);
    }

    private Station FindStationByID(int id)
    {
        
        foreach (Station station in stations)
        {
            if (station.GetStationID() == id)
                return station;
        }
        return null;
    }

    private Material FindMaterial(string materialName)
    {
        foreach (Material mat in availableMaterials)
        {
            if (mat.name == materialName)
                return mat;
        }
        return null;
    }
}
