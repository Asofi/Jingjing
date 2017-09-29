using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Analytizer : MonoBehaviour {

    public static Analytizer Instance;
    public string OutputFileName = "StatisticLog.txt";
    string path;

    public struct StatData {
        public int Tries;
        public float Value;
        public float sum;

        public StatData(int tries, float value) {
            Tries = tries;
            Value = value;
            sum = 0;
        }

    }

    public Dictionary<string, StatData> StatisticData = new Dictionary<string, StatData>();

    // Use this for initialization
    void Awake() {
        //Instance = Instance ?? this;
        //PlayerPrefs.DeleteKey("stars");
        //EventManager.OnGameStart += EventManager_OnGameStart;
    }

    private void EventManager_OnGameStart() {
        StartCoroutine(StartCollectStats(1));
    }

    private void OnApplicationQuit() {
        WriteData();
    }

    public void AddKey(string name) {
        StatisticData.Add(name, new StatData(0, 0));
    }

    public void InsertValue(string Key, float value) {
        if (!StatisticData.ContainsKey(Key)) {
            AddKey(Key);
        }
        StatData _prevData = StatisticData[Key];
        int _tries = _prevData.Tries + 1;
        float _sum = _prevData.sum + value;
        float _value = _sum / _tries;
        print(value + " ------" + _sum + " ------ " + _value);
        StatData _newData = new StatData(_prevData.Tries + 1, _value);
        _newData.sum = _sum;
        StatisticData[Key] = _newData;
    }

    public void WriteData() {
        path = Application.dataPath + "/Statistics/";
        if (!Directory.Exists(path)) {
            // если нет, то создаем ее.
            Directory.CreateDirectory(path);
        }

        path += OutputFileName;
        print(path);
        if (!File.Exists(path)) {
            // Создание файла и запись в него
            using (StreamWriter sw = File.CreateText(path)) {
                foreach(string Key in StatisticData.Keys)
                    sw.WriteLine(string.Format("{0} : average value is {1} for {2} seconds.", Key, StatisticData[Key].Value, StatisticData[Key].Tries));
            }
        } else {
            using (StreamWriter sw = new StreamWriter(path)) {
                foreach (string Key in StatisticData.Keys)
                    sw.WriteLine(string.Format("{0} : average value is {1} for {2} seconds.", Key, StatisticData[Key].Value, StatisticData[Key].Tries));
                //sw.NewLine = string.Format("{0} : average value is {1} for {2} tries.", Key, StatisticData[Key].Value, StatisticData[Key].Tries);
            }
        }
    }

    IEnumerator StartCollectStats(float timeStep) {
        while (true) {
            InsertValue("Stars", SuperManager.Instance.ShopManager.StarsCount);
            yield return new WaitForSeconds(timeStep);
        }
    }
}
