using System.Windows.Forms;

namespace Attempt2
{
    partial class Form1
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.button7 = new System.Windows.Forms.Button();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button10 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.button8 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.button6);
            this.groupBox3.Controls.Add(this.checkBox4);
            this.groupBox3.Controls.Add(this.button7);
            this.groupBox3.Controls.Add(this.textBox7);
            this.groupBox3.Controls.Add(this.textBox8);
            this.groupBox3.Location = new System.Drawing.Point(883, 592);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(768, 483);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Channel4";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(173, 161);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(164, 51);
            this.button6.TabIndex = 20;
            this.button6.Tag = "4;2;setAmplitude";
            this.button6.Text = "Set Amplitude";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(21, 58);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(106, 29);
            this.checkBox4.TabIndex = 4;
            this.checkBox4.Tag = "4";
            this.checkBox4.Text = "On/Off";
            this.checkBox4.UseVisualStyleBackColor = true;
            this.checkBox4.CheckedChanged += new System.EventHandler(this.OnOffSwitch);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(173, 101);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(164, 51);
            this.button7.TabIndex = 19;
            this.button7.Tag = "4;1;setVoltage";
            this.button7.Text = "Set Voltage";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button);
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(30, 172);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(100, 31);
            this.textBox7.TabIndex = 18;
            this.textBox7.Tag = "4;2";
            this.textBox7.Text = "0";
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(30, 114);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(100, 31);
            this.textBox8.TabIndex = 17;
            this.textBox8.Tag = "4;1";
            this.textBox8.Text = "0";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button4);
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Controls.Add(this.checkBox3);
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Controls.Add(this.textBox6);
            this.groupBox4.Location = new System.Drawing.Point(75, 592);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(768, 483);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Channel3";
            this.groupBox4.Enter += new System.EventHandler(this.groupBox4_Enter);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(177, 168);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(164, 51);
            this.button4.TabIndex = 16;
            this.button4.Tag = "3;2;setAmplitude";
            this.button4.Text = "Set Amplitude";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(177, 108);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(164, 51);
            this.button5.TabIndex = 15;
            this.button5.Tag = "3;1;setVoltage";
            this.button5.Text = "Set Voltage";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(25, 58);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(106, 29);
            this.checkBox3.TabIndex = 3;
            this.checkBox3.Tag = "3";
            this.checkBox3.Text = "On/Off";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.OnOffSwitch);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(34, 179);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 31);
            this.textBox5.TabIndex = 14;
            this.textBox5.Tag = "3;2";
            this.textBox5.Text = "0";
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(34, 121);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(100, 31);
            this.textBox6.TabIndex = 13;
            this.textBox6.Tag = "3;1";
            this.textBox6.Text = "0";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Controls.Add(this.textBox4);
            this.groupBox2.Controls.Add(this.checkBox2);
            this.groupBox2.Location = new System.Drawing.Point(882, 72);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(768, 483);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Channel2";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(174, 164);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(164, 51);
            this.button1.TabIndex = 12;
            this.button1.Tag = "1;2;setAmplitude";
            this.button1.Text = "Set Amplitude";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(174, 104);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(164, 51);
            this.button3.TabIndex = 11;
            this.button3.Tag = "2;1;setVoltage";
            this.button3.Text = "Set Voltage";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(31, 175);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 31);
            this.textBox3.TabIndex = 10;
            this.textBox3.Tag = "2;2";
            this.textBox3.Text = "0";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(31, 117);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 31);
            this.textBox4.TabIndex = 9;
            this.textBox4.Tag = "2;1";
            this.textBox4.Text = "0";
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(22, 51);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(106, 29);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Tag = "2";
            this.checkBox2.Text = "On/Off";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.OnOffSwitch);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox5);
            this.groupBox1.Controls.Add(this.button8);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(74, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(768, 483);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Channel1";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.textBox12);
            this.groupBox5.Controls.Add(this.button10);
            this.groupBox5.Controls.Add(this.button9);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.textBox11);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.textBox10);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.textBox9);
            this.groupBox5.Controls.Add(this.checkedListBox2);
            this.groupBox5.Controls.Add(this.checkedListBox1);
            this.groupBox5.Location = new System.Drawing.Point(0, 252);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(768, 231);
            this.groupBox5.TabIndex = 13;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Incrementation";
            this.groupBox5.Enter += new System.EventHandler(this.groupBox5_Enter);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(515, 135);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(158, 64);
            this.button10.TabIndex = 9;
            this.button10.Tag = "1";
            this.button10.Text = "STOP";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(346, 135);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(158, 64);
            this.button9.TabIndex = 8;
            this.button9.Tag = "1";
            this.button9.Text = "START";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.startButton);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(516, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 25);
            this.label4.TabIndex = 7;
            this.label4.Text = "Increment";
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(518, 67);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(100, 31);
            this.textBox11.TabIndex = 6;
            this.textBox11.Tag = "1";
            this.textBox11.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(401, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Set End";
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(403, 67);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(100, 31);
            this.textBox10.TabIndex = 4;
            this.textBox10.Tag = "1";
            this.textBox10.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(286, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 25);
            this.label1.TabIndex = 3;
            this.label1.Text = "Set Start";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(288, 67);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(100, 31);
            this.textBox9.TabIndex = 2;
            this.textBox9.Tag = "1";
            this.textBox9.Text = "0";
            // 
            // checkedListBox2
            // 
            this.checkedListBox2.FormattingEnabled = true;
            this.checkedListBox2.Items.AddRange(new object[] {
            "Increase",
            "Decrease"});
            this.checkedListBox2.Location = new System.Drawing.Point(26, 135);
            this.checkedListBox2.Name = "checkedListBox2";
            this.checkedListBox2.Size = new System.Drawing.Size(246, 88);
            this.checkedListBox2.TabIndex = 1;
            this.checkedListBox2.Tag = "1";
            this.checkedListBox2.SelectedIndexChanged += new System.EventHandler(this.increaseDecreaseButton);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "Change Voltage",
            "Change Amplitude"});
            this.checkedListBox1.Location = new System.Drawing.Point(26, 42);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(246, 88);
            this.checkedListBox1.TabIndex = 0;
            this.checkedListBox1.Tag = "1";
            this.checkedListBox1.SelectedIndexChanged += new System.EventHandler(this.chooseVariableButton);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(189, 117);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(164, 51);
            this.button8.TabIndex = 12;
            this.button8.Tag = "1;1;setVoltage";
            this.button8.Text = "Set Voltage";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(189, 182);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(164, 51);
            this.button2.TabIndex = 8;
            this.button2.Tag = "1;2;setAmplitude";
            this.button2.Text = "Set Amplitude";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(26, 51);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(106, 29);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Tag = "1";
            this.checkBox1.Text = "On/Off";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.OnOffSwitch);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(46, 182);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 31);
            this.textBox2.TabIndex = 5;
            this.textBox2.Tag = "1;2";
            this.textBox2.Text = "0";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(46, 124);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 31);
            this.textBox1.TabIndex = 4;
            this.textBox1.Tag = "1;1";
            this.textBox1.Text = "0";
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 25);
            this.label2.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(634, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 25);
            this.label5.TabIndex = 11;
            this.label5.Text = "Constant";
            // 
            // textBox12
            // 
            this.textBox12.Location = new System.Drawing.Point(636, 67);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(100, 31);
            this.textBox12.TabIndex = 10;
            this.textBox12.Tag = "1";
            this.textBox12.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1690, 1104);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Hello World";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.CheckedListBox checkedListBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox10;
        private Button button10;
        private Label label5;
        private TextBox textBox12;
    }
}

