﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using log4net;
using log4net.Layout;
using NP2COM;


namespace NP2COMV
{
    public partial class Form1 : Form
    {
        public Form1 ()
        {
            InitializeComponent ();
        }

        private void Form1_Load (object sender, EventArgs e)
        {
            namedPipeComboBox.Items.AddRange(Directory.GetFiles(@"\\.\pipe\"));
            serialPortComboBox.Items.AddRange(SerialPort.GetPortNames());
            parityComboBox.Items.AddRange(Enum.GetNames(typeof(Parity)));
            stopBitsComboBox.Items.AddRange(Enum.GetNames(typeof(StopBits)));
            if (namedPipeComboBox.Items.Count > 0) namedPipeComboBox.SelectedIndex = 0;
            if (serialPortComboBox.Items.Count > 0) serialPortComboBox.SelectedIndex = 0;
            baudRateComboBox.SelectedIndex = 10;
            parityComboBox.SelectedIndex = 0;
            dataBitsComboBox.SelectedIndex = 3;
            stopBitsComboBox.SelectedIndex = 1;
        }

        private void button1_Click (object sender, EventArgs e)
        {
            if (Connection != null && Connection.IsStarted)
            {
                button1.Text = "Test";
                Connection.Stop();
            }
            else
            {
                var rtbAppender = new RichTextBoxAppender {RichTextBox = richTextBox1};
                log4net.Config.BasicConfigurator.Configure(rtbAppender);
                rtbAppender.Layout = new PatternLayout("%-5p %d{HH:mm:ss,fff} %-22.22c{1} %-18.18M - %m%n");
                var namedPipe = Regex.Match((string) namedPipeComboBox.SelectedItem, @"\\\\(?<machine>[^\\]+)\\pipe\\(?<pipe>\w+)");
                Parity parity;
                StopBits stopbits;
                Enum.TryParse((string) parityComboBox.SelectedItem, out parity);
                Enum.TryParse((string) stopBitsComboBox.SelectedItem, out stopbits);
                Connection = new Connection(new Settings
                                                {
                                                    Logger = LogManager.GetLogger(typeof(Connection)),
                                                    BaudRate = int.Parse((string) baudRateComboBox.SelectedItem),
                                                    ComPort = (string) serialPortComboBox.SelectedItem,
                                                    Parity = parity,
                                                    StopBits = stopbits,
                                                    DataBits = int.Parse((string) dataBitsComboBox.SelectedItem),
                                                    MachineName = namedPipe.Groups["machine"].Value,
                                                    NamedPipe = namedPipe.Groups["pipe"].Value,
                                                });
                Connection.Start();
                button1.Text = "Testing...";
            }
        }

        protected Connection Connection { get; set; }
    }
}
