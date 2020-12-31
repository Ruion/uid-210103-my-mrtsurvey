using UnityEngine;
using UnityEngine.Events;
using System.IO.Ports;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

public class VendingMachine : WaitExtension
{
    private const float V = 2f;

    /*public byte[] test;
public byte aa;*/

    private SerialPort VMSerialPort;

    public WriteType VMSerialPortWriteType = WriteType.Byte;
    public string vmserialPortText;
    public PortName portname = PortName.COM1;
    public PortBaudrate portbaudrate = PortBaudrate.pb115200;

    [HideInInspector] public DBModelEntity vmOperateRecordDB;
    public UnityEvent onMotorTurn;
    public UnityEvent onMotorFail;

    // Start is called before the first frame update
    private void Start()
    {
        VMSerialPort = new SerialPort();
    }

    public void OpenPort()
    {
        VMSerialPort.PortName = portname.ToString();
        VMSerialPort.BaudRate = Convert.ToInt32(portbaudrate);
        VMSerialPort.Parity = Parity.None;
        VMSerialPort.DataBits = 8;
        VMSerialPort.Open();

        //        Debug.Log("Serial Port Opened: " + VMSerialPort.ToString());

        System.Threading.Thread.Sleep(2000);
    }

    #region SendToPort (int)

    public void SendToPort(int value_)
    {
        OpenPort();

        if (value_ == 0)
        {
            Debug.Log("Running motor 1");
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x00, 0x00, 0x35 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 1)
        {
            Debug.Log("Running motor 2");
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x01, 0x00, 0x36 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 2)
        {
            Debug.Log("Running motor 3");
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x02, 0x00, 0x37 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 3)
        {
            Debug.Log("Running motor 4");
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x03, 0x00, 0x38 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 4)
        {
            Debug.Log("Running motor 5");
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x04, 0x00, 0x39 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 5)
        {
            Debug.Log("Running motor 6");
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x05, 0x00, 0x3A };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 6)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x06, 0x00, 0x3B };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 7)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0A, 0x00, 0x3F };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 8)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0B, 0x00, 0x40 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 9)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0C, 0x00, 0x41 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 10)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0D, 0x00, 0x42 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 11)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0E, 0x00, 0x43 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 12)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x0F, 0x00, 0x44 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 13)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x10, 0x00, 0x45 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 14)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x14, 0x00, 0x49 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 15)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x15, 0x00, 0x4A };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 16)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x16, 0x00, 0x4B };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 17)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x17, 0x00, 0x4C };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 18)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x18, 0x00, 0x4D };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 19)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x19, 0x00, 0x4E };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 20)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x1A, 0x00, 0x4F };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 21)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x1E, 0x00, 0x53 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 22)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x1F, 0x00, 0x54 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 23)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x20, 0x00, 0x55 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 24)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x21, 0x00, 0x56 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 25)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x22, 0x00, 0x57 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 26)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x23, 0x00, 0x58 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 27)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x24, 0x00, 0x59 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 28)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x28, 0x00, 0x5D };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 29)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x29, 0x00, 0x5E };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 30)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2A, 0x00, 0x5F };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 31)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2B, 0x00, 0x60 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 32)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2C, 0x00, 0x61 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 33)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2D, 0x00, 0x62 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }
        else if (value_ == 34)
        {
            PortReset();
            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = { 0x01, 0x02, 0x31, 0x01, 0x2E, 0x00, 0x63 };
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }
        }

        ClosePort();
    }

    #endregion SendToPort (int)

    #region SendToPort (bytes)

    public void SendToPort(byte[] bytes)
    {
        if (FindObjectOfType<GameSettingEntity>().gameSettings.debugMode)
        {
            OpenPort();
            PortReset();

            if (VMSerialPortWriteType == WriteType.Byte)
            {
                byte[] SendingBytes = bytes;
                VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
            }
            else if (VMSerialPortWriteType == WriteType.String)
            {
                VMSerialPort.Write(vmserialPortText);
            }

            System.Threading.Thread.Sleep(2000);

            ClosePort();
        }
        else
        {
            Wait(V, () =>
            {
                PortReset();
                Wait(V, () =>
                {
                    if (VMSerialPortWriteType == WriteType.Byte)
                    {
                        byte[] SendingBytes = bytes;
                        VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);
                    }
                    else if (VMSerialPortWriteType == WriteType.String)
                    {
                        VMSerialPort.Write(vmserialPortText);
                    }

                    Wait(V, () =>
                    {
                        ClosePort();
                    });
                });
            });
        }
    }

    #endregion SendToPort (bytes)

    private void ClosePort()
    {
        VMSerialPort.Close();
    }

    private void PortReset()
    {
        byte[] SendingBytes = { 0xFF, 0x01, 0x01, 0x01, 0x01, 0x01, 0x03 };
        VMSerialPort.Write(SendingBytes, 0, SendingBytes.Length);

        System.Threading.Thread.Sleep(2000);
    }

    public async void TurnMotor(string id)
    {
        /* var process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "C:\\Windows\\System32\\fsutil.exe",
                    Arguments = "behavior query SymlinkEvaluation",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
        };*/

        var proc = new System.Diagnostics.Process();

        proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

        // Using PDFtoPrinter
        proc.StartInfo.FileName = "C:\\UID-APP\\Vending-Machine-Controller\\Vending-Machine-Controller.exe";
        //  proc.StartInfo.Arguments = "-motor_lane " + motor_lane;
        proc.StartInfo.Arguments = "-motor_lane motor_" + id;

        proc.StartInfo.RedirectStandardOutput = true;

        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;

        proc.Start();

        PlayerPrefs.SetString("turn_result", "Success");

        string output = await proc.StandardOutput.ReadToEndAsync();
        proc.WaitForExit();

        if (output.Contains("fail"))
        {
            PlayerPrefs.SetString("turn_result", "Fail");
            if (onMotorFail.GetPersistentEventCount() > 0) onMotorFail.Invoke();
        }

        if (proc.HasExited == false)
        {
            proc.WaitForExit(3000);
        }

        proc.EnableRaisingEvents = true;

        proc.Close();

        if (onMotorTurn.GetPersistentEventCount() > 0) onMotorTurn.Invoke();

        if (PlayerPrefs.GetString("turn_result") == "Fail") SecondTurn(id);
    }

    private async void SecondTurn(string id)
    {
        var proc = new System.Diagnostics.Process();

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "C:\\Windows\\System32\\fsutil.exe",
                Arguments = "behavior query SymlinkEvaluation",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

        proc.StartInfo.FileName = "C:\\UID-APP\\Vending-Machine-Controller.exe";
        //  proc.StartInfo.Arguments = "-motor_lane " + motor_lane;
        proc.StartInfo.Arguments = "-motor_lane motor_" + id;

        proc.StartInfo.RedirectStandardOutput = true;

        PlayerPrefs.SetString("turn_result", "Success");

        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;

        proc.Start();

        PlayerPrefs.SetString("turn_result", "Success");

        string output = await proc.StandardOutput.ReadToEndAsync();
        proc.WaitForExit();

        if (output.Contains("fail"))
        {
            PlayerPrefs.SetString("turn_result", "Fail");
            if (onMotorFail.GetPersistentEventCount() > 0) onMotorFail.Invoke();
        }

        if (proc.HasExited == false)
        {
            proc.WaitForExit(3000);
        }

        proc.EnableRaisingEvents = true;

        proc.Close();

        if (onMotorTurn.GetPersistentEventCount() > 0) onMotorTurn.Invoke();

        if (PlayerPrefs.GetString("turn_result") == "Fail") ThirdTurn(id);
    }

    private async void ThirdTurn(string id)
    {
        var proc = new System.Diagnostics.Process();

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "C:\\Windows\\System32\\fsutil.exe",
                Arguments = "behavior query SymlinkEvaluation",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            }
        };

        proc.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;

        // Using PDFtoPrinter
        proc.StartInfo.FileName = "C:\\UID-APP\\Vending-Machine-Controller.exe";
        //  proc.StartInfo.Arguments = "-motor_lane " + motor_lane;
        proc.StartInfo.Arguments = "-motor_lane motor_" + id;

        proc.StartInfo.RedirectStandardOutput = true;

        PlayerPrefs.SetString("turn_result", "Success");

        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;

        proc.Start();

        PlayerPrefs.SetString("turn_result", "Success");

        string output = await proc.StandardOutput.ReadToEndAsync();
        proc.WaitForExit();

        if (output.Contains("fail"))
        {
            PlayerPrefs.SetString("turn_result", "Fail");
            if (onMotorFail.GetPersistentEventCount() > 0) onMotorFail.Invoke();
        }

        if (proc.HasExited == false)
        {
            proc.WaitForExit(3000);
        }

        proc.EnableRaisingEvents = true;

        proc.Close();

        if (onMotorTurn.GetPersistentEventCount() > 0) onMotorTurn.Invoke();
    }
}

#region properties class

[Serializable]
public enum PortName
{
    COM1,
    COM2,
    COM3
}

[Serializable]
public enum PortBaudrate
{
    pb110 = 110,
    pb300 = 300,
    pb600 = 600,
    pb1200 = 1200,
    pb4800 = 4800,
    pb9600 = 9600,
    pb14400 = 14400,
    pb19200 = 19200,
    pb38400 = 38400,
    pb57600 = 57600,
    pb115200 = 115200,
    pb128000 = 128000,
    pb256000 = 256000
}

[Serializable]
public enum WriteType
{
    Byte,
    String
}

#endregion properties class