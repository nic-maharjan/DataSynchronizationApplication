using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Timers;
using System.Windows.Forms;

namespace DataSynchronizationApplication
{
	public partial class MainForm : Form
	{
		private string mssqlConnectionString = "Server=DESKTOP-8AO3K2J;Database=SyncDb;Integrated Security=True;";
		private string sqliteConnectionString = "Data Source=D:/DataSyncProject/DataSynchronizationApplication/SyncDb.db"; // SQLite local database
		private System.Timers.Timer syncTimer;

		public MainForm()
		{
			InitializeComponent();
			SetupTimer();
		}

		private void SetupTimer()
		{
			syncTimer = new System.Timers.Timer();
			syncTimer.Elapsed += OnTimedEvent;
			syncTimer.AutoReset = true;
		}

		private void ManualSyncButton_Click(object sender, EventArgs e)
		{
			SyncData();
		}

		private void StartSyncButton_Click(object sender, EventArgs e)
		{
			int interval;
			if (int.TryParse(IntervalTextBox.Text, out interval))
			{
				syncTimer.Interval = interval * 60000; // Convert to milliseconds
				syncTimer.Start();
				MessageBox.Show("Automatic sync started!");
			}
			else
			{
				MessageBox.Show("Please enter a valid interval in minutes.");
			}
		}

		private void OnTimedEvent(Object source, ElapsedEventArgs e)
		{
			SyncData();
		}

		private void SyncData()
		{
			try
			{
				// Fetch data from MSSQL
				using (SqlConnection mssqlConn = new SqlConnection(mssqlConnectionString))
				{
					mssqlConn.Open();

					SqlCommand customerCmd = new SqlCommand("SELECT * FROM Customer", mssqlConn);
					SqlDataAdapter customerAdapter = new SqlDataAdapter(customerCmd);
					DataTable customerData = new DataTable();
					customerAdapter.Fill(customerData);

					SqlCommand locationCmd = new SqlCommand("SELECT * FROM Location", mssqlConn);
					SqlDataAdapter locationAdapter = new SqlDataAdapter(locationCmd);
					DataTable locationData = new DataTable();
					locationAdapter.Fill(locationData);

					// Sync data with SQLite
					using (SQLiteConnection sqliteConn = new SQLiteConnection(sqliteConnectionString))
					{
						sqliteConn.Open();

						// Enable WAL mode
						SQLiteCommand walCmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", sqliteConn);
						walCmd.ExecuteNonQuery();

						foreach (DataRow row in customerData.Rows)
						{
							// Check if record exists in SQLite
							SQLiteCommand checkCmd = new SQLiteCommand("SELECT * FROM Customer WHERE CustomerID = @CustomerID", sqliteConn);
							checkCmd.Parameters.AddWithValue("@CustomerID", row["CustomerID"]);
							SQLiteDataReader reader = checkCmd.ExecuteReader();
							// Check if record exists in SQLite
							SQLiteCommand checkCmdLoc = new SQLiteCommand("SELECT * FROM location WHERE CustomerID = @CustomerID", sqliteConn);
							checkCmdLoc.Parameters.AddWithValue("@CustomerID", row["CustomerID"]);
							SQLiteDataReader readerLoc = checkCmdLoc.ExecuteReader();

							if (reader.Read())
							{
								// Check for updates on Name, Email, and Phone fields
								CheckAndLogFieldChange(sqliteConn, row, reader, "Name");
								CheckAndLogFieldChange(sqliteConn, row, reader, "Email");
								CheckAndLogFieldChange(sqliteConn, row, reader, "Phone");
								
							}
							else
							{
								// Insert new record into SQLite
								SQLiteCommand insertCmd = new SQLiteCommand("INSERT INTO Customer (CustomerID, Name, Email, Phone) VALUES (@CustomerID, @Name, @Email, @Phone)", sqliteConn);
								insertCmd.Parameters.AddWithValue("@CustomerID", row["CustomerID"]);
								insertCmd.Parameters.AddWithValue("@Name", row["Name"]);
								insertCmd.Parameters.AddWithValue("@Email", row["Email"]);
								insertCmd.Parameters.AddWithValue("@Phone", row["Phone"]);
								insertCmd.ExecuteNonQuery();
							
							}
							if(readerLoc.Read())
							{
								// Check location
								CheckAndLogLocationChange(sqliteConn, row, locationData);
							}
							else
							{
								// Insert location if it exists
								InsertLocationIfExists(sqliteConn, row, locationData);
							}
						}

					}
					// Populate  // Invoke the UI update for DataGridView on the main thread
					Invoke((MethodInvoker)delegate
					{
						PopulateCustomerDataGrid(customerData, locationData);
						MessageBox.Show("Data sync complete!");
					});

				}
				

			
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Error during sync: {ex.Message}");
			}
		}
		private void PopulateCustomerDataGrid(DataTable customerData, DataTable locationData)
		{
			// Merge Customer and Location tables for display
			var mergedTable = customerData.Clone();
			mergedTable.Columns.Add("Location");

			foreach (DataRow customerRow in customerData.Rows)
			{
				foreach (DataRow locationRow in locationData.Select("CustomerID = " + customerRow["CustomerID"]))
				{
					var newRow = mergedTable.NewRow();
					newRow.ItemArray = customerRow.ItemArray;
					newRow["Location"] = locationRow["Address"];
					mergedTable.Rows.Add(newRow);
				}
			}

			// Bind to DataGridView
			CustomerDataGridView.DataSource = mergedTable;
		}
		// Helper method to check for changes in the location and log if necessary
		private void CheckAndLogLocationChange(SQLiteConnection sqliteConn, DataRow row, DataTable locationData)
		{
			var customerId = Convert.ToInt32(row["CustomerID"]);
			var locationRow = locationData.AsEnumerable().FirstOrDefault(r => r.Field<int>("CustomerID") == customerId);

			string newAddress = locationRow?["Address"].ToString() ?? string.Empty;

			// Check if the address in the SQLite is the same
			using (SQLiteCommand locationCheckCmd = new SQLiteCommand("SELECT Address FROM Location WHERE CustomerID = @CustomerID", sqliteConn))
			{
				locationCheckCmd.Parameters.AddWithValue("@CustomerID", customerId);
				string oldAddress = locationCheckCmd.ExecuteScalar()?.ToString() ?? string.Empty;

				if (oldAddress != newAddress)
				{
					using (SQLiteCommand updateLocationCmd = new SQLiteCommand("UPDATE Location SET Address = @Address WHERE CustomerID = @CustomerID", sqliteConn))
					{
						updateLocationCmd.Parameters.AddWithValue("@Address", newAddress);
						updateLocationCmd.Parameters.AddWithValue("@CustomerID", customerId);
						updateLocationCmd.ExecuteNonQuery();
					}

					// Log the location change
					LogChange(sqliteConn, customerId, "Address", oldAddress, newAddress);
				}
			}
		}

