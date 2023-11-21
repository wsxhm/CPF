using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace ConsoleApp1
{
    /// <summary>
    /// Represents the method that handles the <see cref="SqliteDataAdapter.RowUpdated">RowUpdated</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="SqliteRowUpdatedEventArgs">SqliteRowUpdatedEventArgs</see> that contains the event data.</param>
    public delegate void SqliteRowUpdatedEventHandler(Object sender, SqliteRowUpdatedEventArgs e);

    /// <summary>
    /// Represents the method that handles the <see cref="SqliteDataAdapter.RowUpdating">RowUpdating</see> events.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A <see cref="SqliteRowUpdatingEventArgs">SqliteRowUpdatingEventArgs</see> that contains the event data.</param>
    public delegate void SqliteRowUpdatingEventHandler(Object sender, SqliteRowUpdatingEventArgs e);

    /// <summary>
    /// This class represents an adapter from many commands: select, update, insert and delete to fill <see cref="System.Data.DataSet">Datasets.</see>
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public sealed class SqliteDataAdapter : DbDataAdapter
    {
        /// <summary>
        /// Row updated event.
        /// </summary>
        public event SqliteRowUpdatedEventHandler RowUpdated;

        /// <summary>
        /// Row updating event.
        /// </summary>
        public event SqliteRowUpdatingEventHandler RowUpdating;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SqliteDataAdapter() { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectCommand"></param>
        public SqliteDataAdapter(SqliteCommand selectCommand)
        {
            SelectCommand = selectCommand;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectCommandText"></param>
        /// <param name="selectConnection"></param>
        public SqliteDataAdapter(string selectCommandText, SqliteConnection selectConnection)
            : this(new SqliteCommand(selectCommandText, selectConnection)) { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="selectCommandText"></param>
        /// <param name="selectConnectionString"></param>
        public SqliteDataAdapter(string selectCommandText, string selectConnectionString)
            : this(selectCommandText, new SqliteConnection(selectConnectionString)) { }

        /// <summary>
        /// Create row updated event.
        /// </summary>
        protected override RowUpdatedEventArgs CreateRowUpdatedEvent(DataRow dataRow, IDbCommand command,
                                                                     System.Data.StatementType statementType,
                                                                      DataTableMapping tableMapping)
        {
            return new SqliteRowUpdatedEventArgs(dataRow, command, statementType, tableMapping);
        }

        /// <summary>
        /// Create row updating event.
        /// </summary>
        protected override RowUpdatingEventArgs CreateRowUpdatingEvent(DataRow dataRow, IDbCommand command,
                                                                       System.Data.StatementType statementType,
                                                                        DataTableMapping tableMapping)
        {
            return new SqliteRowUpdatingEventArgs(dataRow, command, statementType, tableMapping);
        }

        /// <summary>
        /// Raise the RowUpdated event.
        /// </summary>
        /// <param name="value"></param>
        protected override void OnRowUpdated(RowUpdatedEventArgs value)
        {
            //base.OnRowUpdated(value);
            if (RowUpdated != null && value is SqliteRowUpdatedEventArgs)
                RowUpdated(this, (SqliteRowUpdatedEventArgs)value);
        }

        /// <summary>
        /// Raise the RowUpdating event.
        /// </summary>
        /// <param name="value"></param>
        protected override void OnRowUpdating(RowUpdatingEventArgs value)
        {
            if (RowUpdating != null && value is SqliteRowUpdatingEventArgs)
                RowUpdating(this, (SqliteRowUpdatingEventArgs)value);
        }

        /// <summary>
        /// Delete command.
        /// </summary>
        public new SqliteCommand DeleteCommand
        {
            get { return (SqliteCommand)base.DeleteCommand; }
            set { base.DeleteCommand = value; }
        }

        /// <summary>
        /// Select command.
        /// </summary>
        public new SqliteCommand SelectCommand
        {
            get { return (SqliteCommand)base.SelectCommand; }
            set { base.SelectCommand = value; }
        }

        /// <summary>
        /// Update command.
        /// </summary>
        public new SqliteCommand UpdateCommand
        {
            get { return (SqliteCommand)base.UpdateCommand; }
            set { base.UpdateCommand = value; }
        }

        /// <summary>
        /// Insert command.
        /// </summary>
        public new SqliteCommand InsertCommand
        {
            get { return (SqliteCommand)base.InsertCommand; }
            set { base.InsertCommand = value; }
        }
    }

#pragma warning disable 1591

    public class SqliteRowUpdatingEventArgs : RowUpdatingEventArgs
    {
        public SqliteRowUpdatingEventArgs(DataRow dataRow, IDbCommand command, System.Data.StatementType statementType,
                                          DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) { }
    }

    public class SqliteRowUpdatedEventArgs : RowUpdatedEventArgs
    {
        public SqliteRowUpdatedEventArgs(DataRow dataRow, IDbCommand command, System.Data.StatementType statementType,
                                         DataTableMapping tableMapping)
            : base(dataRow, command, statementType, tableMapping) { }
    }

#pragma warning restore 1591
}
