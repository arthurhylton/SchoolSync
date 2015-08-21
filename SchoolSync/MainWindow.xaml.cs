using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace SchoolSync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm = new MainWindowViewModel();
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = vm;
            PostSchoolsToServer(vm.Changes.ToList());
        }

        private void PostSchoolsToServer(List<School> schools)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(Properties.Settings.Default.RemoteDatabaseWebServiceURL);
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";

            using(var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(schools);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using(var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                global::System.Windows.MessageBox.Show(result);
            }
            //Dictionary<string, string> data = new Dictionary<string, string>();
            //int index = 0;
            //foreach (var school in schools)
            //{
            //    data.Add()
            //}
        }
    }
}
