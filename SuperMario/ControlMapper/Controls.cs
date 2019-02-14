using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SuperMario.ControlMapper
{
    public class Controls
    {
        public const string XMLNAME = "control.xml";

        public enum MENUKeys
        {
            DEBUG_PLAYER_CREATE = 'P',
            DEBUG_OBJECT_CREATE = 'O',
            DEBUG_GOOMBA_CREATE = 'G'
        }

        XDocument controlFile;
        public IDictionary<char, string> KeyBinds = new Dictionary<char, string>();
        public Controls(string xmlPath)
        {
            controlFile = XDocument.Load(xmlPath);
            foreach (var b in controlFile.Root.Elements())
            {
                var name = b.Element("Name").Value;
                name = name.ToLower();
                var value = b.Element("Key").Value;
                char cval = (char)0;
                //special logic for spaces
                if (value == "")
                    value = " ";
                if (value == "[shift]")
                    cval = (char)160;
                KeyBinds.Add(cval != 0 ? cval : value.ToCharArray().First(), name);
            }
        }

        public List<string> GetKeyControl()
        {
            var keyResult = new List<string>();
            foreach(var key in Keyboard.GetState().GetPressedKeys())
            {
                if (KeyBinds.Keys.Contains((char)key))                
                    keyResult.Add(KeyBinds[(char)key]);                
            }
            return keyResult;
        }

        public List<Keys> prevPress;
        public List<MENUKeys> MenuKeys(KeyboardState state)
        {
            var presses = new List<MENUKeys>();
            var values = Enum.GetValues(typeof(MENUKeys));
            var array = new int[values.Length];
            values.CopyTo(array, 0);
            foreach (var K in state.GetPressedKeys())
            {
                if (!prevPress.Contains(K))                    
                {
                    if (array.Contains((int)K))
                        presses.Add((MENUKeys)K);
                }
            }
            prevPress = state.GetPressedKeys().ToList();
            return presses;
        }
    }
}
