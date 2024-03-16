using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Controls;



namespace ClientSideWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IPEndPoint endpoint;
        UdpClient client;
        IPEndPoint remoteep;
        public string ImageSource
        {
            get { return (string)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(string), typeof(MainWindow));


        public MainWindow()
        {
            InitializeComponent();
            remoteep = new IPEndPoint(IPAddress.Parse("192.168.100.15"), 45001);
            client = new UdpClient();
            DataContext = this;
        }


        private async void Clicked(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(txtbox.Text))
                btn.IsEnabled = true;
            else
                return;

            //sending remote name
            await client.SendAsync(Encoding.UTF8.GetBytes(txtbox.Text), remoteep);
            new Thread(async () =>
            {
                while (true)
                {
                    var imgStream = captureScreenAsync();
                    var chunks = imgStream.ToArray().Chunk(ushort.MaxValue - 29).ToList();
                    foreach (var item in chunks)
                        try { await client.SendAsync(item, item.Length, remoteep); } catch (Exception e) { MessageBox.Show(e.Message); };
                }
            }).Start();


            new Thread(async () =>
            {

                var buffer = new byte[ushort.MaxValue - 29];
                int maxlen = buffer.Length;
                int len = 0;
                var list = new List<byte>();
                while (true)
                {
                    try
                    {
                        do
                        {
                            var result = client.Receive(ref remoteep);
                            MessageBox.Show("recives client");
                            buffer = result;
                            len = buffer.Length;
                            list.AddRange(buffer);
                        } while (len == maxlen);
                        File.AppendAllText("footest.txt", $"recives\n");
                        img.Source = Convert(list.ToArray());
                        list.Clear();
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }).Start();

        }

        MemoryStream? captureScreenAsync()
        {
            using (Bitmap? bitmap = new Bitmap(1920, 1080))
            {
                using (Graphics gr = Graphics.FromImage(bitmap))
                    gr?.CopyFromScreen(0, 0, 0, 0, new System.Drawing.Size(1920, 1080));
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    bitmap?.Save(memoryStream, ImageFormat.Jpeg);
                    return memoryStream;
                }
            }

        }
        static BitmapImage Convert(byte[] byteArray)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = new MemoryStream(byteArray);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();
            return image;
        }

        private async void Clicked_enterance(object sender, RoutedEventArgs e)
        {
            if (txtbox_enterance is null)
                return;
            await client.SendAsync(Encoding.UTF8.GetBytes(txtbox_enterance.Text), txtbox_enterance.Text.Length, remoteep);
            mainPanel.Visibility = Visibility.Visible;
            enterancePanel.Visibility = Visibility.Hidden;

        }



    }
}