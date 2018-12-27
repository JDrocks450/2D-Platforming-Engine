using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperMario
{
    public class Spritesheet
    {
        int[,,] data;

        Texture2D texture;

        public int Rows
        {
            get;
        }
        public int Columns
        {
            get => data[Row, 0, 0];
        }

        public int Row
        {
            get;set;
        }
        public int Column
        {
            get;set;
        }

        public Spritesheet(Texture2D Texture, int rows, params int[] itemsPerRow)
        {
            texture = Texture;
            int columns = itemsPerRow.Max();
            data = new int[rows, columns, 1];
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    data[row, column, 0] = itemsPerRow[row];
                }
            }
            Rows = rows;
        }

        public Rectangle GetFrame(int row, int column)
        {
            while (column > Columns)
                column -= Columns;
            var items = data[row, column, 0];
            int frameWidth = texture.Width / items;
            int frameHeight = texture.Height / Rows;
            return new Rectangle(frameWidth * column, frameHeight * row, frameWidth, frameHeight);
        }

        public Rectangle MoveToFrame(int row, int column)
        {
            Row = row;
            Column = column;
            while (Column > Columns)
                Column -= Columns;
            return GetFrame(row, Column);
        }

        /// <summary>
        /// Advances Column index by the amount and returns that frame, looping back to the start if there are no more columns
        /// </summary>
        /// <param name="amount">The amount to advance</param>
        /// <returns></returns>
        public Rectangle AdvanceFrame(int amount = 1)
        {
            Column += amount;            
            return MoveToFrame(Row, Column);
        }
    }
}
