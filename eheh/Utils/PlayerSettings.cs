namespace eheh.Utils
{
    public class PlayerSettings
    {
        private static PlayerSettings m_instance;

        public static PlayerSettings Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new PlayerSettings();
                }
                return m_instance;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string ClanTag
        {
            get;
            set;
        }

        public string SteamId
        {
            get;
            set;
        }

        public string FOV
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string MaxFPS
        {
            get;
            set;
        }

        public string ClanTile
        {
            get;
            set;
        }

        public string ClanEmblem
        {
            get;
            set;
        }

        public float Sensitivity
        {
            get;
            set;
        }

        public int Level
        {
            get;
            set;
        } = 80;


        public int Prestige
        {
            get;
            set;
        } = 20;

    }
}
