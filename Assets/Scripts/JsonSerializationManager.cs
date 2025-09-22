using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine.Networking;



// 목표
// 1. Serialization(직렬화)를 통해 객체의 정보를 문자로 바꾼다.
// 속성: 객체의 정보(클래스)
// 2. OpenWeather API 사용 예시
// 
[Serializable]
public class Coord
{
    public float lon;
    public float lat;
}

[Serializable]
public class Weather
{
    public int id;
    public string main, description, icon;
}

[Serializable]
public class Main
{
    public float temp, feels_like, temp_min, temp_max;
    public int pressure, humidity, sea_level, grnd_level;
}

[Serializable]
public class Wind
{
    public float speed;
    public int deg;
}

[Serializable]
public class Clouds
{
    public int all;
}

[Serializable]
public class Sys
{
    public int type, id;
    public string country;
    public long sunrise, sunset;
}

[Serializable]
public class WeatherData
{
    public Coord coord;
    public Weather[] weather;
    public string station; // base -> station
    public Main main;
    public int visibility;
    public Wind wind;
    public Clouds clouds;
    public long dt;
    public Sys sys;
    public int timezone;
    public int id;
    public string name;
    public int cod;
}


public class JsonSerializationManager : MonoBehaviour
{
    public WeatherData weatherData;
    public string apiKey = "4a339009a7645f0ed92d1f61cca9f8f5";
    public string lat = "37.48";
    public string lon = "126.78";
    public string baseURL = "https://api.openweathermap.org/data/2.5/weather?";

    // JsonUtility의 한계
    // 1. private 접근지정자의 경우 직렬화 불가능.
    // 2. 프로퍼티 사용 불가
    // 3. 딕셔너리 같은 복잡한 계층구조 자료구조는 사용불가
    [Serializable] // Inspector 창에 보여지게 속성 추가
    public class PlayerData
    {
        string name;
        public string Name { get => name; set => name = value; }
        public int score;
        public int hp;
        public List<string> msgs = new List<string>();
        public Item 물약 = new Item();
        public List<Item> weapons = new List<Item>();
        public Dictionary<int, Item> items = new Dictionary<int, Item>();
    }

    [Serializable]
    public class Item
    {
        public string name;
        public int number;
    }

    [Serializable]
    public class Inventory
    {
        public List<Item> 식료품들 = new List<Item>();
        public List<Item> 갑옷들 = new List<Item>();
    }

    public PlayerData mainPlayer;
    public string jsonStr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    JsonSerialization();

    //    JsonDeSerialization();
    //}

    // 비동기 키워드를 사용하는 Start 메서드
    private async void Start()
    {
        Debug.Log("날씨 정보를 가져오는 중입니다...");

        weatherData = await GetWeatherAsync();

        Debug.Log("성공적으로 날씨 정보를 수신했습니다.");
    }

    private static void JsonDeSerialization()
    {
        // 1. Server or DB로 부터 데이터를 읽음
        string newJson = @"{
            ""name"": ""김동훈"",
            ""score"": 20,
            ""hp"": 50
        }";

        // 2. Json 문자열 -> DeSerialization -> 객체화
        PlayerData newPlayer = JsonUtility.FromJson<PlayerData>(newJson);
        print($"{newPlayer.Name} / {newPlayer.score} / {newPlayer.hp}");
    }

    private void JsonSerialization()
    {
        // 1. Class -> 객체화(instancing)
        mainPlayer = new PlayerData();

        // 2. 객체의 정보 초기화
        mainPlayer.Name = "신태욱";
        mainPlayer.score = 0;
        mainPlayer.hp = 10;
        mainPlayer.msgs.Add("안녕하세요.");
        mainPlayer.msgs.Add("반갑습니다.");
        mainPlayer.물약.name = "hp물약";
        mainPlayer.물약.number = 3;

        Item item = new Item() { name = "단검", number = 3};
        Item item2 = new Item() { name = "장검", number = 3};
        Item item3 = new Item() { name = "양날검", number = 3};

        mainPlayer.weapons.Add(item);
        mainPlayer.weapons.Add(item2);
        mainPlayer.weapons.Add(item3);

        Item item4 = new Item() { name = "갑옷1", number = 3 };
        Item item5 = new Item() { name = "갑옷2", number = 3 };

        mainPlayer.items.Add(0, item4); // items[0] -> 갑옷1
        mainPlayer.items.Add(1, item5);

        // 3. 객체(Object or instance) -> Serialization -> Json
        // jsonStr = JsonUtility.ToJson(mainPlayer);       // JsonUtility 사용
        jsonStr = JsonConvert.SerializeObject(mainPlayer); // Json.Net 사용
    }

    public async Task<WeatherData> GetWeatherAsync()
    {
        string totalURL = $"{baseURL}lat={lat}&lon={lon}&appid={apiKey}";

        // Unity에 웹 요청 클래스 = UnityWebRequest
        using(var webRequest = UnityWebRequest.Get(totalURL))
        {
            // 비동기로 웹 요청 보내기
            var operation = webRequest.SendWebRequest();

            while(!operation.isDone)
            {
                await Task.Yield(); // 다음 프레임까지 대기
            }

            // 네트워크 또는 HTTP 오류 확인
            if(webRequest.result == UnityWebRequest.Result.ConnectionError
                || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
                return null;
            }

            // 성공적으로 데이터 수신시
            string json = webRequest.downloadHandler.text;
            Debug.Log(json);

            json = json.Replace("base", "station");

            return JsonConvert.DeserializeObject<WeatherData>(json);
        }
    }
}
