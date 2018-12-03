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

using MOMControls;
using MOMShared;

namespace Run_StandardPartProcessing.APILog
{
    public partial class CFrmAPILog : CMOMForm
    {
        private bool m_InProcess = true;
        private List<ForgeAPI.Interface.REST.IResult> m_ResultList = null;

        public CFrmAPILog()
        {
            InitializeComponent();
        }

        public void Start(
            Window parent,
            List<ForgeAPI.Interface.REST.IResult> resultList)
        {
            m_ResultList = resultList;

            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoLoad));

            Owner = parent;
            ShowDialog();
        }

        protected override void DoButtons()
        {
            if (m_InProcess == true)
            {
                btnDetails.DisableControl();
            }
            else
            {
                if (gridAPILog.CountSelectedItems() > 0)
                {
                    btnDetails.EnableControl();
                }
                else
                {
                    btnDetails.DisableControl();
                }
            }
        }

        private void DoLoad(object state)
        {
            ObservableCollection<CResultWrapper> list;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Loading");
                lblStatus.ShowStatus();

                list = new ObservableCollection<CResultWrapper>();

                m_ResultList.ForEach(i => list.Add(new CResultWrapper(i)));

                gridAPILog.SetDataSource(list);
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
                lblStatus.HideStatus();

                m_InProcess = false;
                DoButtons();
            }
        }

        #region Control Events

        private void gridAPILog_SelectedItemsChanged(object sender, Infragistics.Windows.DataPresenter.Events.SelectedItemsChangedEventArgs e)
        {
            foreach (Record tmp in gridAPILog.SelectedItems.Records)
            {
                if (tmp.RecordType == RecordType.DataRecord &&
                    (tmp as DataRecord).DataItem is CNotifier)
                {
                    ((tmp as DataRecord).DataItem as CNotifier).NotifyAll();
                }
            }

            DoButtons();
        }
        private void gridAPILog_RecordMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (m_InProcess == false &&
                CUtility.IsDataRecord(sender))
            {
                btnDetails.PerformClick();
            }
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            CResultWrapper selected;
            CFrmAPILogDetails frm;

            m_InProcess = true;
            DoButtons();

            selected = gridAPILog.GetSelectedItems<CResultWrapper>()[0];

            frm = new CFrmAPILogDetails();
            frm.Start(
                this,
                selected.Data);

            m_InProcess = false;
            DoButtons();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseForm();
        }

        #endregion
    }
}
