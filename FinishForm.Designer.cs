namespace WorkApp
{
    partial class FinishForm
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
            this.RoomNames = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SomeLevels = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // RoomNames
            // 
            this.RoomNames.AutoSize = true;
            this.RoomNames.Location = new System.Drawing.Point(12, 24);
            this.RoomNames.Name = "RoomNames";
            this.RoomNames.Size = new System.Drawing.Size(195, 17);
            this.RoomNames.TabIndex = 0;
            this.RoomNames.Text = "Добавлять названия помещений";
            this.RoomNames.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.No;
            this.button1.Location = new System.Drawing.Point(0, 211);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(269, 51);
            this.button1.TabIndex = 1;
            this.button1.Text = "Посчитать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SomeLevels
            // 
            this.SomeLevels.AutoSize = true;
            this.SomeLevels.Location = new System.Drawing.Point(12, 64);
            this.SomeLevels.Name = "SomeLevels";
            this.SomeLevels.Size = new System.Drawing.Size(195, 17);
            this.SomeLevels.TabIndex = 0;
            this.SomeLevels.Text = "Добавлять названия помещений";
            this.SomeLevels.UseVisualStyleBackColor = true;
            // 
            // FinishForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(261, 255);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.SomeLevels);
            this.Controls.Add(this.RoomNames);
            this.Name = "FinishForm";
            this.Text = "FinishForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox RoomNames;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox SomeLevels;
    }
}