		// Helper method to insert location if it exists
		private void InsertLocationIfExists(SQLiteConnection sqliteConn, DataRow row, DataTable locationData)
		{
			var customerId =Convert.ToInt32(row["CustomerID"]);
			var locationRow = locationData.AsEnumerable().FirstOrDefault(r => r.Field<int>("CustomerID") == customerId);

			if (locationRow != null)
			{
				using (SQLiteCommand insertLocationCmd = new SQLiteCommand("INSERT INTO Location (LocationID,CustomerID, Address) VALUES (@LocationID,@CustomerID, @Address)", sqliteConn))
				{
					insertLocationCmd.Parameters.AddWithValue("@LocationID", Convert.ToInt32(locationRow["LocationID"]));
					insertLocationCmd.Parameters.AddWithValue("@CustomerID", customerId);
					insertLocationCmd.Parameters.AddWithValue("@Address", locationRow["Address"]);
					insertLocationCmd.ExecuteNonQuery();
				}
			}
		}




		private void LogChange(SQLiteConnection conn, int customerId, string field, string oldValue, string newValue)
		{
			SQLiteCommand logCmd = new SQLiteCommand(
				"INSERT INTO SyncLog (OperationType, CustomerID, FieldChanged, OldValue, NewValue, SyncTimestamp) " +
				"VALUES (@OperationType, @CustomerID, @FieldChanged, @OldValue, @NewValue, @SyncTimestamp)", conn);

			logCmd.Parameters.AddWithValue("@OperationType", "UPDATE");
			logCmd.Parameters.AddWithValue("@CustomerID", customerId);
			logCmd.Parameters.AddWithValue("@FieldChanged", field);
			logCmd.Parameters.AddWithValue("@OldValue", oldValue);
			logCmd.Parameters.AddWithValue("@NewValue", newValue);
			logCmd.Parameters.AddWithValue("@SyncTimestamp", DateTime.Now);
			logCmd.ExecuteNonQuery();
		}

		// Helper method to check for changes in a specific field and log if necessary
		private void CheckAndLogFieldChange(SQLiteConnection sqliteConn, DataRow row, SQLiteDataReader reader, string fieldName)
		{
			string oldValue = reader[fieldName].ToString();
			string newValue = row[fieldName].ToString();

			if (oldValue != newValue)
			{
				using (SQLiteCommand updateCmd = new SQLiteCommand($"UPDATE Customer SET {fieldName} = @{fieldName} WHERE CustomerID = @CustomerID", sqliteConn))
				{
					updateCmd.Parameters.AddWithValue($"@{fieldName}", newValue);
					updateCmd.Parameters.AddWithValue("@CustomerID", row["CustomerID"]);
					updateCmd.ExecuteNonQuery();
				}

				// Log the change
				LogChange(sqliteConn, Convert.ToInt32(row["CustomerID"]), fieldName, oldValue, newValue);
			}
		}
	}
}
