using ActUtlType64Lib;

namespace MxComponentForm
{
    public partial class Form1 : Form
    {
        int a = 10;


        ActUtlType64 mxComponent;
        bool isConnected = false;

        // 생성자(Constructor)
        public Form1()
        {
            InitializeComponent();

            // Mx Component 객체(instance) 생성
            mxComponent = new ActUtlType64();

            InitializeButtons();
        }

        // 파괴자(Destructor)
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
                        errorMsg += ": GX Simulator2를 기동해 주세요.";
                        break;
                }

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                isConnected = true;

                InitializeButtons();

                MessageBox.Show("시뮬레이터와 연결이 되었습니다.", "OK", MessageBoxButtons.OK);
            }
        }

        private void Close_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("시뮬레이터와 연결을 해주세요.", "주의", MessageBoxButtons.OK
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
                        errorMsg += ": GX Simulator2를 기동해 주세요.";
                        break;
                }

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                isConnected = false;

                MessageBox.Show("시뮬레이터연결이 해제되었습니다.", "OK", MessageBoxButtons.OK);
            }
        }


        private void GetDevice_Click(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                MessageBox.Show("시뮬레이터와 연결을 해주세요.", "주의", MessageBoxButtons.OK
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
                        errorMsg += ": GX Simulator2를 기동해 주세요.";
                        break;
                }

                MessageBox.Show(errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
        }

        private void SetDevice_Click(object sender, EventArgs e)
        {
            int result;
            // 들어온 입력이 정수 형태인지 확인하는 함수
            bool isParsed = Int32.TryParse(textBox2.Text, out result);

            if (isParsed)
            {
                int iRet = mxComponent.SetDevice(textBox1.Text, result);

                CheckError(iRet);

                string resultStr = textBox1.Text + "의 값이 " + result + "로 변경되었습니다.";
                Value.Text = resultStr;
            }
            else
            {
                Value.Text = "Value값이 잘못 입력되었습니다. 숫자를 입력해 주세요.";
            }
        }

        private void ReadDeviceBlock_Click(object sender, EventArgs e)
        {
            int restult;
            bool isParsed = Int32.TryParse(textBox4.Text, out restult);

            // 블록 값이 정수형으로 잘 들어왔을 경우
            if (isParsed)
            {
                int[] data = new int[restult];
                // data[0]에서 0은 처음 주소를 뜻함
                int iRet = mxComponent.ReadDeviceBlock(textBox3.Text, restult, out data[0]);

                CheckError(iRet);

                string total = "";
                for (int i = 0; i < restult; i++)
                {
                    total += data[i] + ",";
                }

                Value2.Text = textBox3.Text + "부터 " + restult.ToString() + "개의" +
                    " 블록의 값: " + total;
            }
            else
            {
                Value2.Text = "블록 값이 정수인지 확인해 주세요.";
            }

        }

        private void WriteDeviceBlock_Click(object sender, EventArgs e)
        {
            string address = textBox3.Text;
            string numOfBlock = textBox4.Text;
            string data = textBox5.Text;

            // 1. 주소 예외처리
            // X, Y, M, D, T, C ...
            char[] charArray = { 'x', 'y', 'm', 'd', 't', 'c', 'X', 'Y', 'M', 'D', 'T', 'C' };
            bool containsAny = address.Any(c => charArray.Contains(c));

            if (!containsAny)
            {
                Value2.Text = "X, Y, M, D, T, C 중 하나가 포함된\n주소의 첫번째 값을 입력해 주세요.";
                return;
            }

            // 2. 블록 개수 예외처리
            int convertedNumOfBlock;
            bool isParsed = Int32.TryParse(numOfBlock, out convertedNumOfBlock);

            if(!isParsed)
            {
                Value2.Text = "블록개수를 잘못 입력하셨습니다.\n정수형으로 입력해 주세요.";
                return;
            }

            // 3. 데이터 예외처리
            string[] dataArr = data.Split(",");

            if(convertedNumOfBlock != dataArr.Length)
            {
                Value2.Text = $"블록개수{convertedNumOfBlock}와 값{data}의 개수{dataArr.Length}가 다릅니다.\n" +
                    "다시 입력해 주세요.(예. 블록개수 3, 값 33,55,500)";
                return;
            }

            int[] values = new int[dataArr.Length];
            // int[] values = new int[5]{ 55, 33, 500, 22, 2 }; // 55,33,500
            for (int i = 0; i < values.Length; i++)
            {
                bool isParsed2 = Int32.TryParse(dataArr[i], out values[i]);

                if (!isParsed2)
                {
                    Value2.Text = "값이 잘못되었습니다.\n다시 입력해 주세요.(예. 22,55,500)";
                    return;
                }
            }

            int iRet = mxComponent.WriteDeviceBlock(address, convertedNumOfBlock, ref values[0]);
            
            CheckError(iRet);

            Value2.Text = $"{address}부터 {numOfBlock}개의 블록에 {data}가 입력되었습니다";
        }
    }
}
