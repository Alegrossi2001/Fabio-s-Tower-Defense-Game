namespace Dypsloom.RhythmTimeline.Scoring
{
    using Dypsloom.RhythmTimeline.Core;
    using Dypsloom.RhythmTimeline.Core.Managers;
    using Dypsloom.Shared;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using UnityEngine;

    public class SaveManager : MonoBehaviour
    {
        [Tooltip("The save file name.")]
        [SerializeField] protected string m_SaveFileName = "Save";
        [Tooltip("Load the save file with the save manager.")]
        [SerializeField] protected bool m_LoadOnStart;
        [Tooltip("Save the file whenever the Score Manager updates the high score.")]
        [SerializeField] protected bool m_SaveOnNewHighScore;
        [Tooltip("Save a copy of the save file as a simple readable Json file.")]
        [SerializeField] protected bool m_DebugJsonCopy = false;

        private RhythmDirector m_RhythmDirector;
        protected ScoreManager m_ScoreManager;
        protected RhythmGameManager m_GameManager;
        protected SaveData m_SaveData;

        public SaveData SaveData => m_SaveData;

        public string SaveFileName
        {
            get => m_SaveFileName;
            set => m_SaveFileName = value;
        }

        private void Awake()
        {
            Toolbox.Set(this);
            m_SaveData = new SaveData();
        }

        private void Start()
        {
            m_RhythmDirector = Toolbox.Get<RhythmDirector>();
            m_ScoreManager = Toolbox.Get<ScoreManager>();
            m_GameManager = Toolbox.Get<RhythmGameManager>();

            if (m_SaveOnNewHighScore) {
                m_ScoreManager.OnNewHighScore += HandleOnSongEnd;
            }
            
            if (m_LoadOnStart) {
                Toolbox.Get<SaveManager>()?.LoadSaveData();
            }
        }

        private void HandleOnSongEnd(RhythmTimelineAsset song)
        {
            SaveSong(song);
        }

        public void SaveSong(RhythmTimelineAsset song)
        {
            m_SaveData.SaveSong(song);
            SaveToDiskInternal();
        }

        /// <summary>
        /// Return the save folder path.
        /// </summary>
        [ContextMenu("Print Save Folder Path")]
        public void PrintSaveFolderPath()
        {
            Debug.Log(GetSaveFolderPath());
        }

        /// <summary>
        /// Return the save folder path.
        /// </summary>
        [ContextMenu("Reset All Song High Scores")]
        public void ResetAllSongHighScores()
        {
            if (m_GameManager == null) {
                m_GameManager = GetComponent<RhythmGameManager>();
                if(m_GameManager == null){return;}
            }

            for (int i = 0; i < m_GameManager.Songs.Length; i++) {
                m_GameManager.Songs[i]?.SetHighScore(new ScoreData());
            }
        }

        /// <summary>
        /// Save all song high score to file.
        /// </summary>
        [ContextMenu("Save All Songs To File")]
        public void SaveAllSongsToFile()
        {
            if (m_GameManager == null) {
                m_GameManager = GetComponent<RhythmGameManager>();
                if(m_GameManager == null){return;}
            }

            if (m_SaveData == null) {
                m_SaveData = new SaveData();
            }
            
            m_SaveData.SaveSongs(m_GameManager.Songs);
            SaveToDiskInternal();
        }

        [ContextMenu("Load Save File")]
        public void LoadSaveData()
        {
            if (m_GameManager == null) {
                m_GameManager = GetComponent<RhythmGameManager>();
                if(m_GameManager == null){return;}
            }
            
            //Load
            LoadInternal();
        }

        /// <summary>
        /// Delete the save from disk.
        /// </summary>
        [ContextMenu("Delete Save File")]
        public void DeleteFromDisk()
        {
            var saveFilePath = GetSaveFilePath();
            if (!File.Exists(saveFilePath)) { return; }

            File.Delete(saveFilePath);
        }

        /// <summary>
        /// Load the saved data from the save index provided.
        /// </summary>
        protected virtual void LoadInternal()
        {
            GetSaveFromDisk();

            m_SaveData.LoadSongSave(m_GameManager.Songs);
        }

        /// <summary>
        /// Get the save data from the disk.
        /// </summary>
        protected virtual void GetSaveFromDisk()
        {
            var saveFilePath = GetSaveFilePath();
            if (!File.Exists(saveFilePath)) { return; }

            var saveData = new SaveData();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(saveFilePath, FileMode.Open);
            JsonUtility.FromJsonOverwrite((string)bf.Deserialize(file), saveData);
            file.Close();
            
            m_SaveData = saveData;
        }

        /// <summary>
        /// Return the save folder path.
        /// </summary>
        /// <returns>The save folder path.</returns>
        protected virtual string GetSaveFolderPath()
        {
            return Application.persistentDataPath;
        }
        
        /// <summary>
        /// Get the save file path.
        /// </summary>
        /// <returns>The save file path.</returns>
        protected virtual string GetSaveFilePath()
        {
            return string.Format("{0}/{1}.save",
                GetSaveFolderPath(), m_SaveFileName);
        }
        
        /// <summary>
        /// Save to disk.
        /// </summary>
        protected virtual void SaveToDiskInternal()
        {
            var saveFilePath = GetSaveFilePath();

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(saveFilePath);
            
            var json = JsonUtility.ToJson(m_SaveData);

            //Save binary file
            bf.Serialize(file, json);
            file.Close();

            if (!m_DebugJsonCopy) { return; }

            var jsonFilePath = saveFilePath + ".json";
            Debug.Log("You are making a Debug Json Copy of the save file at the path: " + jsonFilePath);
            CreateDebugSaveFile(jsonFilePath, m_SaveData);
        }
        
        /// <summary>
        /// Create a Text Save File.
        /// </summary>
        /// <param name="filePath">The File Path.</param>
        /// <param name="value">The string to save.</param>
        private void CreateDebugSaveFile(string filePath, SaveData saveData)
        {
            // Delete the file if it exists.
            if (File.Exists(filePath)) { File.Delete(filePath); }

            var standardSaveDataJson = JsonUtility.ToJson(saveData, true);

            //Create the file.
            using (FileStream fs = File.Create(filePath)) {
                //Write the Entire save file
                WriteToFile(fs, standardSaveDataJson);
            }
        }
        
        /// <summary>
        /// Write to a file.
        /// </summary>
        /// <param name="fs">The file stream.</param>
        /// <param name="value">The string to write.</param>
        private static void WriteToFile(FileStream fs, string value)
        {
            var info = new UTF8Encoding(true).GetBytes(value);
            fs.Write(info, 0, info.Length);
        }
    }
    
    [Serializable]
    public class SaveData
    {
        [SerializeField] protected List<SongSaveData> m_SongSaveData = new List<SongSaveData>();

        public List<SongSaveData> SongSaveData => m_SongSaveData;

        public void SaveSongs(RhythmTimelineAsset[] songs)
        {
            ClearSave();
            for (int i = 0; i < songs.Length; i++) {
                var song = songs[i];
                if(song == null){ continue; }
                
                var songSaveData = new SongSaveData(song);
                m_SongSaveData.Add(songSaveData);
            }
        }

        public void ClearSave()
        {
            m_SongSaveData.Clear();
        }

        public void SaveSong(RhythmTimelineAsset song)
        {
            for (int i = 0; i < m_SongSaveData.Count; i++) {
                var save = m_SongSaveData[i];
                if (save.SongName == song.FullName) {
                    save.SaveSong(song);
                    return;
                }
            }

            var songSaveData = new SongSaveData(song);
            m_SongSaveData.Add(songSaveData);
        }

        public void LoadSongSave(RhythmTimelineAsset[] songs)
        {
            for (int i = 0; i < m_SongSaveData.Count; i++) {
                var saveData = m_SongSaveData[i];
                
                for (int j = 0; j < songs.Length; j++) {
                    var song = songs[j];

                    if (saveData.SongName == song.FullName) {
                        saveData.Load(song);
                        break;
                    }
                }
                
            }
        }
    }

    [Serializable]
    public class SongSaveData
    {
        [SerializeField] protected string m_SongName;
        [SerializeField] protected ScoreData m_ScoreData;

        public string SongName => m_SongName;
        public ScoreData ScoreData => m_ScoreData;

        public SongSaveData()
        {
            m_SongName = "";
            m_ScoreData = null;
        }
        
        public SongSaveData(RhythmTimelineAsset song)
        {
            m_SongName = song.FullName;
            m_ScoreData = song.HighScore;
        }
        
        public void SaveSong(RhythmTimelineAsset song)
        {
            m_SongName = song.FullName;
            m_ScoreData = song.HighScore;
        }

        public void Load(RhythmTimelineAsset song)
        {
            song.SetHighScore(m_ScoreData);
        }
    }
}