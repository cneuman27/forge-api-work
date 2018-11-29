using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;

using Infragistics.Windows.DataPresenter;

namespace ForgeTest.LogUI
{
    public partial class CFrmLogDetails : MOMShared.CMOMForm
    {
        private bool m_InProcess = true;
        private ForgeAPI.Interface.REST.IResult m_Result = null;

        public CFrmLogDetails()
        {
            InitializeComponent();
        }

        protected override void DoButtons()
        {
            if (m_InProcess == true)
            {
                txtURI.DisableControl();
                txtResponseData.DisableControl();
                txtRequestData.DisableControl();
            }
            else
            {
                txtURI.EnableControl();
                txtResponseData.EnableControl();
                txtRequestData.EnableControl();
            }
        }

        private void DoLoad(object state)
        {
            ObservableCollection<Types.CHeaderWrapper> col;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Loading");
                lblStatus.ShowStatus();

                txtURI.SetText(
                    $"[{((int)m_Result.StatusCode)}] {m_Result.Method} {m_Result.URI}");

                if (string.IsNullOrWhiteSpace(m_Result.ResponseData) == false)
                {
                    txtResponseData.SetText(FormatJSON(m_Result.ResponseData));
                }

                col = new ObservableCollection<Types.CHeaderWrapper>();
                m_Result.ResponseHeaders.ForEach(i => col.Add(new Types.CHeaderWrapper(i)));
                gridResponseHeaders.SetDataSource(col);

                if (string.IsNullOrWhiteSpace(m_Result.RequestData) == false)
                {
                    txtRequestData.SetText(FormatJSON(m_Result.RequestData));
                }

                col = new ObservableCollection<Types.CHeaderWrapper>();
                m_Result.RequestHeaders.ForEach(i => col.Add(new Types.CHeaderWrapper(i)));
                gridRequestHeaders.SetDataSource(col);
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                ShowError(e);
            }
            finally
            {
                lblStatus.CollapseStatus();

                m_InProcess = false;
                DoButtons();

            }
        }

        private string FormatJSON(string txt)
        {
            try
            {
                return Newtonsoft.Json.Linq.JValue.Parse(txt).ToString(Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception)
            {
                return txt;
            }
        }

        public void Start(
            Window parent,
            ForgeAPI.Interface.REST.IResult result)
        {
            m_Result = result;

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoLoad));

            Owner = parent;
            ShowDialog();
        }

        #region Control Events

        private void gridResponseHeaders_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            foreach (Record tmp in gridResponseHeaders.SelectedItems.Records)
            {
                if (tmp.RecordType == RecordType.DataRecord &&
                    (tmp as DataRecord).DataItem is MOMShared.CNotifier)
                {
                    ((tmp as DataRecord).DataItem as MOMShared.CNotifier).NotifyAll();
                }
            }

            DoButtons();
        }
        private void gridRequestHeaders_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            foreach (Record tmp in gridRequestHeaders.SelectedItems.Records)
            {
                if (tmp.RecordType == RecordType.DataRecord &&
                    (tmp as DataRecord).DataItem is MOMShared.CNotifier)
                {
                    ((tmp as DataRecord).DataItem as MOMShared.CNotifier).NotifyAll();
                }
            }

            DoButtons();
        }

        #endregion
    }
}
