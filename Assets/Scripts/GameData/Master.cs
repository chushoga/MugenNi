using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class Master : MonoBehaviour {

    // Include on all scenes.
    // Persistant game information loaded from the save file.

    // Coin count
    public static int COIN_COUNT = 0;
    public static int TOTAL_JUMPS = 0;
    public static string[,] LEVEL_PROGRESS = new string[8,8];
    public static List<string> ITEMS = new List<string>();

    // TESTING:
    public Text coinText;
        
	// Use this for initialization
	void Start () {
		
	}

    public void UpdateCoinCount()
    {
        LEVEL_PROGRESS[0,1] = "apple";
        LEVEL_PROGRESS[1,1] = "Tester";
        LEVEL_PROGRESS[2,1] = "Tester2";

        print("update coin count" + COIN_COUNT);
        coinText.text = COIN_COUNT + "`s \n";
        for(int i = 0; i < LEVEL_PROGRESS.Length; i++) {
                coinText.text += "WORLD " + LEVEL_PROGRESS[i, 0] + " \n";
                coinText.text += "STARS " + LEVEL_PROGRESS[i, 1] + " \n";
        }

    }

    // Save the game    
    private Save CreateSaveGameObject()
    {
        Save save = new Save();
        /*
        int i = 0;
        foreach(GameObject targetGameObject in targets)
        {
            TargetJoint2D target = targetGameObject.GetComponent<Target>();
            if(target.activeRobot != null)
            {
                save.livingTargetPositions.Add(target.position);
                save.livingTargetTypes.Add((int)target.activeRobot.GetComponent<Robot>().type);
                i++;
            }
        }
        */
        save.coins = COIN_COUNT;
        return save;
    }

    public void SaveGame()
    {

        Save save = CreateSaveGameObject();

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gamesave.save");
        bf.Serialize(file, save);
        file.Close();

        print("SaveGame");
        COIN_COUNT++;
    }

    public void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/gamesave.save"))
        {

            //1

            //2 
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gamesave.save", FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();

            //3 

            //4

            COIN_COUNT = save.coins;

            UpdateCoinCount();
            print("GameLoaded");
        }
        else
        {
            print("No game saved");
        }
    }
}
