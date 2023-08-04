using log4net;
using MaterialDesignThemes.Wpf;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using VendingMachine.Helpers;

namespace VendingMachine
{
    /// <summary>
    /// Interaction logic for frmConformScreen.xaml
    /// </summary>
    public partial class frmOrderConfirmation : Window
    {

        ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Access acc = new Access();
        modbus mb = new modbus();
        public List<order_item> ordered = new List<order_item>();

        public frmOrderConfirmation()
        {
            InitializeComponent();
         
        }
       

        private void btnPayment_Click(object sender, RoutedEventArgs e)
        {
            if (machine_ready())
            {
                frmUPIPayTM frm = new frmUPIPayTM();
                this.Close();
                frm.Show();
            }
            else
            {
                log.Info("Machine Out Of Order");
                DisplayMsg("Machine out of Order");
            }
        }
        
    
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            totAmt.Text = (config.ordered.Select(k => k.amt)).Sum().ToString("#0.00");
            try
            {
                if (config.ordered.Count > 0)
                {
                    ConfirmOrderdataGrid.FontSize = 17;
                    
                    ConfirmOrderdataGrid.ItemsSource = 
                        (from m in config.ordered select 
                        new {m.product_name,m.qty,m.amt}).ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.G))
                {
                    if (e.Key == Key.G)
                    {
                        e.Handled = true;
                        frmAdminControl frm = new frmAdminControl();
                        this.Close();
                        frm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public async void DisplayMsg(string msg)
        {
            try
            {
                var sampleMessageDialog = new Dialog { Message = { Text = msg } };
                await DialogHost.Show(sampleMessageDialog, "frmOrderConfirmationDialog");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public bool machine_ready()
        {
            bool state = false;

            try
            {
                state = mb.Open(Hardware.vending_port, Hardware.vending_BaudRate, Hardware.vending_DataBits, Hardware.vending_Parity, Hardware.vending_StopBits);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            mb.Close();
            return state;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                frmOrderNow frm = new frmOrderNow();
                this.Close();
                frm.Show();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
