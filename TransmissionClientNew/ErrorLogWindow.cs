// transmission-remote-dotnet
// http://code.google.com/p/transmission-remote-dotnet/
// Copyright (C) 2009 Alan F
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TransmissionRemoteDotnet.Commmands;

namespace TransmissionRemoteDotnet
{
    public partial class ErrorLogWindow : Form
    {
        private ErrorsListViewColumnSorter lvwColumnSorter;

        private EventHandler onErrorDelegate;

        private ErrorLogWindow()
        {
            Program.OnError += onErrorDelegate = new EventHandler(this.OnError);
            InitializeComponent();
            errorListView.ListViewItemSorter = lvwColumnSorter = new ErrorsListViewColumnSorter();
        }

        private void ErrorLogWindow_Load(object sender, EventArgs e)
        {
            errorListView.SuspendLayout();
            lock (Program.LogItems)
            {
                lock (errorListView)
                {
                    foreach (ListViewItem item in Program.LogItems)
                    {
                        errorListView.Items.Add((ListViewItem)item.Clone());
                    }
                }
            }
            errorListView.Sort();
            Toolbox.StripeListView(errorListView);
            errorListView.ResumeLayout();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lock (errorListView)
            {
                errorListView.Items.Clear();
            }
            lock (Program.LogItems)
            {
                Program.LogItems.Clear();
            }
        }

        private void ErrorLogWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Program.OnError -= onErrorDelegate;
        }

        private void OnError(object sender, EventArgs e)
        {
            errorListView.SuspendLayout();
            lock (Program.LogItems)
            {
                lock (errorListView)
                {
                    List<ListViewItem> logItems = Program.LogItems;
                    if (logItems.Count > errorListView.Items.Count)
                    {
                        for (int i = errorListView.Items.Count; i < logItems.Count; i++)
                        {
                            errorListView.Items.Add((ListViewItem)logItems[i].Clone());
                        }
                    }
                }
            }
            errorListView.Sort();
            Toolbox.StripeListView(errorListView);
            errorListView.ResumeLayout();
        }

        private void errorListView_DoubleClick(object sender, EventArgs e)
        {
            lock (errorListView)
            {
                if (errorListView.SelectedItems.Count == 1)
                {
                    Clipboard.SetText(errorListView.SelectedItems[0].SubItems[2].Text);
                }
            }
        }

        private void errorListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                lvwColumnSorter.Order = (lvwColumnSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending);
            }
            else
            {
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }
            this.errorListView.Sort();
            Toolbox.StripeListView(errorListView);
        }
    }
}
