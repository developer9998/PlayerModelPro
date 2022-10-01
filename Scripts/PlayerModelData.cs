using System;
using System.Collections.Generic;
using System.Text;

namespace PlayerModelPlus.Scripts
{
    [Serializable]
    public class PlayerModelData
    {
        public string name { get; set; }
        public string author { get; set; }
        public string file_name { get; set; }
        public string download_url { get; set; }
        public bool detail_colour { get; set; }
        public bool detail_gamemode { get; set; }
    }
}
