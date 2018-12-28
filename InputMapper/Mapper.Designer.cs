namespace InputMapper
{
    partial class Mapper
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.status = new System.Windows.Forms.Label();
            this.save = new System.Windows.Forms.Button();
            this.genmode = new System.Windows.Forms.Button();
            this.name = new System.Windows.Forms.TextBox();
            this.label = new System.Windows.Forms.Label();
            this.set = new System.Windows.Forms.Button();
            this.history = new System.Windows.Forms.TextBox();
            this.load = new System.Windows.Forms.Button();
            this.openfile = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(13, 13);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(97, 13);
            this.status.TabIndex = 0;
            this.status.Text = "Make a selection...";
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(239, 102);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 1;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // genmode
            // 
            this.genmode.Location = new System.Drawing.Point(53, 102);
            this.genmode.Name = "genmode";
            this.genmode.Size = new System.Drawing.Size(99, 23);
            this.genmode.TabIndex = 2;
            this.genmode.Text = "Generate";
            this.genmode.UseVisualStyleBackColor = true;
            this.genmode.Click += new System.EventHandler(this.genmode_Click);
            // 
            // name
            // 
            this.name.Enabled = false;
            this.name.Location = new System.Drawing.Point(16, 55);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(227, 20);
            this.name.TabIndex = 3;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(13, 37);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(79, 13);
            this.label.TabIndex = 4;
            this.label.Text = "Keybind Name:";
            // 
            // set
            // 
            this.set.Enabled = false;
            this.set.Location = new System.Drawing.Point(249, 53);
            this.set.Name = "set";
            this.set.Size = new System.Drawing.Size(65, 23);
            this.set.TabIndex = 5;
            this.set.Text = "Set";
            this.set.UseVisualStyleBackColor = true;
            this.set.Click += new System.EventHandler(this.set_Click);
            // 
            // history
            // 
            this.history.BackColor = System.Drawing.Color.White;
            this.history.ForeColor = System.Drawing.Color.Green;
            this.history.Location = new System.Drawing.Point(16, 132);
            this.history.Multiline = true;
            this.history.Name = "history";
            this.history.ReadOnly = true;
            this.history.Size = new System.Drawing.Size(298, 181);
            this.history.TabIndex = 6;
            this.history.Text = "Empty";
            // 
            // load
            // 
            this.load.Location = new System.Drawing.Point(158, 102);
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(75, 23);
            this.load.TabIndex = 7;
            this.load.Text = "Load";
            this.load.UseVisualStyleBackColor = true;
            this.load.Click += new System.EventHandler(this.load_Click);
            // 
            // openfile
            // 
            this.openfile.Title = "Select Control File";
            // 
            // Mapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(326, 325);
            this.Controls.Add(this.load);
            this.Controls.Add(this.history);
            this.Controls.Add(this.set);
            this.Controls.Add(this.label);
            this.Controls.Add(this.name);
            this.Controls.Add(this.genmode);
            this.Controls.Add(this.save);
            this.Controls.Add(this.status);
            this.Name = "Mapper";
            this.Text = "Mapper";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.k);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button genmode;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.Button set;
        private System.Windows.Forms.TextBox history;
        private System.Windows.Forms.Button load;
        private System.Windows.Forms.OpenFileDialog openfile;
    }
}