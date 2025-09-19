namespace MxComponentForm
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Open = new Button();
            Close = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            GetDevice = new Button();
            Value = new Label();
            SetDevice = new Button();
            textBox2 = new TextBox();
            label2 = new Label();
            ReadDeviceBlock = new Button();
            label3 = new Label();
            textBox3 = new TextBox();
            Value2 = new Label();
            textBox4 = new TextBox();
            label5 = new Label();
            WriteDeviceBlock = new Button();
            label4 = new Label();
            textBox5 = new TextBox();
            SuspendLayout();
            // 
            // Open
            // 
            Open.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            Open.Location = new Point(25, 24);
            Open.Name = "Open";
            Open.Size = new Size(123, 53);
            Open.TabIndex = 0;
            Open.Text = "Open";
            Open.UseVisualStyleBackColor = true;
            Open.Click += Open_Click;
            // 
            // Close
            // 
            Close.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            Close.Location = new Point(164, 24);
            Close.Name = "Close";
            Close.Size = new Size(123, 53);
            Close.TabIndex = 1;
            Close.Text = "Close";
            Close.UseVisualStyleBackColor = true;
            Close.Click += Close_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(25, 107);
            label1.Name = "label1";
            label1.Size = new Size(150, 15);
            label1.TabIndex = 2;
            label1.Text = "Device 주소를 넣어주세요.";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(25, 125);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(123, 23);
            textBox1.TabIndex = 3;
            // 
            // GetDevice
            // 
            GetDevice.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            GetDevice.Location = new Point(164, 125);
            GetDevice.Name = "GetDevice";
            GetDevice.Size = new Size(123, 53);
            GetDevice.TabIndex = 4;
            GetDevice.Text = "GetDevice";
            GetDevice.UseVisualStyleBackColor = true;
            GetDevice.Click += GetDevice_Click;
            // 
            // Value
            // 
            Value.AutoSize = true;
            Value.Location = new Point(25, 223);
            Value.Name = "Value";
            Value.Size = new Size(0, 15);
            Value.TabIndex = 5;
            // 
            // SetDevice
            // 
            SetDevice.Font = new Font("맑은 고딕", 12F, FontStyle.Bold);
            SetDevice.Location = new Point(293, 125);
            SetDevice.Name = "SetDevice";
            SetDevice.Size = new Size(123, 53);
            SetDevice.TabIndex = 6;
            SetDevice.Text = "SetDevice";
            SetDevice.UseVisualStyleBackColor = true;
            SetDevice.Click += SetDevice_Click;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(25, 182);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(123, 23);
            textBox2.TabIndex = 7;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(25, 159);
            label2.Name = "label2";
            label2.Size = new Size(114, 15);
            label2.TabIndex = 8;
            label2.Text = "값을 입력해 주세요.";
            // 
            // ReadDeviceBlock
            // 
            ReadDeviceBlock.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            ReadDeviceBlock.Location = new Point(164, 280);
            ReadDeviceBlock.Name = "ReadDeviceBlock";
            ReadDeviceBlock.Size = new Size(123, 53);
            ReadDeviceBlock.TabIndex = 9;
            ReadDeviceBlock.Text = "ReadDeviceBlock";
            ReadDeviceBlock.UseVisualStyleBackColor = true;
            ReadDeviceBlock.Click += ReadDeviceBlock_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(25, 314);
            label3.Name = "label3";
            label3.Size = new Size(138, 15);
            label3.TabIndex = 14;
            label3.Text = "블록개수 입력해 주세요.";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(25, 280);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(123, 23);
            textBox3.TabIndex = 13;
            // 
            // Value2
            // 
            Value2.AutoSize = true;
            Value2.Location = new Point(175, 349);
            Value2.Name = "Value2";
            Value2.Size = new Size(0, 15);
            Value2.TabIndex = 12;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(25, 332);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(123, 23);
            textBox4.TabIndex = 11;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(25, 262);
            label5.Name = "label5";
            label5.Size = new Size(150, 15);
            label5.TabIndex = 10;
            label5.Text = "Device 주소를 넣어주세요.";
            // 
            // WriteDeviceBlock
            // 
            WriteDeviceBlock.Font = new Font("맑은 고딕", 9F, FontStyle.Bold);
            WriteDeviceBlock.Location = new Point(293, 280);
            WriteDeviceBlock.Name = "WriteDeviceBlock";
            WriteDeviceBlock.Size = new Size(123, 53);
            WriteDeviceBlock.TabIndex = 15;
            WriteDeviceBlock.Text = "WriteDeviceBlock";
            WriteDeviceBlock.UseVisualStyleBackColor = true;
            WriteDeviceBlock.Click += WriteDeviceBlock_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(25, 369);
            label4.Name = "label4";
            label4.Size = new Size(114, 15);
            label4.TabIndex = 17;
            label4.Text = "값을 입력해 주세요.";
            // 
            // textBox5
            // 
            textBox5.Location = new Point(25, 387);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(123, 23);
            textBox5.TabIndex = 16;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(446, 459);
            Controls.Add(label4);
            Controls.Add(textBox5);
            Controls.Add(WriteDeviceBlock);
            Controls.Add(label3);
            Controls.Add(textBox3);
            Controls.Add(Value2);
            Controls.Add(textBox4);
            Controls.Add(label5);
            Controls.Add(ReadDeviceBlock);
            Controls.Add(label2);
            Controls.Add(textBox2);
            Controls.Add(SetDevice);
            Controls.Add(Value);
            Controls.Add(GetDevice);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(Close);
            Controls.Add(Open);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Open;
        private Button Close;
        private Label label1;
        private TextBox textBox1;
        private Button GetDevice;
        private Label Value;
        private Button SetDevice;
        private TextBox textBox2;
        private Label label2;
        private Button ReadDeviceBlock;
        private Label label3;
        private TextBox textBox3;
        private Label Value2;
        private TextBox textBox4;
        private Label label5;
        private Button WriteDeviceBlock;
        private Label label4;
        private TextBox textBox5;
    }
}
