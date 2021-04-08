using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatacertiaVIESChecker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.datacertia.com/?utm_source=software&utm_medium=linklabel&utm_campaign=vieschecker");
        }


        private void button1_Click(object sender, EventArgs e)
        {
            _ = Cursors.WaitCursor;
            string mensaje = "";
            bool resultado = false;
            string soap0 =
                        @"<?xml version=""1.0"" encoding=""utf-8""?>
                        <soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:ec.europa.eu:taxud:vies:services:checkVat:types"">
                       <soapenv:Header/>
                       <soapenv:Body>
                          <urn:checkVat>
                             <urn:countryCode>";
            string pais = comboBox.SelectedItem.ToString();
            string soap2 = @"</urn:countryCode>
                             <urn:vatNumber>";
            string nif = textBox.Text;
            string soap3 = @"</urn:vatNumber>
                          </urn:checkVat>
                       </soapenv:Body>
                    </soapenv:Envelope>";

            string[] soap = { soap0, pais, soap2, nif, soap3 };


            if (nif.Length == 0)
            {
                mensaje = "Debes introducir un NIF.";
                MessageBox.Show(mensaje);
                _ = Cursors.Default;
                return;
            }
            if (nif.Contains("-"))
            {
                mensaje = "Debes introducir el NIF sin guiones";
                MessageBox.Show(mensaje);
                _ = Cursors.Default;
                return;
            }

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://ec.europa.eu/taxation_customs/vies/services/checkVatService");
            req.ContentType = "text/xml;charset=\"utf-8\"";
            req.Accept = "text/xml";
            req.Method = "POST";

            using (Stream stm = req.GetRequestStream())
            {
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                    stmw.Write(string.Concat(soap));
                }
            }

            WebResponse response = req.GetResponse();

            Stream responseStream = response.GetResponseStream();

            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readStream = new StreamReader(responseStream, encode);
            Char[] read = new Char[256];
            int count = readStream.Read(read, 0, 256);
            while (count > 0)
            {
                String str = new String(read, 0, count);
                Console.Write(str);
                count = readStream.Read(read, 0, 256);
                if (str.Contains("<valid>true</valid>")) resultado = true;
            }
            readStream.Close();



            if (resultado) mensaje = "El NIF es válido.";
            else mensaje = "El NIF no es válido.";

            MessageBox.Show(mensaje);
            _ = Cursors.Default;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            comboBox.SelectedItem = "ES";
        }
    }
}
