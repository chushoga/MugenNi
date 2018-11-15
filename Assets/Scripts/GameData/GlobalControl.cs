using System.Collections;
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

    public Text console; // DEBUGGING    
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
    }

    // TEST ADD COINS
    public void AddCoins()
    {
        // add a coin
        LoadedData.coins = LoadedData.coins + 1;

        // output the coin total to the debug console
        console.text = "COINS: " + LoadedData.coins;
    }
    
    private Save CreateFreshSaveJSONGameObject()
    {
        Save save = new Save();

        for (int i = 0; i < 3; i++){

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
            for (int j = 0; j < 8; j++)
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
        // create a save game object
        Save save = CreateFreshSaveJSONGameObject();

        // write to json
        string json = JsonUtility.ToJson(save);

        // save game path
        string path = Application.persistentDataPath + "/saveGame.json";

        // write data to json file
        File.WriteAllText(path, json);

        Debug.Log("Resetting JSON data...");

    }

    // SAVE GAME as JSON
    public void SaveAsJSON()
    {

        string path = Application.persistentDataPath + "/saveGame.json";

        if (File.Exists(path))
        {

            string json = JsonUtility.ToJson(LoadedData);

            File.WriteAllText(path, json);

            Debug.Log("Saveing as Json" + json);

        } else
        {

            Save save = CreateFreshSaveJSONGameObject();
            string json = JsonUtility.ToJson(save);

            File.WriteAllText(path, json);
            Debug.Log("File does not exist. Creating Save file...");

            // TODO: START FROM HERE
            // Add the saved data to current instance of this Game.cs
            // TODO: MAKE this Game.cs persistant
            LoadedData = save;
            print(LoadedData.worldData[0].isLocked);
            //console.text = "WORLD 0 LOCKED: " + LoadedData.worldData[0].isLocked;
            // clear console
            console.text = "";
            int counterA = 0;
            
            foreach (WorldInfo key in LoadedData.worldData)
            {
                int counterB = 0;
                foreach (LevelInfo key2 in LoadedData.worldData[counterA].levelData)
                {
                    // temp if to only show one id
                    //if (counterA == 0) {
                        console.text += "worldId: " + LoadedData.worldData[counterA].worldID + "\n" ;
                        console.text += "WORLD isLocked: " + LoadedData.worldData[counterA].isLocked + "\n"; ;
                        console.text += "levelId: " + LoadedData.worldData[counterA].levelData[counterB].levelID + "\n";
                        console.text += "LEVEL isLocked: " + LoadedData.worldData[counterA].levelData[counterB].isLocked + "\n";
                        console.text += "isCleared: " + LoadedData.worldData[counterA].levelData[counterB].isCleared + "\n";
                        console.text +=  "Best Time: " + LoadedData.worldData[counterA].levelData[counterB].bestTime + "\n";
                        console.text += "stars: " + LoadedData.worldData[counterA].levelData[counterB].stars[0] + "\n";

                        console.text += "\n --------------------------------- \n";
                    //}
                    
                    counterB++;
                }
                //console.text += "WORLD " + counterA + " "+ key +": " + LoadedData.worldData[counterA].levelData[0].isCleared;
                //console.text += " | - "+ key +" - \n";
                counterA++;
            }
        }

        

        //Save saver = JsonUtility.FromJson<Save>(json);

        
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
            
            //coins = save.coins;
            
            //print("JSON Loaded: " + json);

            // load the save data into the master variable
            LoadedData = save; 

            // ---------------------------------------------------------------------
            // THIS IS FOR PREVIEWING ONLY HERE
            /*
            console.text = ""; // clear console
            int counterA = 0; // reset counter

            foreach (WorldInfo key in LoadedData.worldData)
            {
                int counterB = 0;
                foreach (LevelInfo key2 in LoadedData.worldData[counterA].levelData)
                {
                    // temp if to only show one id
                    //if (counterA == 0) {
                    console.text += "worldId: " + LoadedData.worldData[counterA].worldID + "\n";
                    console.text += "WORLD isLocked: " + LoadedData.worldData[counterA].isLocked + "\n"; ;
                    console.text += "levelId: " + LoadedData.worldData[counterA].levelData[counterB].levelID + "\n";
                    console.text += "LEVEL isLocked: " + LoadedData.worldData[counterA].levelData[counterB].isLocked + "\n";
                    console.text += "isCleared: " + LoadedData.worldData[counterA].levelData[counterB].isCleared + "\n";
                    console.text += "Best Time: " + LoadedData.worldData[counterA].levelData[counterB].bestTime + "\n";
                    console.text += "stars: " + LoadedData.worldData[counterA].levelData[counterB].stars[0] + "\n";

                    console.text += "\n --------------------------------- \n";
                    //}

                    counterB++;
                }                

                counterA++;
            }
            */
        // ---------------------------------------------------------------------
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
        /*
       int i = 0;
       for (i = 0; i < 3; i++)
       {   
           save.levelData.Add(new LevelInfo{
               worldID = i,
               levelID = i,
               clear = false,
               finishTimeSeconds = 450,
               starSpecial = currentStars
           });

       }

       save.coins = coins;
       */

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


        console.text = "CONIS: " + LoadedData.coins;

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
            console.text = "COINS: " + LoadedData.coins;

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
