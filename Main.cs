using System.Media;
using System.Text;
using WiFiViewer.Properties;
using static WiFiViewer.Utils.CMD;
using static WiFiViewer.Utils.XmlParser;

namespace WiFiViewer
{
    public partial class Main : Form
    {
        //Temp Path
        private string path = "C:\\Windows\\Temp\\WiFiViewerByStarlyn1232";

        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            //Static Size
            MinimumSize = Size;
            MaximumSize = Size;

            //Init Process Variable
            pro.StartInfo.UseShellExecute = false;
            pro.StartInfo.CreateNoWindow = true;
            pro.StartInfo.RedirectStandardInput = true;
            pro.StartInfo.RedirectStandardOutput = true;
            pro.StartInfo.RedirectStandardError = true;
            pro.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

            //Create Hidden Path
            ReCreatePath(path);
        }

        //ReCreate Path
        private void ReCreatePath(string path, bool create = true)
        {
            //Create Hidden Path
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            if (create)
                Directory.CreateDirectory(path);
        }

        //Load List
        private void ListAP()
        {
            string get_list = CMDStr("netsh wlan show profile", true);

            cbList.Items.Clear();

            string init = "User profiles\r\n-------------";
            string pattern = "    All User Profile     : ";
            string line = "";

            int qnt = 0;

            get_list = get_list.Substring(get_list.IndexOf(init) + init.Length);

            StringReader reader = new(get_list);

            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains(pattern))
                {
                    line = line.Substring(line.IndexOf(pattern) + pattern.Length);

                    if (line == "??")
                        continue;

                    cbList.Items.Add(line);
                    qnt++;
                }
            }

            bool state = qnt > 0;

            if (state)
                cbList.SelectedIndex = 0;

            cbList.Enabled = state;
            txtFind.Enabled = state;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            ListAP();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ind = cbList.SelectedIndex;

            try
            {
                if (ind < 0)
                    throw new Exception("1");

                string ap = cbList.SelectedItem as string;

                ReCreatePath(path);

                string get_pass = CMDStr($"netsh wlan export profile name=\"{ap}\" folder={path} key=clear", true);

                if (!get_pass.Contains("successfully"))
                    throw new Exception("2");

                string file = $"{path}\\Wi-Fi-{ap}.xml";

                using (var File = new FileStream(file,FileMode.Open,FileAccess.Read,FileShare.None))
                {
                    long qnt = File.Length;
                    byte[] data = new byte[qnt];
                    File.Read(data, 0, (int)qnt);

                    string content = Encoding.UTF8.GetString(data);

                    txtPass.Text = XmlRead(ref content, "keyMaterial");

                    string _info = "";
                    string _temp = XmlRead(ref content, "name");

                    _info += (String.IsNullOrEmpty(_temp) ? "" : $"Name : {_temp}\n");
                    _info += (String.IsNullOrEmpty((_temp = XmlRead(ref content, "connectionType"))) ? "" : $"Connection Type : {_temp}\n");
                    _info += (String.IsNullOrEmpty((_temp = XmlRead(ref content, "connectionMode"))) ? "" : $"Connection Mode : {_temp}\n");
                    _info += (String.IsNullOrEmpty((_temp = XmlRead(ref content, "authentication"))) ? "" : $"Authentication : {_temp}\n");
                    _info += (String.IsNullOrEmpty((_temp = XmlRead(ref content, "encryption"))) ? "" : $"Encryption : {_temp}\n");
                    _info += (String.IsNullOrEmpty((_temp = XmlRead(ref content, "keyType"))) ? "" : $"Key Type : {_temp}\n");

                    infoBox.Text = _info;

                    File.Close();
                }

                //MessageBox.Show(get_pass);
            }

            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);

                txtPass.Clear();
                infoBox.Clear();
                return;
            }
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            btnUpdate.Focus();

            using (var player = new SoundPlayer(Resources.Sayayin))
            {
                player.Play();

                Thread run = new(() =>
                {
                    Thread.Sleep(3600);
                                        
                    player.Stop();

                    _Dispose();
                });

                run.Start();
            }
        }

        //Collect Garbage
        private void _Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            ReCreatePath(path, false);
            Environment.Exit(0);
        }

        private void txtFind_TextChanged(object sender, EventArgs e)
        {
            int ind = cbList.Items.Count;

            if (ind == 0)
                return;

            object obj = null;

            foreach (var item in cbList.Items)
            {
                if (($"{item}").ToLower().Contains(txtFind.Text.ToLower()))
                {
                    obj = item;
                    break;
                }
            }

            if (obj != null)
                cbList.SelectedItem = obj;

            else
                cbList.SelectedIndex = 0;
        }
    }
}