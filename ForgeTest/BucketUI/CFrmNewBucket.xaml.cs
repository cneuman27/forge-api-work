using System;
using System.Collections.Generic;
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

using MOMControls;
using MOMShared;

using Autofac;

namespace ForgeTest.BucketUI
{
    public partial class CFrmNewBucket : CMOMForm
    {
        private bool m_InProcess = true;
        private bool m_Cancelled = true;

        private ForgeAPI.Interface.Authentication.IToken m_AuthenticationToken = null;
        private IContainer m_IoC = null;

        public bool Cancelled
        {
            get { return m_Cancelled; }
        }

        public CFrmNewBucket()
        {
            InitializeComponent();
        }

        protected override void DoButtons()
        {
            if (m_InProcess == true)
            {
                txtBucketName.DisableControl();
                cmbRetentionPolicy.DisableControl();
                btnOK.DisableControl();
            }
            else
            {
                txtBucketName.EnableControl();
                cmbRetentionPolicy.EnableControl();

                if (string.IsNullOrWhiteSpace(txtBucketName.GetText()) ||
                    cmbRetentionPolicy.GetSelectedIndex() == -1)
                {
                    btnOK.DisableControl();
                }
                else
                {
                    btnOK.EnableControl();
                }
            }
        }

        public void Start(
            Window parent,
            ForgeAPI.Interface.Authentication.IToken authToken,
            IContainer ioc)
        {
            m_AuthenticationToken = authToken;
            m_IoC = ioc;

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoLoad));

            Owner = parent;
            ShowDialog();
        }

        private void DoLoad(object state)
        {
            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Loading");
                lblStatus.ShowStatus();

                cmbRetentionPolicy.AddItem(ForgeAPI.Interface.Enums.E_RetentionPolicy.Transient);
                cmbRetentionPolicy.AddItem(ForgeAPI.Interface.Enums.E_RetentionPolicy.Temporary);
                cmbRetentionPolicy.AddItem(ForgeAPI.Interface.Enums.E_RetentionPolicy.Persistent);
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
        private void DoOK(object state)
        {
            bool hasError = false;

            ForgeAPI.Interface.IFactory factory;
            ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IInputs inputs;
            ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IOutputs outputs;
            ForgeAPI.Interface.DataManagement.Buckets.IService service;

            try
            {
                m_InProcess = true;
                DoButtons();

                lblStatus.SetStatus("Adding Bucket");
                lblStatus.ShowStatus();

                service = m_IoC.Resolve<ForgeAPI.Interface.DataManagement.Buckets.IService>();
                factory = m_IoC.Resolve<ForgeAPI.Interface.IFactory>();
                inputs = factory.CreateInputs<ForgeAPI.Interface.DataManagement.Buckets.CreateBucket.IInputs>(m_AuthenticationToken);

                inputs.BucketKey = txtBucketName.GetText().Trim();
                inputs.RetentionPolicy = (ForgeAPI.Interface.Enums.E_RetentionPolicy)cmbRetentionPolicy.GetSelectedItem();

                outputs = service.CreateBucket(inputs);

                if (outputs.Success() == false)
                {
                    throw new Exception(outputs.FailureReason());
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                hasError = true;
                ShowError(e);
            }
            finally
            {
                if (hasError)
                {
                    lblStatus.HideStatus();

                    m_InProcess = false;
                    DoButtons();
                }
                else
                {
                    m_Cancelled = false;
                    HideForm();
                }
            }
        }

        #region Control Events

        private void txtBucketName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (m_InProcess == false)
            {
                DoButtons();
            }
        }

        private void cmbRetentionPolicy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (m_InProcess == false)
            {
                DoButtons();
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            m_InProcess = true;
            DoButtons();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DoOK));
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            m_Cancelled = true;
            HideForm();
        }

        #endregion
    }
}
