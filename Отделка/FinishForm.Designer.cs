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
            this.SomeLevels = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.chkSplitLevel = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ColumnFinSelector = new System.Windows.Forms.ComboBox();
            this.LocFinSelector = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkCol = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.modelFloor = new System.Windows.Forms.CheckBox();
            this.modelCeil = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.checkGroup = new System.Windows.Forms.CheckBox();
            this.GroupSelector = new System.Windows.Forms.ComboBox();
            this.SelNum = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.GroupFloorSelector = new System.Windows.Forms.ComboBox();
            this.checkFloorGroup = new System.Windows.Forms.CheckBox();
            this.locWallParam = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // RoomNames
            // 
            this.RoomNames.AutoSize = true;
            this.RoomNames.Location = new System.Drawing.Point(3, 32);
            this.RoomNames.Name = "RoomNames";
            this.RoomNames.Size = new System.Drawing.Size(195, 17);
            this.RoomNames.TabIndex = 0;
            this.RoomNames.Text = "Добавлять названия помещений";
            this.RoomNames.UseVisualStyleBackColor = true;
            // 
            // SomeLevels
            // 
            this.SomeLevels.AutoSize = true;
            this.SomeLevels.Location = new System.Drawing.Point(3, 55);
            this.SomeLevels.Name = "SomeLevels";
            this.SomeLevels.Size = new System.Drawing.Size(122, 17);
            this.SomeLevels.TabIndex = 0;
            this.SomeLevels.Text = "Несколько этажей";
            this.SomeLevels.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 174);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Выберите стадию:";
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.RoomNames);
            this.panel1.Controls.Add(this.checkBox4);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.chkSplitLevel);
            this.panel1.Controls.Add(this.SomeLevels);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(878, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(235, 654);
            this.panel1.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(235, 22);
            this.panel3.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Параметры отделки";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(3, 124);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(229, 17);
            this.checkBox4.TabIndex = 0;
            this.checkBox4.Text = "Помещения имеют одинаковые номера";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.checkBox4.MouseHover += new System.EventHandler(this.checkBox1_MouseHover);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(3, 101);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(189, 17);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Считать для новых перегородок";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.checkBox1.MouseHover += new System.EventHandler(this.checkBox1_MouseHover);
            // 
            // chkSplitLevel
            // 
            this.chkSplitLevel.AutoSize = true;
            this.chkSplitLevel.Location = new System.Drawing.Point(3, 78);
            this.chkSplitLevel.Name = "chkSplitLevel";
            this.chkSplitLevel.Size = new System.Drawing.Size(125, 17);
            this.chkSplitLevel.TabIndex = 0;
            this.chkSplitLevel.Text = "Разбить по этажам";
            this.chkSplitLevel.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 590);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(878, 64);
            this.panel2.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.No;
            this.button1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(878, 64);
            this.button1.TabIndex = 1;
            this.button1.Text = "Посчитать";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 73.76637F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.566967F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ColumnFinSelector, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.LocFinSelector, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboBox1, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBox2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.checkBox3, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.checkCol, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.label7, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label9, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.label10, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.modelFloor, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.modelCeil, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.label11, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.checkGroup, 2, 6);
            this.tableLayoutPanel1.Controls.Add(this.GroupSelector, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.SelNum, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.label12, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.PhaseSelector, 1, 8);
            this.tableLayoutPanel1.Controls.Add(this.label13, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.GroupFloorSelector, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.checkFloorGroup, 2, 7);
            this.tableLayoutPanel1.Controls.Add(this.locWallParam, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(878, 590);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "Отделка колонн:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(98, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Местная отделка:";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 20);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Основная отделка:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(3, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(114, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Элемент отделки:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ColumnFinSelector
            // 
            this.ColumnFinSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ColumnFinSelector.FormattingEnabled = true;
            this.ColumnFinSelector.Location = new System.Drawing.Point(123, 70);
            this.ColumnFinSelector.Name = "ColumnFinSelector";
            this.ColumnFinSelector.Size = new System.Drawing.Size(553, 21);
            this.ColumnFinSelector.TabIndex = 4;
            // 
            // LocFinSelector
            // 
            this.LocFinSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LocFinSelector.FormattingEnabled = true;
            this.LocFinSelector.Location = new System.Drawing.Point(123, 43);
            this.LocFinSelector.Name = "LocFinSelector";
            this.LocFinSelector.Size = new System.Drawing.Size(553, 21);
            this.LocFinSelector.TabIndex = 4;
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(123, 16);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(553, 21);
            this.comboBox1.TabIndex = 4;
            // 
            // checkBox2
            // 
            this.checkBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(707, 19);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(707, 46);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 5;
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkCol
            // 
            this.checkCol.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkCol.AutoSize = true;
            this.checkCol.Location = new System.Drawing.Point(707, 73);
            this.checkCol.Name = "checkCol";
            this.checkCol.Size = new System.Drawing.Size(15, 14);
            this.checkCol.TabIndex = 5;
            this.checkCol.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(682, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "По типу:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Location = new System.Drawing.Point(123, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(553, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Тип стен отделки";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Location = new System.Drawing.Point(123, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(553, 20);
            this.label9.TabIndex = 3;
            this.label9.Text = "Пол в модели";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Location = new System.Drawing.Point(123, 114);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(553, 20);
            this.label10.TabIndex = 3;
            this.label10.Text = "Потолок в модели:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // modelFloor
            // 
            this.modelFloor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.modelFloor.AutoSize = true;
            this.modelFloor.Location = new System.Drawing.Point(707, 97);
            this.modelFloor.Name = "modelFloor";
            this.modelFloor.Size = new System.Drawing.Size(15, 14);
            this.modelFloor.TabIndex = 5;
            this.modelFloor.UseVisualStyleBackColor = true;
            // 
            // modelCeil
            // 
            this.modelCeil.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.modelCeil.AutoSize = true;
            this.modelCeil.Location = new System.Drawing.Point(707, 117);
            this.modelCeil.Name = "modelCeil";
            this.modelCeil.Size = new System.Drawing.Size(15, 14);
            this.modelCeil.TabIndex = 5;
            this.modelCeil.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(3, 134);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(114, 20);
            this.label11.TabIndex = 3;
            this.label11.Text = "Группирование:";
            // 
            // checkGroup
            // 
            this.checkGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkGroup.AutoSize = true;
            this.checkGroup.Location = new System.Drawing.Point(707, 137);
            this.checkGroup.Name = "checkGroup";
            this.checkGroup.Size = new System.Drawing.Size(15, 14);
            this.checkGroup.TabIndex = 5;
            this.checkGroup.UseVisualStyleBackColor = true;
            this.checkGroup.CheckedChanged += new System.EventHandler(this.checkGroup_CheckedChanged);
            // 
            // GroupSelector
            // 
            this.GroupSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupSelector.FormattingEnabled = true;
            this.GroupSelector.Location = new System.Drawing.Point(123, 137);
            this.GroupSelector.Name = "GroupSelector";
            this.GroupSelector.Size = new System.Drawing.Size(553, 21);
            this.GroupSelector.TabIndex = 6;
            this.GroupSelector.SelectedIndexChanged += new System.EventHandler(this.GroupSelector_SelectedIndexChanged);
            // 
            // SelNum
            // 
            this.SelNum.AutoSize = true;
            this.SelNum.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelNum.Location = new System.Drawing.Point(123, 194);
            this.SelNum.Name = "SelNum";
            this.SelNum.Size = new System.Drawing.Size(553, 396);
            this.SelNum.TabIndex = 3;
            this.SelNum.Text = "Все помещения";
            this.SelNum.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Location = new System.Drawing.Point(3, 194);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(114, 396);
            this.label12.TabIndex = 3;
            this.label12.Text = "Выбрано элементов:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PhaseSelector
            // 
            this.PhaseSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PhaseSelector.FormattingEnabled = true;
            this.PhaseSelector.Location = new System.Drawing.Point(123, 177);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(553, 21);
            this.PhaseSelector.TabIndex = 2;
            this.PhaseSelector.SelectedIndexChanged += new System.EventHandler(this.PhaseSelector_SelectedIndexChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label13.Location = new System.Drawing.Point(3, 154);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(114, 20);
            this.label13.TabIndex = 3;
            this.label13.Text = "Группирование пола:";
            // 
            // GroupFloorSelector
            // 
            this.GroupFloorSelector.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupFloorSelector.FormattingEnabled = true;
            this.GroupFloorSelector.Location = new System.Drawing.Point(123, 157);
            this.GroupFloorSelector.Name = "GroupFloorSelector";
            this.GroupFloorSelector.Size = new System.Drawing.Size(553, 21);
            this.GroupFloorSelector.TabIndex = 6;
            // 
            // checkFloorGroup
            // 
            this.checkFloorGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.checkFloorGroup.AutoSize = true;
            this.checkFloorGroup.Location = new System.Drawing.Point(707, 157);
            this.checkFloorGroup.Name = "checkFloorGroup";
            this.checkFloorGroup.Size = new System.Drawing.Size(15, 14);
            this.checkFloorGroup.TabIndex = 5;
            this.checkFloorGroup.UseVisualStyleBackColor = true;
            this.checkFloorGroup.CheckedChanged += new System.EventHandler(this.checkFloorGroup_CheckedChanged);
            // 
            // locWallParam
            // 
            this.locWallParam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.locWallParam.Location = new System.Drawing.Point(754, 43);
            this.locWallParam.Name = "locWallParam";
            this.locWallParam.Size = new System.Drawing.Size(121, 20);
            this.locWallParam.TabIndex = 7;
            this.locWallParam.Text = "Состав";
            // 
            // FinishForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1113, 654);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FinishForm";
            this.Text = "FinishForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox RoomNames;
        private System.Windows.Forms.CheckBox SomeLevels;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chkSplitLevel;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox LocFinSelector;
        private System.Windows.Forms.ComboBox ColumnFinSelector;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox PhaseSelector;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkCol;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox modelFloor;
        private System.Windows.Forms.CheckBox modelCeil;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label SelNum;
        private System.Windows.Forms.CheckBox checkGroup;
        private System.Windows.Forms.ComboBox GroupSelector;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox GroupFloorSelector;
        private System.Windows.Forms.CheckBox checkFloorGroup;
        private System.Windows.Forms.TextBox locWallParam;
    }
}