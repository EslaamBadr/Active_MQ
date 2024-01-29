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
using System.Threading.Tasks;

namespace AMQ_Consumer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IConnection connection;
        private ISession session;
        private IMessageConsumer consumer;

        public MainWindow()
        {
            InitializeComponent();
            this.Init();
        }

        private void Init()
        {
            Uri connecturi = new Uri("activemq:tcp://localhost:61616");
            ConnectionFactory connectionFactory = new ConnectionFactory(connecturi);

            // Create a Connection
            this.connection = connectionFactory.CreateConnection();
            this.connection.Start();

            // Create a Session
            this.session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

            // Get the destination (Topic or Queue)
            IDestination destination = this.session.GetQueue("RTIS");

            // Create a MessageProducer from the Session to the Topic or Queue
            this.consumer = this.session.CreateConsumer(destination);
            this.consumer.Listener += Consumer_Listener;
        }

        private void Consumer_Listener(IMessage message)
        {
            var txtMessage = message as ITextMessage;
            lstBox.Dispatcher.Invoke(() => {
                lstBox.Items.Add(txtMessage.Text);
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clean up
            consumer.Close();
            session.Close();
            connection.Close();
        }
    }
}