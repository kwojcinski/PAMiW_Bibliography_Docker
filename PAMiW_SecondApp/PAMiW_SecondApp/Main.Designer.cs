namespace PAMiW_SecondApp
{
    partial class Main
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AddPosition = new System.Windows.Forms.Button();
            this.ShowPosition = new System.Windows.Forms.Button();
            this.AddFile = new System.Windows.Forms.Button();
            this.ShowFiles = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(87, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dodaj pozycję";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(87, 110);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Wyświetl moje pozycje";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Dodaj plik";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(87, 209);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Wyświetl moje pliki";
            // 
            // AddPosition
            // 
            this.AddPosition.Location = new System.Drawing.Point(270, 65);
            this.AddPosition.Name = "AddPosition";
            this.AddPosition.Size = new System.Drawing.Size(75, 23);
            this.AddPosition.TabIndex = 4;
            this.AddPosition.Text = "Dodaj";
            this.AddPosition.UseVisualStyleBackColor = true;
            this.AddPosition.Click += new System.EventHandler(this.AddPosition_Click);
            // 
            // ShowPosition
            // 
            this.ShowPosition.Location = new System.Drawing.Point(270, 110);
            this.ShowPosition.Name = "ShowPosition";
            this.ShowPosition.Size = new System.Drawing.Size(75, 23);
            this.ShowPosition.TabIndex = 5;
            this.ShowPosition.Text = "Wyświetl";
            this.ShowPosition.UseVisualStyleBackColor = true;
            this.ShowPosition.Click += new System.EventHandler(this.ShowPosition_Click);
            // 
            // AddFile
            // 
            this.AddFile.Location = new System.Drawing.Point(270, 160);
            this.AddFile.Name = "AddFile";
            this.AddFile.Size = new System.Drawing.Size(75, 23);
            this.AddFile.TabIndex = 6;
            this.AddFile.Text = "Dodaj";
            this.AddFile.UseVisualStyleBackColor = true;
            this.AddFile.Click += new System.EventHandler(this.AddFile_Click);
            // 
            // ShowFiles
            // 
            this.ShowFiles.Location = new System.Drawing.Point(270, 209);
            this.ShowFiles.Name = "ShowFiles";
            this.ShowFiles.Size = new System.Drawing.Size(75, 23);
            this.ShowFiles.TabIndex = 7;
            this.ShowFiles.Text = "Wyświetl";
            this.ShowFiles.UseVisualStyleBackColor = true;
            this.ShowFiles.Click += new System.EventHandler(this.ShowFiles_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 328);
            this.Controls.Add(this.ShowFiles);
            this.Controls.Add(this.AddFile);
            this.Controls.Add(this.ShowPosition);
            this.Controls.Add(this.AddPosition);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Main";
            this.Text = "PAMiW_291118";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button AddPosition;
        private System.Windows.Forms.Button ShowPosition;
        private System.Windows.Forms.Button AddFile;
        private System.Windows.Forms.Button ShowFiles;
    }
}