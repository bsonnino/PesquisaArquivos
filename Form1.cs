using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WindowsApplication3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string NomeArq;

            if (textBox1.Text != "")
            {
                NomeArq = Path.GetFileName(textBox1.Text);
                if (NomeArq == "")
                    NomeArq = "*.*";
                NomeArq = Path.Combine(Path.GetDirectoryName(textBox1.Text), NomeArq);
            }
            else
            {
                NomeArq = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "*.*");
            }
            listView1.BeginUpdate();
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                try
                {
                    listView1.Items.Clear();
                    PesquisaArquivos(NomeArq);

                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
            finally
            {
                listView1.EndUpdate();
            }
            statusStrip1.Items[0].Text = listView1.Items.Count.ToString() + " files found";
        }

        private void PesquisaArquivos(string NomeDir)
        {
            System.IO.DirectoryInfo DirInfo;
            System.IO.FileInfo[] AFileInfo;
            System.IO.DirectoryInfo[] ADirInfo;
            ListViewItem ListItem;

            statusStrip1.Items[0].Text = "Processing " + Path.GetDirectoryName(NomeDir);
            Application.DoEvents();
            // cria instância de DirectoryInfo para o diretório selecionado
            DirInfo = new DirectoryInfo(Path.GetDirectoryName(NomeDir));
            try
            {
                // obtém arquivos do diretório
                AFileInfo = DirInfo.GetFiles(Path.GetFileName(NomeDir));
                // processa arquivos, adicionando-os na ListView
                foreach (FileInfo FilInfo in AFileInfo)
                {
                    ListItem = listView1.Items.Add(FilInfo.FullName);
                    ListItem.SubItems.Add(FilInfo.Length.ToString());
                    ListItem.SubItems.Add(FilInfo.CreationTime.ToString());
                    ListItem.SubItems.Add(FilInfo.LastAccessTime.ToString());
                    ListItem.SubItems.Add(FilInfo.LastWriteTime.ToString());
                    ListItem.SubItems.Add(FilInfo.DirectoryName);
                }
                // procura subdiretórios
                ADirInfo = DirInfo.GetDirectories();
                // chama função recursivamente
                foreach (DirectoryInfo DirecInfo in ADirInfo)
                    PesquisaArquivos(DirecInfo.FullName + "\\" + Path.GetFileName(NomeDir));
            }
            catch
            {
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult rc = System.Windows.Forms.MessageBox.Show("Confirm delete of checked files ?",
                 "Delete checked files", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rc == DialogResult.Yes)
            {
                foreach (ListViewItem ListItem in listView1.Items)
                {
                    if (ListItem.Checked)
                        File.Delete(ListItem.Text);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.BeginUpdate();
            try
            {
                foreach (ListViewItem ListItem in listView1.Items)
                    ListItem.Checked = true;
            }
            finally
            {
                listView1.EndUpdate();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (ListViewItem item in listView1.Items)
            {
                sb.AppendLine(item.SubItems[0].Text + '\t' + item.SubItems[1].Text + '\t' +
                    item.SubItems[2].Text + '\t' + item.SubItems[3].Text);
            }
            using (SaveFileDialog dialog = new SaveFileDialog()) 
            { 
                if (dialog.ShowDialog(this) == DialogResult.OK) 
                { 
                    File.WriteAllText(dialog.FileName, sb.ToString()); 
                } 
            }
        }
    }
}