using OnlineLU.Client.Library.AzureHelper;
using OnlineLU.Client.Library.Contollers;
using OnlineLU.Client.Library.Events;
using OnlineLU.Client.Library.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnlineLU.Client.WinForms
{
    public partial class Principal : Form
    {
        private bool m_ShowSummary;
        private bool m_Recursive;
        private ProcessController m_ProcessController;
        private HardwareInfoModel m_HardwareInfo;
        private HistoryController m_HistoryController;
        private TestFileManager m_FileCreator;

        public Principal()
        {
            InitializeComponent();
            this.m_ShowSummary = this.cbSummary.Checked;
            this.m_Recursive = this.cbRecursive.Checked;
            this.SetHardwareInfo();
            this.m_HistoryController = new HistoryController();

        }

        private void SetHardwareInfo()
        {
            var _hardwareController = new HardwareController();
            this.m_HardwareInfo = _hardwareController.GetHardwareInfo();

            AddHardware("Nome: " + m_HardwareInfo.System.SystemName);
            AddHardware("CPU: " + m_HardwareInfo.System.Name);
            AddHardware("Cores: " + m_HardwareInfo.System.Cores);
            AddHardware(string.Format("Memória: {0} GB", m_HardwareInfo.System.Memory));
            foreach (var item in m_HardwareInfo.Video)
            {
                AddHardware(string.Format("Vídeo: {0} - Memória: {1}", item.Name, item.Memory));
            }
            AddHardware("HardwareKey: " + m_HardwareInfo.HardwareKey);
        }

        private void AddHardware(string text)
        {
            this.listHardware.Items.Add(text);
        }

        private void AddMessage(string message)
        {
            listProcessing.Items.Add(message);
            listProcessing.SelectedIndex = listProcessing.Items.Count - 1;
        }

        private void Principal_Load(object sender, EventArgs e)
        {
            

        }

        private void m_ProcessController_SummaryChange(object sender, SummaryEventArgs e)
        {
            if (this.InvokeRequired)
            {
                if (e != null)
                {
                    if (e.Clear)
                    {
                        if (!m_Recursive)
                        {
                            this.Invoke(new MethodInvoker(() => clearListProcessing()));
                        }
                    }
                    this.Invoke(new MethodInvoker(() => AddMessage(e.Message)));
                }
            }
            else
            {
                if (e != null)
                {
                    AddMessage(e.Message);
                }
            }
        }

        private void Activate_Click(object sender, EventArgs e)
        {
            //clearListProcessing();
            ActivateListening();

        }

        private void ActivateListening()
        {
            this.btActivate.Text = "Processando...";
            this.btActivate.Enabled = false;

            if (m_ProcessController == null)
            {
                m_ProcessController = new ProcessController();
                m_ProcessController.SetShowSummary(this.m_ShowSummary);
                m_ProcessController.SummaryChange += m_ProcessController_SummaryChange;
                m_ProcessController.FinishChange += m_ProcessController_FinishChange;
                m_ProcessController.HistoryEventChange += m_ProcessController_HistoryEventChange;
            }
            m_ProcessController.Start(cbCreate.Checked, txtRange.Text);
        }

        private void m_ProcessController_HistoryEventChange(object sender, HistoryEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    if (e.History != null)
                    {
                        e.History.HardwareInfo = this.m_HardwareInfo;
                        if (m_HistoryController.SendHistoryResult(e.History))
                        {
                            AddMessage("Resultados enviados");
                        }
                        else
                        {
                            AddMessage("Falha ao enviar resultados");
                        }
                    }
                }));
            }
        }

        private void m_ProcessController_FinishChange(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.btActivate.Text = "Activate";
                    this.btActivate.Enabled = true;
                    if (m_Recursive)
                    {
                        if (numTime.Value > 0)
                        {
                            Thread.Sleep(Convert.ToInt32(numTime.Value));
                        }
                        ActivateListening();
                    }
                }));
            }
        }

        private void btClear_Click(object sender, EventArgs e)
        {
            clearListProcessing();
        }

        private void clearListProcessing()
        {
            listProcessing.Items.Clear();
        }

        private void cbSummary_CheckedChanged(object sender, EventArgs e)
        {
            this.m_ShowSummary = (sender as CheckBox).Checked;
            if (this.m_ProcessController != null)
            {
                m_ProcessController.SetShowSummary(this.m_ShowSummary);
            }
        }

        private void cbRecursive_CheckedChanged(object sender, EventArgs e)
        {
            this.m_Recursive = this.cbRecursive.Checked;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (m_FileCreator == null)
            {
                m_FileCreator = new TestFileManager();
            }
            int _range = Convert.ToInt32(numRange.Value);
            int _precision = Convert.ToInt32(numPrecision.Value);

            m_FileCreator.CreateFilePrecision(_range, _precision, txtLocation.Text);

            MessageBox.Show("Arquivo Criado!");
            
        }

        private void numRange_ValueChanged(object sender, EventArgs e)
        {
            txtLocation.Text = string.Format("c:\\temp\\{0}.lu", Convert.ToInt32(numRange.Value));
        }

        private void btnTamanho_Click(object sender, EventArgs e)
        {
            if (m_FileCreator == null)
            {
                m_FileCreator = new TestFileManager();
            }

            m_FileCreator.VerifyFileSize(txtLocation.Text);
        }


    }
}
