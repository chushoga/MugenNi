using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour {

    public Text console;
    
    public int coins;    
    public int[] currentStars;

    public Save LoadedData;

    public void Start()
    {
        currentStars = new int[3] { 0, 0, 0 };

        LoadAsJSON();
    }

    // TEST ADD COINS
    public void AddCoins()
    {
        coins = coins + 1;
        console.text = "COINS: " + coins;
        LoadedData.coins = coins; // JANKY
    }
    
    // Create a save game object
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

    private Save CreateFreshSaveJSONGameObject()
    {
        Save save = new Save();

        for (int i = 0; i < 3; i++){


            List<LevelInfo> lister = new List<LevelInfo>();
            for (int j = 0; j < 8; j++)
            {
                LevelInfo li = new LevelInfo
                {
                    levelID = j,
                    isCleared = false,
                    isLocked = true,
                    bestTime = 0,
                    stars = new int[3] { 0, 0, 0 }
                };
                lister.Add(li);
            }

            save.worldData.Add(new WorldInfo
            {
                worldID = i,
                isLocked = true,
                levelData = lister
            });
            
        }
        return save;
    }

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


        console.text = "CONIS: " + coins;

    }

    public void LoadGame()
    {
        // 1
        // checks if there is a save game and if there is then it will open and set the variables in the save file.
        if(File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {
            // 2
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            // 4
            coins = save.coins;
            console.text = "COINS: " + coins;

            print("Loaded");
        } else
        {
            print("No gave saved-----!");
            // create a new game if there is no save game present.
        }

    }

    public void SaveAsJSON()
    {

        string path = Application.persistentDataPath + "/saveGame.json";

        if (File.Exists(path))
        {

            //Save save = CreateSaveGameObject();
            
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


    public void LoadAsJSON()
    {
        // checks if there is a save game and if there is then it will open and set the variables in the save file.
        if (File.Exists(Application.persistentDataPath + "/saveGame.json"))
        {
            // 2
            
            string path = Application.persistentDataPath + "/saveGame.json";
            string json = File.ReadAllText(path);
            
            Save save = (Save)JsonUtility.FromJson(json, typeof(Save));
            
            // 4
            coins = save.coins;
            console.text = "COINS: " + coins;

            print("Loaded" + json);

            LoadedData = save; // load the save data into the master data

            // ---------------------------------------------------------------------
            // THIS IS FOR PREVIEWING ONLY HERE
            
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
                //console.text += "WORLD " + counterA + " "+ key +": " + LoadedData.worldData[counterA].levelData[0].isCleared;
                //console.text += " | - "+ key +" - \n";
                counterA++;
            }
        // ---------------------------------------------------------------------
        }
        else
        {
            print("No gave saved-----!");
            // create a new game if there is no save game present.
        }
    }

}
