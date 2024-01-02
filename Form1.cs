using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat
{
    public partial class Form1 : Form
    {
        Socket socket;
        EndPoint epLocal, epRemote;
        byte[] buffer;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // set up socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // get user IP
            textLocalIp.Text = GetLockalIP();
            textRemoteIp.Text = GetLockalIP();
        }
        private string GetLockalIP()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip.ToString();
            }
            return "127.0.0.1";
        }


        private void buttonConnect_Click(object sender, EventArgs e)
        {
            // binding Socket
            epLocal = new IPEndPoint(IPAddress.Parse(textLocalIp.Text), Convert.ToInt32(textLocalPort.Text));
            socket.Bind(epLocal);
            // Connecting to remote IP
            epRemote = new IPEndPoint(IPAddress.Parse(textRemoteIp.Text), Convert.ToInt32(textRemotePort.Text));
            socket.Connect(epRemote);
            // Listening the specific port
            buffer = new byte[4096];
            socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);

        }

        private void MessageCallBack(IAsyncResult asyncResult)
        {
            try
            {
                byte[] receiveData = new byte[4096];
                receiveData = (byte[])asyncResult.AsyncState;
                // Converting byte[] to string
                UTF8Encoding uEncoding = new UTF8Encoding();
                string receiveMessage = uEncoding.GetString(receiveData);

                // Adding this message into Listbox
                listMessage.Items.Add("Friend: " + receiveMessage);

                buffer = new byte[4096];
                socket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epRemote, new AsyncCallback(MessageCallBack), buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textLocalPort_TextChanged(object sender, EventArgs e)
        {

        }

        private void textRemotePort_TextChanged(object sender, EventArgs e)
        {

        }

        private void textMessage_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            // Convert string message to byte[]
            UTF8Encoding uEncoding = new UTF8Encoding();
            byte[] sendingMessage = new byte[4096];
            sendingMessage = uEncoding.GetBytes(textMessage.Text);
            // Sending the Encoded message
            socket.Send(sendingMessage);
            // adding to the Listbox
            listMessage.Items.Add("Me: " + textMessage.Text);
            textMessage.Text = "";
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
