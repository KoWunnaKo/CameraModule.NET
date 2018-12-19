﻿//***************************************************************
// 文件名（File Name）：    AqAcqusitionImage.cs
//
// 数据表（Tables）：       Nothing
//
// 作者（Author）：         台琰
//
// 日期（Create Date）：    2018.12.04
//
//***************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using AqCameraModule;
using AqDevice;

namespace AqVision.Acquisition
{
    public partial class AqAcqusitionControl : UserControl
    {        
        bool _isOpened = false;
        bool _isContinue = false;
        bool _isGetBitmapSuc = false;
        AqDevice.IAqCameraManager _cameraManager = null;
        List<AqDevice.IAqCamera> _cameras;
        Dictionary<string, int> _cameraNameToIndex = new Dictionary<string, int>();
        AqCameraParameters _cameraParam = new AqCameraParameters();
        AqFileParameters _fileParam = new AqFileParameters();
        System.Drawing.Bitmap _revBitmap = null;

        public AqCameraParameters CameraParam { get => _cameraParam; set => _cameraParam = value; }
        public AqFileParameters FileParam { get => _fileParam; set => _fileParam = value; }
        public System.Drawing.Bitmap RevBitmap
        {
            get { return _revBitmap; }
            set { _revBitmap = value; }
        }

        public AqAcqusitionControl()
        {
            InitializeComponent();
            InitializeAcquisitionParam();
            InitializationControlShow();
            radioButtonCamera_CheckedChanged(null, null);
        }

        private void InitializeAcquisitionParam()
        {
            string currentPath = System.IO.Directory.GetCurrentDirectory();
            //currentPath = Application.StartupPath + "\\Config.ini"; //使用哪种获取当前路径方式待定
            string cameraParamPath = currentPath + "\\CameraData.dat";
            string imageSourcePath = currentPath + "\\ImageSource.ini";
            CameraParam = CameraParam.DeSerializeAndRead(cameraParamPath);
            FileParam = FileParam.DeSerializeAndRead(imageSourcePath);
        }

        private void InitializationControlShow()
        {
            //初始化相机列表

            //初始化文件列表
            comboBoxFile.Items.Add("新增文件");
            //初始化文件夹列表
            comboBoxFolder.Items.Add("新增文件夹");
        }

        #region 选择图像采集源
        #region From Camera
        private void radioButtonCamera_CheckedChanged(object sender, EventArgs e)
        {
            panelCamera.Enabled = true;
            panelCamerapanelLocalFile.Enabled = false;
            panelLocalFolder.Enabled = false;
            panelAcquisitionCtrl.Enabled = true;
        }

        private void buttonParameterSet_Click(object sender, EventArgs e)
        {
            AqCameraParametersSet CameraParamSet;
            CameraParamSet = new AqCameraParametersSet(ref _cameraParam);
            CameraParamSet.Show();
            CameraParamSet.Focus();
        }
        #endregion

        #region From File
        private void radioButtonLocalFile_CheckedChanged(object sender, EventArgs e)
        {
            panelCamera.Enabled = false;
            panelCamerapanelLocalFile.Enabled = true;
            panelLocalFolder.Enabled = false;
            panelAcquisitionCtrl.Enabled = false;
        }


