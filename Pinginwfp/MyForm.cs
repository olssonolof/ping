using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Media;
using System.Threading;

namespace Pinginwfp
{

    class MyForm : Form
    {
        Label acces = new Label();
        bool keepGoing = false;
        CheckBox loop = new CheckBox();
        CheckBox autoScroll = new CheckBox();
        Button start = new Button();
        string target;
        string input;
        

        Dictionary<string, string> hostIP = new Dictionary<string, string>
        {
            { "LocalHost", "127.0.0.1" },
            { "Google", "64.233.191.155"},
            {"Broken adress", "1" }
        };
        RichTextBox response = new RichTextBox();
        ComboBox host = new ComboBox();

        public MyForm()
        {
            Size = new Size(500, 400);
            loop = new CheckBox()
            {
                CheckAlign = ContentAlignment.MiddleRight,
            };

            autoScroll = new CheckBox
            {
                Text = "Autoscroll",
                TextAlign = ContentAlignment.MiddleRight,
                CheckAlign = ContentAlignment.MiddleRight,

            };

            Label looping = new Label
            {
                Text = "Continuous pinging?",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,

            };

            TableLayoutPanel indexing = new TableLayoutPanel
            {
                RowCount = 4,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
            };

            TableLayoutPanel indexing2 = new TableLayoutPanel
            {
                RowCount = 2,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
            };

            acces = new Label
            {
                Text = "Is there internetacces?",
                Dock = DockStyle.Fill,
                Font = new Font("Arial", 20),
            };

            start = new Button
            {
                Text = "Start",
                Anchor = AnchorStyles.Top,

            };

            Button stop = new Button
            {
                Text = "Stop",
                Anchor = AnchorStyles.Top
            };

            host = new ComboBox
            {
                Dock = DockStyle.Fill,
            };

            host.DataSource = new BindingSource(hostIP, null);
            host.DisplayMember = "Key";
            host.ValueMember = "Value";

            // host.Items.AddRange(new object[] { "LocalHost" , "Google", "Broken adress" });

            // host.SelectedIndex = 0;


            response = new RichTextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(15),
                HideSelection = false,
            };

            start.Click += Start_Click;
            loop.CheckStateChanged += Loop_CheckStateChanged;
            stop.Click += Stop_Click;
            response.TextChanged += Response_TextChanged;
            host.TextChanged += Host_TextChanged;
            host.KeyUp += Host_KeyUp;


            indexing.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            indexing.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            indexing.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            indexing.RowStyles.Add(new RowStyle(SizeType.Percent, 40));


            indexing.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            indexing.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            indexing.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            Controls.Add(indexing);
            indexing.Controls.Add(acces, 1, 0);
            indexing.Controls.Add(indexing2, 1, 1);
            indexing2.Controls.Add(loop, 0, 0);
            indexing2.Controls.Add(start, 0, 1);
            indexing2.Controls.Add(looping, 1, 0);
            indexing2.Controls.Add(stop, 1, 1);

            indexing.Controls.Add(host, 1, 2);
            indexing.Controls.Add(response, 1, 3);
            indexing.Controls.Add(autoScroll, 0, 3);





        }

        private void Host_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                StartPing();
            }
        }

        private void Host_TextChanged(object sender, EventArgs e)
        {            
            keepGoing = false;
            loop.Checked = false;
            acces.ForeColor = Color.Black;
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            keepGoing = false;
            loop.Checked = false;
            acces.ForeColor = Color.Black;
        }

        private void Loop_CheckStateChanged(object sender, EventArgs e)
        {
            if (loop.Checked)
            {
                keepGoing = true;
            }
            else
            {
                keepGoing = false;
            }
        }

        private void Start_Click(object sender, EventArgs e)
        {
            StartPing();
        }

        private void Response_TextChanged(object sender, EventArgs e)
        {
            if (autoScroll.Checked)
            {
                // set the current caret position to the end
                response.SelectionStart = response.Text.Length;
                // scroll it automatically
                response.ScrollToCaret();
            }
        }

        void StartPing()
        {
            input = host.Text;
            if (!hostIP.Keys.Any(key => key.Contains(input)))
            {
                target = input;
            }
            else
            {
                target = ((KeyValuePair<string, string>)host.SelectedItem).Value;
            }

            start.Enabled = false;
            Thread pingThread = new Thread(new ThreadStart(() => Ping(target)));
            pingThread.Start();
        }


        void Ping(string host)
        {

            Invoke((MethodInvoker)delegate
            {
                response.Text = ""; // runs on UI thread
            });                     
            do
            {
                try
                {
                    Ping ping = new Ping();
                    PingReply reply = ping.Send(host, 1000);
                    

                    if (reply.Status.ToString() == "Success")
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            acces.ForeColor = Color.Green;
                            response.Text += $"Adress pinged: {reply.Address.ToString()}\nStatus : {reply.Status}\nTime : {reply.RoundtripTime.ToString()} \nAddress : {reply.Address}\n=======================\n";
                        });
                    }
                    else
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            response.Text += "Ping failde! \nYou do NOT have acces to the internet\n";
                            acces.ForeColor = Color.Black;
                        });
                    }
                }

                catch
                {
                    Invoke((MethodInvoker)delegate
                    {
                        response.Text += "Ping failde! \nYou do NOT have acces to the internet\n";
                        acces.ForeColor = Color.Black;
                    });
                }
                if (keepGoing)
                {
                    Thread.Sleep(1000);
                }

                if (keepGoing == false)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        start.Enabled = true;
                    });
                }

            } while (keepGoing);
        }
    }
}
