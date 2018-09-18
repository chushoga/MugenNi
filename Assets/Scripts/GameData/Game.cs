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
    public int finishTimeSeconds;
    public bool clear;
    public int[] currentStars;

    public void Start()
    {
        currentStars = new int[3] { 0, 0, 0 };
    }

    // TEST ADD COINS
    public void AddCoins()
    {
        coins = coins + 1;
        console.text = "COINS: " + coins;
    }
    
    // Create a save game object
    private Save CreateSaveGameObject()
    {
        Save save = new Save();

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


        return save;
    }

    private Save CreateJSONGameObjectFullSave()
    {
        Save save = new Save();

        for (int i = 0; i < 8; i++){
            for (int j = 0; j < 8; j++){
                save.levelData.Add(new LevelInfo
                {
                        worldID = i,
                        levelID = j,                        
                        clear = false,
                        finishTimeSeconds = 450,
                        starSpecial = new int[3] { 0, 0, 0 }
                    }
                );
            }
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
        }

    }

    public void SaveAsJSON()
    {

        string path = Application.persistentDataPath + "/saveGame.json";

        if (File.Exists(path))
        {

            Save save = CreateSaveGameObject();
            string json = JsonUtility.ToJson(save);

            File.WriteAllText(path, json);

            Debug.Log("Saveing as Json" + json);

        } else
        {

            Save save = CreateJSONGameObjectFullSave();
            string json = JsonUtility.ToJson(save);

            File.WriteAllText(path, json);
            Debug.Log("File does not exist. Creating Save file...");
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
        }
        else
        {
            print("No gave saved-----!");
        }
    }

}
