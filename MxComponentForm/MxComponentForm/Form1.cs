using ActUtlType64Lib;

namespace MxComponentForm
{
    public partial class Form1 : Form
    {
        ActUtlType64 mxComponent;
        bool isConnected = false;

        // ������(Constructor)
        public Form1()
        {
            InitializeComponent();

            // Mx Component ��ü(instance) ����
            mxComponent = new ActUtlType64();

            InitializeButtons();
        }

        // �ı���(Destructor)
        ~Form1()
        {
            mxComponent.Close();
        }

        private void InitializeButtons()
        {
            GetDevice.Enabled = isConnected;
            SetDevice.Enabled = isConnected;
        }

        private void Open_Click(object sender, EventArgs e)
        {
            int iRet = mxComponent.Open();

            if (iRet != 0)
            {
                string errorMsg = Convert.ToString(iRet, 16);

                switch (errorMsg)
                {
                    case "1809001":
                        errorMsg += ": GX Simulator2�� �⵿�� �ּ���.";
                        break;
                }

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                isConnected = true;

                InitializeButtons();

                MessageBox.Show("�ùķ����Ϳ� ������ �Ǿ����ϴ�.", "OK", MessageBoxButtons.OK);
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("�ùķ����Ϳ� ������ ���ּ���.", "����", MessageBoxButtons.OK
                    , MessageBoxIcon.Exclamation);

                return;
            }

            int iRet = mxComponent.Close();

            if (iRet != 0)
            {
                string errorMsg = Convert.ToString(iRet, 16);

                switch (errorMsg)
                {
                    case "1809001":
                        errorMsg += ": GX Simulator2�� �⵿�� �ּ���.";
                        break;
                }

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                isConnected = false;

                MessageBox.Show("�ùķ����Ϳ����� �����Ǿ����ϴ�.", "OK", MessageBoxButtons.OK);
            }
        }


        private void GetDevice_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("�ùķ����Ϳ� ������ ���ּ���.", "����", MessageBoxButtons.OK
                    , MessageBoxIcon.Exclamation);

                return;
            }

            int output = 0;
            int iRet = mxComponent.GetDevice(textBox1.Text, out output);

            CheckError(iRet);

            if (iRet == 0)
            {
                Value.Text = "Output: " + output.ToString();
            }
        }

        private static void CheckError(int iRet)
        {
            if (iRet != 0)
            {
                string errorMsg = Convert.ToString(iRet, 16);

                switch (errorMsg)
                {
                    case "1809001":
                        errorMsg += ": GX Simulator2�� �⵿�� �ּ���.";
                        break;
                }

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
        }

        private void SetDevice_Click(object sender, EventArgs e)
        {
            int result;
            // ���� �Է��� ���� �������� Ȯ���ϴ� �Լ�
            bool isParsed = Int32.TryParse(textBox2.Text, out result);

            if (isParsed)
            {
                int iRet = mxComponent.SetDevice(textBox1.Text, result);

                CheckError(iRet);

                string resultStr = textBox1.Text + "�� ���� " + result + "�� ����Ǿ����ϴ�.";
                Value.Text = resultStr;
            }
            else
            {
                Value.Text = "Value���� �߸� �ԷµǾ����ϴ�. ���ڸ� �Է��� �ּ���.";
            }
        }

        private void ReadDeviceBlock_Click(object sender, EventArgs e)
        {
            int restult;
            bool isParsed = Int32.TryParse(textBox4.Text, out restult);

            // ��� ���� ���������� �� ������ ���
            if (isParsed)
            {
                int[] data = new int[restult];
                // data[0]���� 0�� ó�� �ּҸ� ����
                int iRet = mxComponent.ReadDeviceBlock(textBox3.Text, restult, out data[0]);

                CheckError(iRet);

                string total = "";
                for (int i = 0; i < restult; i++)
                {
                    total += data[i] + ",";
                }

                Value2.Text = textBox3.Text + "���� " + restult.ToString() + "����" +
                    " ����� ��: " + total;
            }
            else
            {
                Value2.Text = "��� ���� �������� Ȯ���� �ּ���.";
            }

        }

        private void WriteDeviceBlock_Click(object sender, EventArgs e)
        {
            string address = textBox3.Text;
            string numOfBlock = textBox4.Text;
            string data = textBox5.Text;

            // 1. �ּ� ����ó��
            // X, Y, M, D, T, C ...
            char[] charArray = { 'x', 'y', 'm', 'd', 't', 'c', 'X', 'Y', 'M', 'D', 'T', 'C' };
            bool containsAny = address.Any(c => charArray.Contains(c));

            if (!containsAny)
            {
                Value2.Text = "X, Y, M, D, T, C �� �ϳ��� ���Ե�\n�ּ��� ù��° ���� �Է��� �ּ���.";
                return;
            }

            // 2. ��� ���� ����ó��
            int convertedNumOfBlock;
            bool isParsed = Int32.TryParse(numOfBlock, out convertedNumOfBlock);

            if(!isParsed)
            {
                Value2.Text = "��ϰ����� �߸� �Է��ϼ̽��ϴ�.\n���������� �Է��� �ּ���.";
                return;
            }

            // 3. ������ ����ó��
            string[] dataArr = data.Split(",");

            if(convertedNumOfBlock != dataArr.Length)
            {
                Value2.Text = $"��ϰ���{convertedNumOfBlock}�� ��{data}�� ����{dataArr.Length}�� �ٸ��ϴ�.\n" +
                    "�ٽ� �Է��� �ּ���.(��. ��ϰ��� 3, �� 33,55,500)";
                return;
            }

            int[] values = new int[dataArr.Length];
            // int[] values = new int[5]{ 55, 33, 500, 22, 2 }; // 55,33,500
            for (int i = 0; i < values.Length; i++)
            {
                bool isParsed2 = Int32.TryParse(dataArr[i], out values[i]);

                if (!isParsed2)
                {
                    Value2.Text = "���� �߸��Ǿ����ϴ�.\n�ٽ� �Է��� �ּ���.(��. 22,55,500)";
                    return;
                }
            }

            int iRet = mxComponent.WriteDeviceBlock(address, convertedNumOfBlock, ref values[0]);
            
            CheckError(iRet);

            Value2.Text = $"{address}���� {numOfBlock}���� ��Ͽ� {data}�� �ԷµǾ����ϴ�";
        }
    }
}
