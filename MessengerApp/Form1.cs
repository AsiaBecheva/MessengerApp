namespace MessengerApp
{
    using System;
    using System.Windows.Forms;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;

    public partial class Form1 : Form
    {
        Socket sck;
        EndPoint epLocal, epRemote;
        byte[] buffer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Set up socket
            sck = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sck.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            //Get ip address
            textLocalIp.Text = GetLocalIp();
            textRemoteIp.Text = GetLocalIp();
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            //Binding Socket
            epLocal = new IPEndPoint(IPAddress.Parse(textLocalIp.Text), Convert.ToInt32(textLocalPort.Text));
            sck.Bind(epLocal);

            //Connecting to remote IP
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIp.Text), Convert.ToInt32(textRemotePort.Text));
            sck.Connect(epRemote);

            //Listening the specific port
            buffer = new byte[1500];
            sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallback), buffer); 
        }

        private void MessageCallback(IAsyncResult aResult)
        {
            try
            {
                byte[] receivedData = new byte[1500];
                receivedData = (byte[])aResult.AsyncState;

                //Converting byte to string
                ASCIIEncoding aEncoding = new ASCIIEncoding();
                string receivedMessage = aEncoding.GetString(receivedData);

                //Adding this message to listbox
                listMessage.Items.Add("Friend: " + receivedMessage);

                buffer = new byte[1500];
                sck.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallback), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            //Convert string message to byte
            ASCIIEncoding aEncoding = new ASCIIEncoding();
            byte[] sendingMessage = new byte[1500];
            sendingMessage = aEncoding.GetBytes(textMessage.Text);

            //Sending the Encoded message
            sck.Send(sendingMessage);

            //Adding to the listbox
            listMessage.Items.Add("Me: " + textMessage.Text);
            textMessage.Text = "";
        }

        private string GetLocalIp()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "198.168.1.1";
        }
    }
}
