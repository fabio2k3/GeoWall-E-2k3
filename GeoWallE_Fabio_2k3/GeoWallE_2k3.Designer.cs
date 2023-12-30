namespace GeoWallE_Fabio_2k3
{
    partial class GeoWallE_2k3
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
            button1 = new Button();
            rtb_Code = new RichTextBox();
            pctBox_Draw = new PictureBox();
            rtb_Error = new RichTextBox();
            button2 = new Button();
            ((System.ComponentModel.ISupportInitialize)pctBox_Draw).BeginInit();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Font = new Font("Stencil", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            button1.Location = new System.Drawing.Point(23, 12);
            button1.Name = "button1";
            button1.Size = new Size(221, 54);
            button1.TabIndex = 0;
            button1.Text = "Compile && Run";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // rtb_Code
            // 
            rtb_Code.Location = new System.Drawing.Point(23, 81);
            rtb_Code.Name = "rtb_Code";
            rtb_Code.Size = new Size(602, 473);
            rtb_Code.TabIndex = 2;
            rtb_Code.Text = "";
            // 
            // pctBox_Draw
            // 
            pctBox_Draw.BackColor = SystemColors.ActiveBorder;
            pctBox_Draw.Location = new System.Drawing.Point(651, 12);
            pctBox_Draw.Name = "pctBox_Draw";
            pctBox_Draw.Size = new Size(779, 681);
            pctBox_Draw.TabIndex = 3;
            pctBox_Draw.TabStop = false;
            // 
            // rtb_Error
            // 
            rtb_Error.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            rtb_Error.Location = new System.Drawing.Point(23, 573);
            rtb_Error.Name = "rtb_Error";
            rtb_Error.Size = new Size(602, 120);
            rtb_Error.TabIndex = 4;
            rtb_Error.Text = "";
            // 
            // button2
            // 
            button2.Font = new Font("Stencil", 13.8F, FontStyle.Bold, GraphicsUnit.Point);
            button2.Location = new System.Drawing.Point(475, 14);
            button2.Name = "button2";
            button2.Size = new Size(150, 54);
            button2.TabIndex = 5;
            button2.Text = "Exit";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // GeoWallE_2k3
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaptionText;
            ClientSize = new Size(1442, 705);
            Controls.Add(button2);
            Controls.Add(rtb_Error);
            Controls.Add(pctBox_Draw);
            Controls.Add(rtb_Code);
            Controls.Add(button1);
            Name = "GeoWallE_2k3";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pctBox_Draw).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button button1;
        private RichTextBox rtb_Code;
        private PictureBox pctBox_Draw;
        private RichTextBox rtb_Error;
        private Button button2;
    }
}