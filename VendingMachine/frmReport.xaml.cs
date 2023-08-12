using log4net;
using MaterialDesignThemes.Wpf;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
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
using VendingMachine.Helpers;
using Microsoft.Office.Interop.Excel;
using System.Net.Mail;
using System.Net.Http.Headers;
using System.Net;
using System.IO.Enumeration;
using System.IO;
using System.Diagnostics;

namespace VendingMachine
{
    /// <summary>
    /// Interaction logic for frmReport.xaml
    /// </summary>
    public partial class frmReport : System.Windows.Window
    {
        public frmReport()
        {
            InitializeComponent();
        }

        Access acc = new Access();
        string cmd = string.Empty;string ToAddress = string.Empty;
        System.Data.DataTable dt = new System.Data.DataTable();
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                radioBtn_Stock.IsChecked = true;
                radioBtn_Sales.IsChecked = false;
                txtEmailId.Focus();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        private void btnSendReport_Click(object sender, RoutedEventArgs e)
        {
            string reportName = string.Empty;
            if (radioBtn_Sales.IsChecked.HasValue ? radioBtn_Sales.IsChecked.Value : false)
            {
                reportName = radioBtn_Sales.Content.ToString();
            }
            else if (radioBtn_Stock.IsChecked.HasValue ? radioBtn_Stock.IsChecked.Value : false)
            {
                reportName = radioBtn_Stock.Content.ToString();
            }
            if (txtEmailId.Text != null)
            {
                ToAddress = txtEmailId.Text;
            }
            try
            {
                if (reportName == "Stock Report")
                {
                    cmd = @"SELECT product_id as 'Product ID', product_name as 'Product Name', sum(stock) as 'Available In Stock',OutOfStock as 'Out Of Stock' from (
	                    Select product_id, product_name ,price, 
                        case when soldout > 0 then 0 else stock end as stock,
                        case when soldout > 0 then 'Yes' else 'No' end as OutOfStock
                        from mst_product p
                        ) as M group by M.product_id;";
                }
                else if (reportName == "Sales Report")
                {
                    cmd = @"select order_id as 'Order ID',product_lineitems as 'Order Details',total_amount as 'Order Amount',
                        total_quantity as 'Order Quantity',order_datetime as 'Order Date',payment_method as 'Payment Method', 
                        transaction_id as 'Transaction ID',machine_id as 'Machine ID' from sales_order order by order_datetime";
                }
                dt = acc.GetTable(cmd);
                if (dt.Rows.Count > 0 && !string.IsNullOrEmpty(reportName))
                {
                    string filename =  ExportToExcel(dt, reportName);
                    SendThroughMail(filename, reportName);
                }
            }
            catch (Exception ex)
            {
                DisplayMsg(ex.Message);
                log.Error(ex);
            }
        }

        public async void DisplayMsg(string msg)
        {
            try
            {
                var sampleMessageDialog = new Dialog { Message = { Text = msg } };
                await DialogHost.Show(sampleMessageDialog, "frmReportDialog");
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private async void SendThroughMail(string fileAttachment, string ReportName)
        {
            try
            {
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(Properties.Settings.Default.FromMailAddress);
                mail.To.Add(ToAddress);
                mail.Subject = string.Format("{0} - {1}", ReportName, DateTime.Now);
                mail.IsBodyHtml = true;
                mail.Body = "<!DOCTYPE HTML> <html>\r\n <head>\r\n </head>\r\n <body>\r\n   <h1>Greetings From Gurushektra<h1> <h3>Please Find the Attached Report<h3>\r\n </body>\r\n</html>";
                SmtpClient SmtpServer = new SmtpClient(Properties.Settings.Default.SMTPClientAddress);
                System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(fileAttachment);
                mail.Attachments.Add(attachment);
                SmtpServer.Port = 587;
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new NetworkCredential(Properties.Settings.Default.FromMailAddress, Properties.Settings.Default.SMTPAppPassword);
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
                DisplayMsg("Mail Sent Successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string ExportToExcel(System.Data.DataTable dt,string label)
        {
            Microsoft.Office.Interop.Excel.Application excel = null;
            Microsoft.Office.Interop.Excel.Workbook wb = null;

            object missing = Type.Missing;
            Microsoft.Office.Interop.Excel.Worksheet ws = null;
            Microsoft.Office.Interop.Excel.Range rng = null;
            string filename = String.Empty;

            try
            {
                filename = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "/Reports/"+label+"/"+string.Format("{0}.xlsx",label));
                excel = new Microsoft.Office.Interop.Excel.Application();
                wb = excel.Workbooks.Open(filename, 0, false, 5, "", "",
                            false, XlPlatform.xlWindows, "", true, false,
                            0, true, false, false);
                ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.ActiveSheet;

                for (int Idx = 0; Idx < dt.Columns.Count; Idx++)
                {
                    ws.Range["A1"].Offset[0, Idx].Value = dt.Columns[Idx].ColumnName;
                }

                for (int Idx = 0; Idx < dt.Rows.Count; Idx++)
                {  
                    ws.Range["A2"].Offset[Idx].Resize[1, dt.Columns.Count].Value =
                    dt.Rows[Idx].ItemArray;
                }

                wb.RefreshAll();
                excel.Calculate();
                wb.Save();
                wb.Close(true);
                excel.Quit();
            }
            catch (COMException ex)
            {
                MessageBox.Show("Error accessing Excel: " + ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.ToString());
            }
            return filename;
        }

        private void adminBtn_Click(object sender, RoutedEventArgs e)
        {
            frmAdminControl frm=new frmAdminControl();
            this.Close();
            frm.Show();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtEmailId.Text = string.Empty;
        }
    }
}
