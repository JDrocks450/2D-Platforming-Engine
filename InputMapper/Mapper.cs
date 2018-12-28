using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace InputMapper
{
    public partial class Mapper : Form
    {
        Dictionary<string, char> keys = new Dictionary<string, char>();

        bool generating = false;
        public Mapper()
        {
            InitializeComponent();
            KeyPreview = true;
        }

        bool keyWaiting = true;
        char keyPressed;
        bool shouldLoop = true;
        bool editing = false;
        public async void SetupControls()
        {
            int i = 0;
            while (shouldLoop) //ESC
            {
                retry:
                name.Enabled = generating;
                set.Enabled = generating;                                
                if (generating)
                {
                    name.Focus();
                    status.Text = "Input a keybind name...";
                    await Task.Run(() =>
                    {
                        while (!nameSet) { };
                    });
                }
                else if (i < keys.Count)
                {
                    editing = true;
                    name.Text = keys.Skip(i).Take(1).First().Key;
                }
                else if (editing == true)
                {
                    MessageBox.Show("There aren't any more bindings to be edited. More will be added if you continue.");
                    editing = false;
                    generating = true;
                    goto retry;
                }
                nameSet = false;
                name.Enabled = false;
                set.Enabled = false;
                status.Text = "Press a key for bind: " + name.Text;
                keyWaiting = true;                
                await Task.Run(() =>
                {
                    while (keyWaiting) { }
                });
                if (!editing)
                    keys.Add(name.Text, keyPressed);
                else
                    keys[name.Text] = keyPressed;
                name.Clear();
                DisplayKeys();
                i++;
            }
        }

        public void DisplayKeys()
        {
            history.Clear();
            foreach(var str in keys.Keys)            
                history.Text += $"{'"' + str + '"'} :: {keys[str]}{Environment.NewLine}";            
        }

        private void genmode_Click(object sender, EventArgs e)
        {
            generating = true;
            (sender as Button).Text = "Generating...";
            (sender as Button).Enabled = false;
            SetupControls();
        }

        bool nameSet = false;
        private void set_Click(object sender, EventArgs e)
        {
            nameSet = true;
        }

        private void k(object sender, KeyEventArgs e)
        {
            if (!nameSet && e.KeyCode == Keys.Enter)
            {
                nameSet = true;
                return;
            }
            keyPressed = (char)e.KeyCode;
            keyWaiting = false;
        }

        private void save_Click(object sender, EventArgs e)
        {
            shouldLoop = false;
            var xml = new XDocument();
            xml.AddAnnotation("Created by Mapper by Jeremy Glazebrook");
            var element = new XElement("Mapper");
            xml.Add(element);
            int i = 0;
            foreach (var str in keys.Keys)
            {
                i++;
                element.Add(
                    new XElement($"Binding{i}", 
                    new XElement("Name", str),
                    new XElement("Key", keys[str])));
            }
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                , "Settings");
            Directory.CreateDirectory(path);
            path = Path.Combine(path, "control.xml");
            xml.Save(path);
            System.Diagnostics.Process.Start(path);
        }

        private void load_Click(object sender, EventArgs e)
        {
            var r = openfile.ShowDialog();
            if (r == DialogResult.Cancel)
                return;
            genmode.Enabled = false;
            load.Enabled = false;
            var doc = XDocument.Load(openfile.FileName);
            foreach(var d in doc.Root.Elements())
            {
                var key = d.Element("Key").Value;
                if (key == "")
                    key = " ";
                keys.Add(d.Element("Name").Value, key.ToCharArray().First());
            }
            DisplayKeys();
            SetupControls();
        }
    }
}
