using Newtonsoft.Json;
using styxSignHelper.Model;
using styxSignHelper.Properties;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Web;
using System.Web.Routing;
using System.Web.Services;
using System.Windows.Forms;

namespace styxSignHelper
{
    static class Program
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [STAThread]
        static void Main()
        {
            string thisprocessname = Process.GetCurrentProcess().ProcessName;
            if (Process.GetProcesses().Count(p => p.ProcessName == thisprocessname) > 1)
                return;

            IntPtr h = Process.GetCurrentProcess().MainWindowHandle;
            ShowWindow(h, 0);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MyCustomApplicationContext());
        }
    }
    public class MyCustomApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private readonly Uri jsonAddress;
        private readonly Uri soapAddress;
        private readonly ServiceHost hostJson;
        private readonly ServiceHost hostSoap;
        //private readonly RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        private readonly string localUrl = "http://localhost";

        public MyCustomApplicationContext()
        {
            // Initialize Tray Icon
            trayIcon = new NotifyIcon()
            {
                Icon = Resources.StyxAPI,
                Visible = true,
                Text = "Styx Sign Helper"
            };
            trayIcon.Click += new EventHandler(TrayIcon_Click);

            try
            {
                jsonAddress = new Uri(string.Format("{0}:{1}", localUrl, ConfigurationManager.AppSettings["port"]));
            }
            catch
            {
                jsonAddress = new Uri("http://localhost:64321");
            }
            try
            {
                soapAddress = new Uri(string.Format("{0}:{1}/soap", localUrl, ConfigurationManager.AppSettings["soapport"]));
            }
            catch
            {
                soapAddress = new Uri("http://localhost:64322/soap");
            }

            if (hostJson == null)
            {
                try
                {
                    hostJson = new WebServiceHost(typeof(SignService), jsonAddress);
                    hostJson.AddServiceEndpoint(typeof(ISignService), new WebHttpBinding(), "");
                    hostJson.Open();
                }
                catch (CommunicationException commProblem)
                {
                    Console.WriteLine("There was a communication problem. " + commProblem.Message);
                }
            }

            if (hostSoap == null)
            {
                try
                {
                    hostSoap = new ServiceHost(typeof(SoapService), soapAddress);
                    ServiceMetadataBehavior smb = hostJson.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (smb == null)
                        smb = new ServiceMetadataBehavior
                        {
                            HttpGetEnabled = true
                        };
                    smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                    hostSoap.Description.Behaviors.Add(smb);
                    hostSoap.AddServiceEndpoint(
                      ServiceMetadataBehavior.MexContractName,
                      MetadataExchangeBindings.CreateMexHttpBinding(),
                      "mex"
                    );
                    var wsHttpBinding = new WSHttpBinding(SecurityMode.None);
                    hostSoap.AddServiceEndpoint(typeof(ISignService), new BasicHttpBinding(), "");
                    hostSoap.Open();
                }
                catch (CommunicationException commProblem)
                {
                    Console.WriteLine("There was a communication problem. " + commProblem.Message);
                }
            }

            //rkApp.SetValue("styxWebAPI", Application.ExecutablePath);
        }

        void Help(object sender, EventArgs e)
        {
            string applicationDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            string myFile = Path.Combine(applicationDirectory, "doc/help.html");
            Process.Start(myFile);
        }

        void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            if (hostJson != null)
                hostJson.Close();
            if (hostSoap != null)
                hostSoap.Close();
            Application.Exit();
        }

        private void TrayIcon_Click(object sender, EventArgs e)
        {
            var eventArgs = e as MouseEventArgs;
            switch (eventArgs.Button)
            {
                case MouseButtons.Right:
                    CreateNotifyMenu();
                    MethodInfo methodInfo = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                    methodInfo.Invoke(trayIcon, null);
                    break;
            }
        }

        private void CreateNotifyMenu()
        {
            trayIcon.ContextMenu = new ContextMenu();
            //trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Help", Help));
            //try
            //{
            //    var client = new ASClient.ASClientControl();
            //    client.Init();
            //    client.Silent = false;
            //    var signCertificateSerialNumber = client.SignCertificateSerialNumber;
            //    if (signCertificateSerialNumber != null)
            //    {
            //        trayIcon.ContextMenu.MenuItems.Add(new MenuItem(signCertificateSerialNumber));
            //        trayIcon.ContextMenu.MenuItems.Add(new MenuItem("-"));
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);
            //}
            trayIcon.ContextMenu.MenuItems.Add(new MenuItem("Exit", Exit));
        }
    }

    [ServiceContract]
    [MyBehavior]
    public interface ISignService
    {
        [OperationContract]
        [WebInvoke(Method = "*", UriTemplate = "SignString", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SignStrOut SignString(SingStrIn singStrIn);

        [OperationContract(Name = "GetSignString")]
        [WebGet(UriTemplate = "SignString/{cert_sn}/{str_to_sign}")]
        string GetSignString(string cert_sn, string str_to_sign);

        [OperationContract]
        [WebInvoke(Method = "*", UriTemplate = "crypto/signMSG", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        SignMSGOut SignMSG(SignMSGIn signMSGIn);
    }

    public class SignService : ISignService
    {
        public SignStrOut SignString(SingStrIn singStrIn)
        {
            SignStrOut result = new SignStrOut();
            try
            {
                result.Result = SignStr(singStrIn);
                result.error = null;
            }
            catch (Exception ex)
            {
                result.Result = null;
                result.error.Message = ex.Message;
                result.error.Code = 60600;
            }
            return result;
        }

        public string GetSignString(string cert_sn, string str_to_sign)
        {
            SignStrOut result = SignString(new SingStrIn()
            {
                CertSn = cert_sn,
                StrToSign = str_to_sign
            });
            return result.Result.Signature;
        }

        private SignStrResult SignStr(SingStrIn singStrIn)
        {
            SignStrResult result = new SignStrResult();
            if (singStrIn == null) return result;
            try
            {
                var client = new ASClient.ASClientControl();
                client.Init();
                client.Silent = false;
                var signCertificateSerialNumber = singStrIn.CertSn.Length > 0 ? singStrIn.CertSn : client.SignCertificateSerialNumber;
                if (signCertificateSerialNumber != null)
                {
                    result.StrToSign = singStrIn.StrToSign;
                    result.CertSn = signCertificateSerialNumber;
                    result.Signature = client.SignMessageCertCodepage(singStrIn.StrToSign, signCertificateSerialNumber, 1251);
                }
            }
            catch 
            {
                throw;
            }
            return result;
        }

        public SignMSGOut SignMSG(SignMSGIn signMSGIn)
        {
            SignMSGOut result = new SignMSGOut();
            if (signMSGIn == null) return result;
            try
            {
                var client = new ASClient.ASClientControl();
                client.Init();
                client.Silent = false;
                var signCertificateSerialNumber = signMSGIn.CertSn.Length > 0 ? signMSGIn.CertSn : client.SignCertificateSerialNumber;
                if (signCertificateSerialNumber != null)
                {
                    result.SignedMsg = client.SignMessageCertCodepage(signMSGIn.Obj, signCertificateSerialNumber, 1251);
                    if (result.SignedMsg.Equals(string.Empty))
                        throw new Exception("Certificate not found");
                    else
                        result.Result = 1;
                }
            }
            catch (Exception ex)
            {
                result.Result = 0;
                result.ErrorCode = 1;
                result.Status = ex.Message;

            }
            return result;
        }
    }

    /// <summary>
    /// Сводное описание для Mobile
    /// </summary>
    [WebService]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class SoapService : ISignService
    {

        [WebMethod(Description = "Sign string")]
        public SignStrOut SignString(SingStrIn singStrIn)
        {
            SignService sSignService = new SignService();
            return sSignService.SignString(singStrIn);
        }

        public string GetSignString(string cert_sn, string str_to_sign)
        {
            SignService sSignService = new SignService();
            return sSignService.GetSignString(cert_sn, str_to_sign);
        }

        public SignMSGOut SignMSG(SignMSGIn signMSGIn)
        {
            throw new NotImplementedException();
        }
    }
}
