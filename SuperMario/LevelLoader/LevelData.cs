using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.LevelLoader
{
    public class LevelData
    {
        const int BLOCK_SIZE = 24;

        string uri;

        public enum OBJ_TABLE
        {
            EMPTY,
            BLOCK,
            MARIO,
            QUES_BLOCK,
            GROUND,
            BUSH,
            GOOMBA,
        }

        enum DATA_LAYOUT
        {
            DEFAULT,
            X = 0,
            Y = 8,
            ID = 16,
            NOTHING = 20,
            WIDTH = 24,
            HEIGHT = 32
        }

        public List<GameObject> LoadedObjects = new List<GameObject>();

        int[] dataLengthTable = new int[] { 8, 8, 4, 4,8,8 };

        byte[] fileData;

        public static LevelData LoadFile(string url)
        {
            var data = new LevelData();
            data.uri = url;
            if (true)
            {
                if (!File.Exists(url))
                    File.Create(url);                
            }
            using (var file = File.OpenRead(url))
            {
                data.fileData = new byte[file.Length];
                file.Read(data.fileData, 0, data.fileData.Length);
            }
            if (data.fileData.Length > 0)
                data.ReadData();
            return data;
        }

        void WriteDebugData(string url)
        {
            using (var file = File.OpenWrite(url))
            {
                int x = 1000;
                int y = 500;
                byte id = 1;
                string Xhex = x.ToString("X8");
                string Yhex = y.ToString("X8");
                string idHex = id.ToString("X4");
                var bytes = ASCIIEncoding.ASCII.GetBytes(" ");
                file.Write(bytes, 0, bytes.Length);
                bytes = ASCIIEncoding.ASCII.GetBytes(Xhex);
                file.Write(bytes, 0, bytes.Length);
                bytes = ASCIIEncoding.ASCII.GetBytes(Yhex);
                file.Write(bytes, 0, bytes.Length);
                bytes = ASCIIEncoding.ASCII.GetBytes(idHex);
                file.Write(bytes, 0, bytes.Length);
                bytes = ASCIIEncoding.ASCII.GetBytes(((int)0).ToString("X4"));
                file.Write(bytes, 0, bytes.Length);
            }
        }

        void ReadData()
        {
            if (Encoding.ASCII.GetChars(new byte[] { fileData[0] }).First() != ' ')
                throw new Exception("Level not formatted");
            List<char> block = new List<char>();
            foreach (var b in fileData)
            {
                char data = Encoding.ASCII.GetChars(new byte[] { b })[0];
                if (data == ' ')
                {
                    if (block.Count != 0)
                    {
                        Parse(block.ToArray());
                        block.Clear();
                    }
                }
                else
                    block.Add(data);
            }
            if (block.Count > 0)
                Parse(block.ToArray());
        }        

        void Parse(char[] block)
        {
            int X =0, Y=0;
            int width=0, height=0;
            byte OBJID = 0;
            int offset = 0;
            for (int i = 0; i < dataLengthTable.Length; i++)
            {
                var data = new string(block.Skip(offset).Take(dataLengthTable[i]).ToArray());
                int intFromHex = 0;
                byte byteFromHex = 0;
                if (dataLengthTable[i] == 8)
                    intFromHex = int.Parse(data, System.Globalization.NumberStyles.HexNumber);
                if (dataLengthTable[i] == 4)
                    byteFromHex = byte.Parse(data, System.Globalization.NumberStyles.HexNumber);
                switch ((DATA_LAYOUT)offset)
                {
                    case DATA_LAYOUT.X:
                        X = intFromHex;
                        break;
                    case DATA_LAYOUT.Y:
                        Y = intFromHex;
                        break;
                    case DATA_LAYOUT.ID:
                        OBJID = byteFromHex;
                        break;
                    case DATA_LAYOUT.WIDTH:
                        width = intFromHex;
                        break;
                    case DATA_LAYOUT.HEIGHT:
                        height = intFromHex;
                        break;
                }
                offset += dataLengthTable[i];
            }
            CreateGameObject(OBJID, X, Y, width, height, block.Skip(offset).Take(block.Length - offset).ToArray());
        }

        void CreateGameObject(byte ID, int X, int Y, int Width, int Height, char[] raw)
        {
            GameObject obj = null;
            var box = new Microsoft.Xna.Framework.Rectangle(X, Y, Width, Height);
            obj = GetInstanceByID(ID, box);
            if (obj is null)
                throw new Exception("obj not loaded");
            obj.LoadFromFile(raw);
            LoadedObjects.Add(obj);
        }

        public GameObject GetInstanceByID(byte objID, Rectangle StartPos)
        {
            GameObject obj = null;
            var box = StartPos;
            switch ((OBJ_TABLE)objID)
            {
                case OBJ_TABLE.BLOCK:
                    obj = new PrefabObjects.Block(box);
                    break;
                case OBJ_TABLE.GROUND:
                    obj = new PrefabObjects.Ground(box);
                    break;
                case OBJ_TABLE.BUSH:
                    obj = new PrefabObjects.Bush(StartPos.Location);
                    break;
                case OBJ_TABLE.GOOMBA:
                    obj = new Enemies.Goomba(box.Location, Enemies.Goomba.Direction.Left);
                    break;
                case OBJ_TABLE.MARIO:
                    obj = new Player(box);
                    break;
                case OBJ_TABLE.QUES_BLOCK:
                    obj = new PrefabObjects.QuestionBlock(box, PrefabObjects.QuestionBlock.SpawnObjectLogic.Inferred, null);
                    break;
            }
            return obj;
        }

        public byte GetIDByInstance(GameObject obj)
        {
            for(byte i = 0; i < 255; i++)
            {
                var t = GetInstanceByID(i, new Rectangle())?.GetType();
                if (t == null)
                    continue;
                if (t == obj.GetType())
                    return i;
            }
            return 0;
        }

        public void WriteObjectDataToFile(GameObject obj)
        {
            int offset = 0;
            List<byte> block = new List<byte>();
            block.Add(ASCIIEncoding.ASCII.GetBytes(" ")[0]);
            foreach(var length in dataLengthTable)
            {
                string data = "";
                switch ((DATA_LAYOUT)offset)
                {
                    case DATA_LAYOUT.X:
                        data = ((int)obj.X).ToString("X8");
                        break;
                    case DATA_LAYOUT.Y:
                        data = ((int)obj.Y).ToString("X8");
                        break;
                    case DATA_LAYOUT.WIDTH:
                        data = ((int)obj.Width).ToString("X8");
                        break;
                    case DATA_LAYOUT.HEIGHT:
                        data = ((int)obj.Height).ToString("X8");
                        break;
                    case DATA_LAYOUT.NOTHING:
                        data = "0000";
                        break;
                    case DATA_LAYOUT.ID:
                        data = GetIDByInstance(obj).ToString("X4");
                        if (data == "0000")
                            throw new Exception("Object not implemented");
                        break;
                }
                block.AddRange(Encoding.ASCII.GetBytes(data));
                offset += length;
            }
            block.AddRange(Encoding.ASCII.GetBytes(obj.GetBlockData));
            using (var f = File.OpenWrite(uri))
            {
                f.Position = f.Length;
                f.Write(block.ToArray(), 0, block.Count);
            }
        }
    }
}
