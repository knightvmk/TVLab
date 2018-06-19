namespace TV_Lab
{
    partial class P_FORM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(P_FORM));
            this.P_TABLE = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.P_TABLE)).BeginInit();
            this.SuspendLayout();
            // 
            // P_TABLE
            // 
            this.P_TABLE.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.P_TABLE.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.P_TABLE.BackgroundColor = System.Drawing.SystemColors.ActiveCaption;
            this.P_TABLE.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.P_TABLE.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.P_TABLE.GridColor = System.Drawing.SystemColors.ActiveCaption;
            this.P_TABLE.Location = new System.Drawing.Point(3, 4);
            this.P_TABLE.Name = "P_TABLE";
            this.P_TABLE.Size = new System.Drawing.Size(886, 448);
            this.P_TABLE.TabIndex = 0;
            // 
            // P_FORM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(892, 456);
            this.Controls.Add(this.P_TABLE);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "P_FORM";
            this.Text = "ТВиМС | Таблица вероятностей случайных величин";
            ((System.ComponentModel.ISupportInitialize)(this.P_TABLE)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView P_TABLE;
    }
}