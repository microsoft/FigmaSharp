namespace FigmaDocumentExporter.WinForms
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
            if (disposing && (components != null)) {
                components.Dispose ();
            }
            base.Dispose (disposing);
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
            this.documentTextField = new System.Windows.Forms.TextBox();
            this.outputPathTextField = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "FileID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 20);
            this.label2.TabIndex = 1;
            this.label2.Text = "Output Directory";
            // 
            // documentTextField
            // 
            this.documentTextField.Location = new System.Drawing.Point(170, 13);
            this.documentTextField.Name = "documentTextField";
            this.documentTextField.Size = new System.Drawing.Size(382, 26);
            this.documentTextField.TabIndex = 2;
            // 
            // outputPathTextField
            // 
            this.outputPathTextField.Location = new System.Drawing.Point(170, 58);
            this.outputPathTextField.Name = "outputPathTextField";
            this.outputPathTextField.Size = new System.Drawing.Size(382, 26);
            this.outputPathTextField.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(300, 107);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(252, 35);
            this.button1.TabIndex = 4;
            this.button1.Text = "Export";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(591, 166);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.outputPathTextField);
            this.Controls.Add(this.documentTextField);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox documentTextField;
        private System.Windows.Forms.TextBox outputPathTextField;
        private System.Windows.Forms.Button button1;
    }
}

