using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;

using Gears.Cloud._Debug;

namespace Gears.Cartography
{
    //TODO: This needs to be refactored and parameterized.
    public sealed class MapEngine
    {
        private Map map0; //for testing purposes

        //used for later stages.
        private const string _VERSION = "0.1.0";
        private const string _KEY = @""; //some key

        public MapEngine()
        {

        }
        public void DebugSerialize(string SAVE_LOCATION)
        {
            map0 = new Map();
            PopulateFields();
            SerializeToXML(map0, SAVE_LOCATION);
        }
        public void DebugDeserialize(string LOAD_LOCATION)
        {
            DeserializeFromXML(LOAD_LOCATION);
        }

        private void PopulateFields()
        {
            //debug values
            map0.VERSION = "0.1.0";

            map0.BGM_FILE_LOC = "BGM_FILE_LOC DIRECTORY test";
            map0.FADE_IN_FILE_LOC = "FADE_IN_FILE_LOC DIRECTORY test";
            map0.FADE_OUT_FILE_LOC = "FADE_OUT_FILE_LOC DIRECTORY test";
            map0.BG_IMAGE_FILE_LOC = "BG_IMAGE_FILE_LOC DIRECTORY test";

            map0.NUM_LAYERS = 5;
            map0.LAYER_WIDTH_TILES = 80;
            map0.LAYER_HEIGHT_TILES = 35;

        }
        private static void SerializeToXML(Map map, string SAVE_LOCATION)
        {
            using (TextWriter textWriter = new StreamWriter(SAVE_LOCATION))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Map));
                serializer.Serialize(textWriter, map);
                textWriter.Close();
            }
        }
        private static Map DeserializeFromXML(string LOAD_LOCATION)
        {
            try
            {
                using (TextReader textReader = new StreamReader(LOAD_LOCATION))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(Map));
                    Map map;
                    try
                    {
                        map = (Map)deserializer.Deserialize(textReader);

                        Debug.Out("@MAP/VERSION=" + map.VERSION);
                        Debug.Out("@MAP/BGMFILE=" + map.BGM_FILE_LOC);
                        Debug.Out("@MAP/FADEINFILE=" + map.FADE_IN_FILE_LOC);
                        Debug.Out("@MAP/FADEOUTFILE=" + map.FADE_OUT_FILE_LOC);
                        Debug.Out("@MAP/BGIFILE=" + map.BG_IMAGE_FILE_LOC);
                        Debug.Out("@MAP/LAYERS=" + map.NUM_LAYERS);
                        Debug.Out("@MAP/LAYERWIDTH=" + map.LAYER_WIDTH_TILES);
                        Debug.Out("@MAP/LAYERHEIGHT=" + map.LAYER_HEIGHT_TILES);
                        return map;
                    }
                    catch (InvalidOperationException ioe)
                    {
                        Debug.Out("##MapEngine.DeserializeFromXML(): An error has occurred. The XML file read from " + LOAD_LOCATION + " is of an incompatible format.");
                        Debug.Out(ioe.Message);
                    }

                    textReader.Close();
                    return null;
                }
            }
            catch (ArgumentNullException ane)
            {
                Debug.Out("##MapEngine.DeserializeFromXML(): An error has occurred. ArgumentNullException.");
                Debug.Out(ane.Message);
            }
            catch (FileNotFoundException fnfe)
            {
                Debug.Out("##MapEngine.DeserializeFromXML(): An error has occurred. The file for " + LOAD_LOCATION + " does not exist.");
                Debug.Out(fnfe.Message);
            }
            catch (DirectoryNotFoundException dnfe)
            {
                Debug.Out("##MapEngine.DeserializeFromXML(): An error has occurred. The directory for " + LOAD_LOCATION + " does not exist.");
                Debug.Out(dnfe.Message);
            }
            catch (IOException ioe)
            {
                Debug.Out("##MapEngine.DeserializeFromXML(): An error has occurred. IOException.");
                Debug.Out(ioe.Message);
            }
            catch (ArgumentException ae)
            {
                Debug.Out("##MapEngine.DeserializeFromXML(): An error has occurred. ArgumentException.");
                Debug.Out(ae.Message);
            }
            return null;

        }
    }
    //TODO: Cleanup.
    public class Map
    {
        //ASSIGNMENTS ARE FOR TESTING PURPOSES ONLY!!!
        protected Map(Map copyMe)
        {
            VERSION = copyMe.VERSION;
            BGM_FILE_LOC = copyMe.BGM_FILE_LOC;
            FADE_IN_FILE_LOC = copyMe.FADE_IN_FILE_LOC;
            FADE_OUT_FILE_LOC = copyMe.FADE_OUT_FILE_LOC;
            BG_IMAGE_FILE_LOC = copyMe.BG_IMAGE_FILE_LOC;
            NUM_LAYERS = copyMe.NUM_LAYERS;
            LAYER_WIDTH_TILES = copyMe.LAYER_WIDTH_TILES;
            LAYER_HEIGHT_TILES = copyMe.LAYER_HEIGHT_TILES;
            LAYERS = new List<layer>();

            foreach (layer lay in copyMe.LAYERS)
                LAYERS.Add(lay);


        }

        public Map() { }

        [XmlAttribute("vers")]
        public string VERSION {get; set;}

        [XmlElement("bgmfile")]
        public string BGM_FILE_LOC {get; set;}
        [XmlElement("fadeinfile")]
        public string FADE_IN_FILE_LOC {get; set;}
        [XmlElement("fadeoutfile")]
        public string FADE_OUT_FILE_LOC {get; set;}
        [XmlElement("bgifile")]
        public string BG_IMAGE_FILE_LOC {get; set;}

        [XmlElement("layers")]
        public byte NUM_LAYERS {get; set;}
            [XmlAttribute("width")]
            public int LAYER_WIDTH_TILES {get; set;}
            [XmlAttribute("height")]
            public int LAYER_HEIGHT_TILES {get; set;}

            [XmlElement("data")]
            public List<layer> LAYERS { get; set; }


            protected static Map deserializeFromXml(string fileName)
            {
                XmlSerializer deserializer = new XmlSerializer(typeof(Map));
                TextReader reader = new StreamReader(fileName);

                return (Gears.Cartography.Map)deserializer.Deserialize(reader);
            }



    }

    public class layer
    {
        [XmlAttribute("layerName")]
        public string NAME { get; set; }

        [XmlElement("tiles")]
        public List<tile> TILES { get; set; }

        [XmlElement("components")]
        public List<component> COMPONENTS { get; set; }


    }

    public class tile
    {
        [XmlAttribute("coordinates")]
        public string COORDS { get; set; }
        [XmlAttribute("tileSet")]
        public string TILESET { get; set; }
        [XmlAttribute("tileID")]
        public string TILEID { get; set; }
    }

    public class component
    {
        [XmlAttribute("coordinates")]
        public string COORDS { get; set; }
        [XmlAttribute("name")]
        public string NAME { get; set; }

        //first actor in the list is the root
        [XmlElement("actors")]
        public List<actors> ACTORS { get; set; }
        
    }

    public class actors
    {
        [XmlAttribute("className")]
        public Type actor { get; set; }
    }


    //TODO
    internal class MapLoader : Map
    {
        //strings instead of values. throw exception if not cool
    }



    public class InvalidMapFileFormatException : System.IO.FileLoadException
    {
        public InvalidMapFileFormatException() { }
        public InvalidMapFileFormatException(string message) { }
        public InvalidMapFileFormatException(string message, System.Exception inner) { }

        // Constructor needed for serialization 
        // when exception propagates from a remoting server to the client.
        protected InvalidMapFileFormatException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) { }
    }
}
