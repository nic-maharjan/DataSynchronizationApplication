namespace DataSynchronizationApplication
{
	partial class MainForm
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.TextBox IntervalTextBox;
		private System.Windows.Forms.Button ManualSyncButton;
		private System.Windows.Forms.Button StartSyncButton;
		private System.Windows.Forms.DataGridView CustomerDataGridView;

		private void InitializeComponent()
		{
			this.IntervalTextBox = new System.Windows.Forms.TextBox();
			this.ManualSyncButton = new System.Windows.Forms.Button();
			this.StartSyncButton = new System.Windows.Forms.Button();
			this.CustomerDataGridView = new System.Windows.Forms.DataGridView();
			((System.ComponentModel.ISupportInitialize)(this.CustomerDataGridView)).BeginInit();
			this.SuspendLayout();
			// 
			// IntervalTextBox
			// 
			this.IntervalTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.IntervalTextBox.Location = new System.Drawing.Point(152, 47);
			this.IntervalTextBox.Multiline = true;
			this.IntervalTextBox.Name = "IntervalTextBox";
			this.IntervalTextBox.Size = new System.Drawing.Size(270, 41);
			this.IntervalTextBox.TabIndex = 0;
			// 
			// ManualSyncButton
			// 
			this.ManualSyncButton.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.ManualSyncButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.ManualSyncButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.ManualSyncButton.Location = new System.Drawing.Point(139, 110);
			this.ManualSyncButton.Name = "ManualSyncButton";
			this.ManualSyncButton.Size = new System.Drawing.Size(93, 30);
			this.ManualSyncButton.TabIndex = 1;
			this.ManualSyncButton.Text = "Manual Sync";
			this.ManualSyncButton.UseVisualStyleBackColor = false;
			this.ManualSyncButton.Click += new System.EventHandler(this.ManualSyncButton_Click);
			// 
			// StartSyncButton
			// 
			this.StartSyncButton.BackColor = System.Drawing.SystemColors.ControlDark;
			this.StartSyncButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.StartSyncButton.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.StartSyncButton.Location = new System.Drawing.Point(322, 110);
			this.StartSyncButton.Name = "StartSyncButton";
			this.StartSyncButton.Size = new System.Drawing.Size(128, 30);
			this.StartSyncButton.TabIndex = 2;
			this.StartSyncButton.Text = "Auto Sync";
			this.StartSyncButton.UseVisualStyleBackColor = false;
			this.StartSyncButton.Click += new System.EventHandler(this.StartSyncButton_Click);
			// 
			// CustomerDataGridView
			// 
			this.CustomerDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.CustomerDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.CustomerDataGridView.ColumnHeadersHeight = 29;
			this.CustomerDataGridView.Location = new System.Drawing.Point(2, 157);
			this.CustomerDataGridView.Name = "CustomerDataGridView";
			this.CustomerDataGridView.ReadOnly = true;
			this.CustomerDataGridView.RowHeadersWidth = 51;
			this.CustomerDataGridView.Size = new System.Drawing.Size(586, 250);
			this.CustomerDataGridView.TabIndex = 3;
			// 
			// MainForm
			// 
			this.ClientSize = new System.Drawing.Size(600, 500);
			this.Controls.Add(this.IntervalTextBox);
			this.Controls.Add(this.ManualSyncButton);
			this.Controls.Add(this.StartSyncButton);
			this.Controls.Add(this.CustomerDataGridView);
			this.Name = "MainForm";
			this.Text = "MSSQL to SQLite Sync";
			((System.ComponentModel.ISupportInitialize)(this.CustomerDataGridView)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
	}

}

