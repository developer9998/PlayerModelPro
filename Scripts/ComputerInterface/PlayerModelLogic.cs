using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace PlayerModelPro.Scripts.ComputerInterface
{
    public class PlayerModelLogic
    {
        public static int index;
        public static List<PlayerModelData> playerModelData;

        public static void GetPlayerModelData()
        {
            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable)
                return;

            WebClient wc = new WebClient();
            string json = wc.DownloadString("https://raw.githubusercontent.com/developer9998/PlayerModelDefaultPlayerModels/main/playerModelOnline.json");

            var players = JsonConvert.DeserializeObject<List<PlayerModelData>>(json);
            playerModelData = players;
        }
    }
}
