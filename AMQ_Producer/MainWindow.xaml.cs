using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Apache.NMS;
using Apache.NMS.ActiveMQ;
using System;
using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;
using Newtonsoft.Json;

namespace AMQ_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IConnection _connection;
        private ISession _session;
        private IMessageProducer _producer;

        public MainWindow()
        {
            
            InitializeComponent();
            Init();
            //_connection = connection;
            //_session = session;
            //_producer = producer;
        }

        private void Init()
        {
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");
            ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);

            // Create a Connection
            _connection = connectionFactory.CreateConnection();
            _connection.Start();

            // Create a Session
            _session = _connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

            // Get the destination (Topic or Queue)
            IDestination destination = _session.GetQueue("RTIS");

            // Create a MessageProducer from the Session to the Topic or Queue
            _producer = _session.CreateProducer(destination);
            _producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a messages
            JsonObj obj = new JsonObj()
            {
                STS_Id = 1,
                Container_Id = "CONT123456",
                Container_Id_Conf = 100,
                Iso_Code = "C123",
                Iso_Code_Conf = 100,
                ITV = 360,
                ITV_Conf = 100,
                Sealed = 'Y',
                Sealed_Conf = 100,
                Hazard = 'N',
                Hazard_Conf = 100,
                Twin = 'N',
                Door_Direction = 'F',
                Move_Type = "LOAD",
                Img_Paths = new Dictionary<string, string>
                {
                    {"itv_num","PATH" },
                    {"container_Id","PATH" },
                    {"seal","PATH" },
                    {"hazard","PATH" },
                    {"twin","PATH" },
                    {"damages","PATH" },
                    {"record","PATH" },
                },
                Damages = new Dictionary<string, int>
                {
                    {"hole",0 },
                    {"rust",0 },
                    {"dent",0 },
                },
                TOS_Id = 1,
            };
            string jsonText = JsonConvert.SerializeObject(obj);
            ITextMessage message = _session.CreateTextMessage(jsonText);

            // Tell the producer to send the message
            _producer.Send(message);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up
            _session.Close();
            _connection.Close();
        }

        public class JsonObj
        {
            public int? STS_Id { get; set; }
            public string? Container_Id { get; set; }
            public int? Container_Id_Conf { get; set; }
            public string? Iso_Code { get; set; }
            public int? Iso_Code_Conf { get; set; }
            public int? ITV { get; set; }
            public int? ITV_Conf { get; set; }
            public char? Sealed { get; set; }
            public int? Sealed_Conf { get; set; }
            public char? Hazard { get; set; }
            public int? Hazard_Conf { get; set; }
            public char? Twin { get; set; }
            public char? Door_Direction { get; set; }
            public string? Move_Type { get; set; }
            public Dictionary<string, string>? Img_Paths { get; set; }
            public Dictionary<string, int>? Damages { get; set; }
            public int? TOS_Id { get; set; }
        }
    }
}