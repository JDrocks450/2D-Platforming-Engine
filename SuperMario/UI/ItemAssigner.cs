using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SuperMario.PrefabObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario.UI
{
    public class ItemAssigner : UIComponent
    {
        private QuestionBlock _subject;
        public event EventHandler Closed;

        public bool Disabled
        {
            get; set;
        }

        public int X
        {
            get;
            set;
        }

        public int Y
        {
            get;
            set;
        }

        public int Width
        {
            get;
            set;
        }

        public int Height
        {
            get;
            set;
        }

        public QuestionBlock Subject
        {
            get => _subject;
            set
            {
                _subject = value;
                Core.GameCamera.Focus = value;
            }
        }

        public bool Editing
        {
            get; set;
        }

        ObjectSpawner baseObjectSpawner;

        /// <summary>
        /// This will focus the camera onto the Subject and open the assigner on QuestionBlock
        /// </summary>
        /// <param name="Subject">The questionblock to inspect</param>
        /// <param name="editing">Allow editing?</param>
        public ItemAssigner(QuestionBlock Subject, bool editing = true)
        {
            this.Subject = Subject;
            Editing = editing;
            baseObjectSpawner = new ObjectSpawner(new Rectangle(0, 0, 0, 0), true);
            baseObjectSpawner.OnObjectSpawnRequested += BaseObjectSpawner_OnObjectSpawnRequested;
        }

        private void BaseObjectSpawner_OnObjectSpawnRequested(byte id)
        {
            Subject.StoredItem = Items.Item.Parse(id, out _);
            Close();
        }

        void UIComponent.Update(Microsoft.Xna.Framework.GameTime gt)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.OemTilde))
                Close();
            if (!baseObjectSpawner.Populated)
                baseObjectSpawner.PopulateWith(typeof(Items.Item.ITEM_TABLE), 9);
            baseObjectSpawner.Update(gt);
            //Positioning
            Width = baseObjectSpawner.Width + 10;
            Height = baseObjectSpawner.Height + 10;
            X = Core.GameCamera.Screen.Width / 2 - Width / 2; //Middle of Screen
            Y = (Core.GameCamera.Screen.Height / 2 - Height) - Subject.Height / 2; //Middle of Screen and above the Subject
            baseObjectSpawner.X = X + 5;
            baseObjectSpawner.Y = Y + 5;
        }

        void UIComponent.Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            sb.Draw(Core.BaseTexture, new Rectangle(X, Y, Width, Height), Color.Black);
            baseObjectSpawner.Draw(sb);
        }

        void Close()
        {
            Disabled = true;
            Closed?.Invoke(this, null);
        }
    }
}
