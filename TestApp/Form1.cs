using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ServiceReference1.SignServiceClient client = new ServiceReference1.SignServiceClient();

                var result = client.SignString(new ServiceReference1.SingStrIn()
                {
                    str_to_sign = textBox1.Text
                });
                if (result.error != null)
                    throw new Exception(result.error.message);
                textBox2.Text = result.result.signature;
             }
            catch (Exception ex)
            {
                textBox2.Text = ex.Message;
            }
        }
    }
}