        private void comboBoxFile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((ComboBox)sender).Text == "新增文件")
            {

            }
        }

        private void buttonLocationDirectory_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;//该值确定是否可以选择多个文件
            dialog.Title = "选择输入文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                comboBoxFile.Items.Add(dialog.FileName);
            }
        }

        private void ReArrangeComboBoxFile()
        {

        }
        #endregion

        #region From Folder
        private void radioButtonLocalFolder_CheckedChanged(object sender, EventArgs e)
        {
            panelCamera.Enabled = false;
            panelCamerapanelLocalFile.Enabled = false;
            panelLocalFolder.Enabled = true;
            panelAcquisitionCtrl.Enabled = false;
        }

        private void comboBoxFolder_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void buttonSelectFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择所有文件存放目录";
            if (folder.ShowDialog() == DialogResult.OK)
            {
                comboBoxFolder.Items.Add(folder.SelectedPath);
            }
        }

        private void ReArrangeComboBoxFolder()
        {

        }
        #endregion

        #endregion

        #region 相机控制按钮
        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (comboBoxCameraBrand.SelectedIndex == -1)
            {
                MessageBox.Show("未选择相机品牌", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenAllCamera();
        }

        private void buttonSaveImage_Click(object sender, EventArgs e)
        {

        }

        private void buttonSingle_Click(object sender, EventArgs e)
        {
            if (!_isContinue)
            {
                CameraParam.CameraTriggerMode[comboBoxCameraName.Text] = TriggerModes.Unknow;
                OpenOneStream(comboBoxCameraName.SelectedIndex);
            }
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            if (_isContinue)
            {
                buttonContinue.Text = "连续采集";
                CameraParam.CameraTriggerMode[comboBoxCameraName.Text] = TriggerModes.Unknow;
            }
            else
            {
                buttonContinue.Text = "停止采集";
                CameraParam.CameraTriggerMode[comboBoxCameraName.Text] = TriggerModes.Continuous;
            }
            OpenOneStream(comboBoxCameraName.SelectedIndex);
            _isContinue = !_isContinue;
        }
        #endregion

        #region 相机控制函数
        //采集回调
        public void RecCapture(object objUserparam, Bitmap bitmap)
        {
            RevBitmap = bitmap;
            _isGetBitmapSuc = true;

            pictureBoxImageShow.Image = bitmap;
        }

        public bool OpenAllCamera()
        {
            if (!_isOpened) 
            {
                //mark:DLL路径待修改
                string dllPath = System.IO.Directory.GetCurrentDirectory() + "\\" + comboBoxCameraBrand.Text + ".dll";
                Assembly assem = Assembly.LoadFile(dllPath);
                Type type = assem.GetType("AqDevice.AqCameraFactory");
                MethodInfo mi = type.GetMethod("GetInstance");
                object obj = mi.Invoke(null, null);

                _cameraManager = (IAqCameraManager)obj;
                _cameraManager.Init();
                _cameras = _cameraManager.GetCameras();
                _cameraNameToIndex.Clear();
                if (_cameras.Count == 0) return false;

                for (int i = 0; i < _cameras.Count; i++) 
                {
                    _cameras[i].RegisterCaptureCallback(new AqCaptureDelegate(RecCapture));
                    _cameras[i].OpenCamera();
                    _cameraNameToIndex.Add(_cameras[i].Name, i);
                }

                _isOpened = true;
            }

            return true;
        }

        public bool OpenOneStream(int index)
        {
            if (!_isOpened) return false;

            string name = _cameras[index].Name;
            if (CameraParam.CameraName.Contains(_cameras[index].Name))
            {
                _cameras[index].Id = CameraParam.CameraId[name];
                _cameras[index].Ip = CameraParam.CameraIp[name];
                _cameras[index].Mac = CameraParam.CameraMac[name];
                _cameras[index].TriggerSource = CameraParam.CameraTriggerSource[name];
                _cameras[index].TriggerSwitch = CameraParam.CameraTriggerSwitch[name];
                _cameras[index].TriggerMode = CameraParam.CameraTriggerMode[name];
                _cameras[index].TriggerEdge = CameraParam.CameraTriggerEdge[name];
                _cameras[index].ExposureTime = CameraParam.CameraExposureTime[name];
                _cameras[index].AcquisitionFrequency = CameraParam.CameraAcquisitionFrequency[name];
                _cameras[index].TriggerDelay = CameraParam.CameraTriggerDelay[name];
                _cameras[index].Gain = CameraParam.CameraGain[name];
                _cameras[index].GainAuto = CameraParam.CameraGainAuto[name];
                _cameras[index].ImageWidth = CameraParam.CameraImageWidth[name];
                _cameras[index].ImageHeight = CameraParam.CameraImageHeight[name];
                _cameras[index].ImageoffsetX = CameraParam.CameraImageOffsetX[name];
                _cameras[index].ImageoffsetY = CameraParam.CameraImageOffsetY[name];
            }
            else
            {
                CameraParam.CameraName.Add(_cameras[index].Name);
            }

            _cameras[index].OpenStream();

            return true;
        }

        public bool CloseAllCamera()
        {
            try
            {
                if(_isOpened)
                {
                    for (int i = 0; i < _cameras.Count; i++)
                    {
                        _cameras[i].CloseCamera();
                    }
                }
                _isOpened = false;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("IntegrationTesting DisConnect error " + ex.Message);
                //Mark:此处加入log
            }
            return true;
        }

        public bool Connect()
        {
            try
            {
                //preservation and wait for use.
            }
            catch (FormatException ex)
            {
                System.Windows.Forms.MessageBox.Show("Camera Connect Format error " + ex.Message);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Camera Connect error " + ex.Message);
            }

            return true;
        }

        public bool AcquisitionCamera(ref List<System.Drawing.Bitmap> acquisitionBmp, List<string> acquisitionCameraName)
        {
            try
            {
                GC.Collect();
                for (int i = 0; i < acquisitionCameraName.Count; i++)
                {
                    if (CameraParam.AcquisitionParamChanged)
                    {
                        for (int j = 0; j < _cameras.Count; j++)
                        {
                            OpenOneStream(j);
                        }
                        CameraParam.AcquisitionParamChanged = false;                        
                    }

                    if (_cameras.Count < acquisitionCameraName.Count) return false;

                    _isGetBitmapSuc = false;

                    _cameras[_cameraNameToIndex[acquisitionCameraName[i]]].TriggerSoftware();
                    while (!_isGetBitmapSuc)
                    {
                        Thread.Sleep(10);//等待采集回调
                    }
                    acquisitionBmp.Add(RevBitmap);
                }

                return true;
            }
            catch(Exception ex)
            {
                //Mark:加入log
                return false;
            } 
        }

        //Index=0采集所有保存的文件路径
        public bool AcquisitionFile(ref List<System.Drawing.Bitmap> acquisitionBmp, int[] index)
        {
            foreach (int key in index) 
            {
                acquisitionBmp.Add(Image.FromFile(FileParam.InputFile[key]) as Bitmap);
            }
            return true;
        }

        //Index=0采集所有保存的文件夹路径
        public bool AcquisitionFolder(ref List<System.Drawing.Bitmap> acquisitionBmp, int[] index)
        {
            foreach (int key in index)
            {
                foreach(string file in FileParam.FolderFiles[key])
                {
                    acquisitionBmp.Add(Image.FromFile(file) as Bitmap);
                }
            }
            return true;
        }
        #endregion
    }
}
