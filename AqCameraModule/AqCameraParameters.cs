﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using AqDevice;

namespace AqCameraModule
{
    /// <summary>
    /// 相机源：相机参数数据结构
    /// </summary>
    [Serializable]
    public class AqCameraParameters
    {
        bool _acquisitionParamChanged = false;
        string _cameraParamPath = "";
        List<string> _cameraName = new List<string>();
        Dictionary<string, AqCameraBrand> _cameraBrand = new Dictionary<string, AqCameraBrand>();             
        Dictionary<string, string> _cameraId = new Dictionary<string, string>();
        Dictionary<string, string> _cameraIp = new Dictionary<string, string>();
        Dictionary<string, string> _cameraMac = new Dictionary<string, string>();
        Dictionary<string, AqDevice.TriggerSources> _cameraTriggerSource = new Dictionary<string, AqDevice.TriggerSources>();
        Dictionary<string, AqDevice.TriggerSwitchs> _cameraTriggerSwitch = new Dictionary<string, AqDevice.TriggerSwitchs>();
        Dictionary<string, AqDevice.TriggerModes> _cameraTriggerMode = new Dictionary<string, AqDevice.TriggerModes>();
        Dictionary<string, AqDevice.TriggerEdges> _cameraTriggerEdge = new Dictionary<string, AqDevice.TriggerEdges>();
        Dictionary<string, double> _cameraExposureTime = new Dictionary<string, double>();
        Dictionary<string, double> _cameraAcquisitionFrequency = new Dictionary<string, double>();
        Dictionary<string, double> _cameraTriggerDelay = new Dictionary<string, double>();
        Dictionary<string, double> _cameraGain = new Dictionary<string, double>();
        Dictionary<string, bool> _cameraGainAuto = new Dictionary<string, bool>();
        Dictionary<string, Int64> _cameraImageWidth = new Dictionary<string, Int64>();
        Dictionary<string, Int64> _cameraImageHeight = new Dictionary<string, Int64>();
        Dictionary<string, Int64> _cameraImageOffsetX = new Dictionary<string, Int64>();
        Dictionary<string, Int64> _cameraImageOffsetY = new Dictionary<string, Int64>();

        public bool AcquisitionParamChanged
        {
            get { return _acquisitionParamChanged; }
            set { _acquisitionParamChanged = value; }
        }

        public string CameraParamPath
        {
            get => _cameraParamPath;
            set => _cameraParamPath = value;
        }

