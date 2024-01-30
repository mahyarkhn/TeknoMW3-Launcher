using System.Xml.Serialization;

namespace eheh.Helpers
{
    public class MetaFile
    {
        [XmlArray("Tasks")]
        public MetaFileEntry[] Tasks
        {
            get;
            set;
        }

        [XmlAttribute("checksum")]
        public string FolderChecksum
        {
            get;
            set;
        }
    }
}
