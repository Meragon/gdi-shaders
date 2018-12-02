namespace GdiShaders
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.checkBoxFixedStep = new System.Windows.Forms.CheckBox();
            this.shaderRenderer1 = new GdiShaders.ShaderRenderer();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(146, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(186, 212);
            this.listBox1.TabIndex = 1;
            // 
            // checkBoxFixedStep
            // 
            this.checkBoxFixedStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFixedStep.AutoSize = true;
            this.checkBoxFixedStep.Location = new System.Drawing.Point(146, 232);
            this.checkBoxFixedStep.Name = "checkBoxFixedStep";
            this.checkBoxFixedStep.Size = new System.Drawing.Size(74, 17);
            this.checkBoxFixedStep.TabIndex = 2;
            this.checkBoxFixedStep.Text = "Fixed step";
            this.checkBoxFixedStep.UseVisualStyleBackColor = true;
            this.checkBoxFixedStep.CheckedChanged += new System.EventHandler(this.checkBoxFixedStep_CheckedChanged);
            // 
            // shaderRenderer1
            // 
            this.shaderRenderer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.shaderRenderer1.Location = new System.Drawing.Point(12, 12);
            this.shaderRenderer1.Name = "shaderRenderer1";
            this.shaderRenderer1.Size = new System.Drawing.Size(128, 128);
            this.shaderRenderer1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(344, 261);
            this.Controls.Add(this.checkBoxFixedStep);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.shaderRenderer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(200, 200);
            this.Name = "Form1";
            this.Text = "GDI+ GLSL Shaders";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ShaderRenderer shaderRenderer1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.CheckBox checkBoxFixedStep;
    }
}