        //相机参数：对应SDK开发接口      
        public List<string> CameraName
        {
            get { return _cameraName; }
            set
            {
                _cameraName = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, AqCameraBrand> CameraBrand
        {
            get { return _cameraBrand; }
            set
            {
                _cameraBrand = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, string> CameraId
        {
            get { return _cameraId; }
            set {
                _cameraId = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, string> CameraIp
        {
            get { return _cameraIp; }
            set {
                _cameraIp = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, string> CameraMac
        {
            get { return _cameraMac; }
            set {
                _cameraMac = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, AqDevice.TriggerSources> CameraTriggerSource
        {
            get { return _cameraTriggerSource; }
            set
            {
                _cameraTriggerSource = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, AqDevice.TriggerSwitchs> CameraTriggerSwitch
        {
            get { return _cameraTriggerSwitch; }
            set
            {
                _cameraTriggerSwitch = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, AqDevice.TriggerModes> CameraTriggerMode
        {
            get { return _cameraTriggerMode; }
            set
            {
                _cameraTriggerMode = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, AqDevice.TriggerEdges> CameraTriggerEdge
        {
            get { return _cameraTriggerEdge; }
            set
            {
                _cameraTriggerEdge = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, double> CameraExposureTime
        {
            get { return _cameraExposureTime; }
            set
            {
                _cameraExposureTime = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, double> CameraAcquisitionFrequency
        {
            get { return _cameraAcquisitionFrequency; }
            set
            {
                _cameraAcquisitionFrequency = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, double> CameraTriggerDelay
        {
            get { return _cameraTriggerDelay; }
            set
            {
                _cameraTriggerDelay = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, double> CameraGain
        {
            get { return _cameraGain; }
            set
            {
                _cameraGain = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, bool> CameraGainAuto
        {
            get { return _cameraGainAuto; }
            set
            {
                _cameraGainAuto = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, Int64> CameraImageWidth
        {
            get { return _cameraImageWidth; }
            set
            {
                _cameraImageWidth = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, Int64> CameraImageHeight
        {
            get { return _cameraImageHeight; }
            set
            {
                _cameraImageHeight = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, Int64> CameraImageOffsetX
        {
            get { return _cameraImageOffsetX; }
            set
            {
                _cameraImageOffsetX = value;
                AcquisitionParamChanged = false;
            }
        }

        public Dictionary<string, Int64> CameraImageOffsetY
        {
            get { return _cameraImageOffsetY; }
            set
            {
                _cameraImageOffsetY = value;
                AcquisitionParamChanged = false;
            }
        }

        public void SerializeAndSave()
        {
            FileStream fileStream = new FileStream(CameraParamPath, FileMode.Create);
            BinaryFormatter binary = new BinaryFormatter();
            binary.Serialize(fileStream, this);
            fileStream.Close();
        }

        public AqCameraParameters DeSerializeAndRead()
        {
            FileStream fileStream = new FileStream(CameraParamPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryFormatter binary = new BinaryFormatter();
            AqCameraParameters param = binary.Deserialize(fileStream) as AqCameraParameters;
            fileStream.Close();
            return param;
        }
    }

    /// <summary>
    /// 文件源：保存文件路径
    /// </summary>
    [Serializable]
    public class AqFileParameters
    {
        string _fileParamPath = "";
        List<string> _filePath = new List<string>();
        List<string> _folderPath = new List<string>();
        List<List<string>> _folderFiles = new List<List<string>>();
        List<UInt32> _folderIndex = new List<UInt32>();
        AcquisitionMode _cameraAcqusitionMode;

        public string FileParamPath
        {
            get => _fileParamPath;
            set => _fileParamPath = value;
        }

        public List<string> FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        public List<string> FolderPath
        {
            get { return _folderPath; }
            set { _folderPath = value; }
        }

        public List<List<string>> FolderFiles
        {
            get { return _folderFiles; }
            set { _folderFiles = value; }
        }

        public AcquisitionMode CameraAcqusitionMode
        {
            get { return _cameraAcqusitionMode; }
            set { _cameraAcqusitionMode = value; }
        }


        public void SerializeAndSave()
        {
            IniFile.IniFillFullPath = FileParamPath;
            IniFile.WriteValue("Acquisition", "FilesNum", FilePath.Count);
            IniFile.WriteValue("Acquisition", "FoldersNum", FolderPath.Count);

            for (int i = 0; i < FilePath.Count; i++)
            {
                string strFilesKey = string.Format("FilesName{0}", i);
                IniFile.WriteValue("Acquisition", strFilesKey, FilePath[i]);
            }

            for (int j = 0; j < FolderPath.Count; j++)
            {
                string strFoldersKey = string.Format("FoldersName{0}", j);
                IniFile.WriteValue("Acquisition", strFoldersKey, FolderPath[j]);
            }
        }

        public AqFileParameters DeSerializeAndRead()
        {
            IniFile.IniFillFullPath = FileParamPath;
            UInt32 filesNum = Convert.ToUInt32(IniFile.ReadValue("Acquisition", "FilesNum", "0"));
            UInt32 foldersNum = Convert.ToUInt32(IniFile.ReadValue("Acquisition", "FoldersNum", "0"));

            for (int i = 0; i < filesNum; i++)
            {
                string strFilesKey = string.Format("FilesName{0}", i);
                string fileName = IniFile.ReadValue("Acquisition", strFilesKey, "NULL");
                FilePath.Add(fileName);
            }

            for (int j = 0; j < foldersNum; j++)
            {
                string strFoldersKey = string.Format("FoldersName{0}", j);
                string folderName = IniFile.ReadValue("Acquisition", strFoldersKey, "NULL");
                FolderPath.Add(folderName);
            }
            UpdateFilesUnderFolder();
            return this;
        }

        //方法：遍历添加文件夹中的文件路径到FolderFiles
        public void UpdateFilesUnderFolder()
        {
            FolderFiles.Clear();

            foreach (string folder in FolderPath)
            {
                if (folder != "")
                {
                    if (Directory.Exists(folder))
                    {
                        if (Directory.GetFiles(folder).Length != 0)
                        {
                            FolderFiles.Add(new List<string>(Directory.GetFiles(folder)));
                        }
                    }
                }
            }

        }
    }
}
