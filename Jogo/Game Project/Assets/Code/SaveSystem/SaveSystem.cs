using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void Save(EndlessInfo info)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/endless.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        EndlessData data = new EndlessData(info);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void SaveNoFile()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/endless.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        EndlessData data = new EndlessData();

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static EndlessData Load()
    {
        string path = Application.persistentDataPath + "/endless.data";
        Debug.Log(path);

        EndlessData returns;
        bool repeat = false;
        do
        {
            repeat = false;
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                EndlessData data = formatter.Deserialize(stream) as EndlessData;
                stream.Close();
                returns = data;
            }
            else
            {
                Debug.Log("Save file not found in " + path);
                SaveNoFile();
                repeat = true;
                returns = null;
            }
        } while (repeat);

        return returns;
    }
}
