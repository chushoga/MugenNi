﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class GlobalControl : MonoBehaviour {

    // ------------------------------------------------------------------------
    // make sure that there is only ever one of these gameobjects in existance
    // ------------------------------------------------------------------------
    public static GlobalControl Instance;
    // ------------------------------------------------------------------------
 
    public Save LoadedData; // MASTER SAVE DATA // persistant
    public int currentWorld = 0;
    public int currentLevel = 0;

    private void Awake()
    {
        // ------------------------------------------------------------------------
        // make sure that there is only ever one of these gameobjects in existance
        // ------------------------------------------------------------------------
        if (Instance == null)
        {            
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        // ------------------------------------------------------------------------
        // Load the inital JSON file ** moved from the start function and works well now.
        LoadAsJSON();

    }

    public void Start()
    {
        // run the game as fast as possible.
        Application.targetFrameRate = 300;
    }

    // TEST ADD COINS
    // added via a test button in the scene
    public void AddCoins()
    {
        // add a coin
        LoadedData.coins = LoadedData.coins + 1;

    }
    
    // Create a basic empty save game file.
    private Save CreateFreshSaveJSONGameObject()
    {        
        int maxWorlds = 3; // max number of worlds
        int maxLevels = 8; // max number of levels per world

        Save save = new Save(); // game save

        for (int i = 0; i < maxWorlds; i++){

            // ------------------------------
            // Set the first world as unlocked
            // ------------------------------
            bool isWorldLocked = true;

            if (i == 0)
            {
                isWorldLocked = false;
            }
            // ------------------------------

            // create a new list for the level info
            List<LevelInfo> lister = new List<LevelInfo>();

            // create data for 8 levels per world            
            for (int j = 0; j < maxLevels; j++)
            {
                // ------------------------------
                // Set the first Level to unlocked
                // ------------------------------
                bool isLevelLocked = true;

                if (j == 0)
                {
                    isLevelLocked = false;
                }
                // ------------------------------

                // set the default info per level
                LevelInfo li = new LevelInfo
                {
                    levelID = j,
                    isCleared = false,
                    isLocked = isLevelLocked,
                    bestTime = 0,
                    stars = new List<int>() { 0, 0, 0 }
                };

                // add the info to the level info list
                lister.Add(li);
            }

            // add the world and level info to the save data
            save.worldData.Add(new WorldInfo
            {
                worldID = i,
                isLocked = isWorldLocked,
                levelData = lister
            });
            
        }

        // return the clean save game with the data set to their defaults
        return save;
    }
    
    // RESET all JSON data back to default
    public void ResetGameDataJSON()
    {
        print("Resetting JSON data...");
        // create a save game object
        Save save = CreateFreshSaveJSONGameObject();

        // write to json
        string json = JsonUtility.ToJson(save);

        // save game path
        string path = Application.persistentDataPath + "/saveGame.json";

        // write data to json file
        File.WriteAllText(path, json);

        //print("Resetting JSON data...");

        LoadAsJSON();

    }

    // SAVE GAME as JSON
    public void SaveAsJSON()
    {

        string path = Application.persistentDataPath + "/saveGame.json";

        if (File.Exists(path))
        {

            string json = JsonUtility.ToJson(LoadedData);

            File.WriteAllText(path, json);

            //Debug.Log("Saveing as Json" + json);

        } else
        {

            Save save = CreateFreshSaveJSONGameObject();
            string json = JsonUtility.ToJson(save);

            File.WriteAllText(path, json);
            //Debug.Log("File does not exist. Creating Save file...");

            LoadedData = save;
           
        }

        
    }
    
    // LOAD GAME as JSON
    public void LoadAsJSON()
    {
        // checks if there is a save game and if there is then it will open and set the variables in the save file.
        if (File.Exists(Application.persistentDataPath + "/saveGame.json"))
        {
            // set file path            
            string path = Application.persistentDataPath + "/saveGame.json";

            // read the json file
            string json = File.ReadAllText(path);
            
            // create a save from the json data
            Save save = (Save)JsonUtility.FromJson(json, typeof(Save));
            
            // load the save data into the master variable
            LoadedData = save; 

        }
        else
        {
            
            // create a new game if there is no save game present.
            Save save = CreateFreshSaveJSONGameObject();

            // write the save data to json
            string json = JsonUtility.ToJson(save);

            // set the file path
            string path = Application.persistentDataPath + "/saveGame.json";

            // write the data to json
            File.WriteAllText(path, json);

            // set the newly created data to the current loaded master data
            LoadedData = save;

            Debug.Log("File does not exist. Creating Save file...");
            
        }
    }

    /*
     * * *******************************************************************
     * FOR REFERENCE IF WANT TO USE ENCRYPTED SAVE
     * IN THE FUTURE.
     * * *******************************************************************
     */

    // CREATE A SAVE GAME OBJECT
    private Save CreateSaveGameObject()
    {

        Save save = new Save();
    
        return save;

    }

    // CREATE ENCRYPYTED SAVE GAME
    public void SaveGame()
    {
        // 1 Create a Save instance with all the data for the current session saved into it.
        Save save = CreateSaveGameObject();

        // 2 Create a BinaryFormatter and a FileStream by passing a path for the Save instnce
        // to be saved to. It serializes the data into bytes and writes it to disk and closes
        // the FileStream. ** can use any extention that you want, does not need to be .save
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        print("gameSaved");


    }

    // LOAD ENCRYPTED SAVE GAME
    public void LoadGame()
    {
        // 1
        // checks if there is a save game and if there is then it will open and set the variables in the save file.
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // 4
            LoadedData.coins = save.coins;

            print("Loaded");
        }
        else
        {
            print("No gave saved-----!");
            // create a new game if there is no save game present.
        }

    }


    /*
     * *******************************************************************
     */
}
