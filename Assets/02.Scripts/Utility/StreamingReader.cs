using BackEnd;
using Game.Debbug;
using NSY;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// </summary>
/// 작성일자   : 2024-06-23
/// 작성자     : 성엽 (shtjdduq123@naver.com)
/// 클래스용도 : streaming 폴더 데이터 저장 / 로드관련
/// /// </summary>

public class StreamingReader : CSVReader
{
    public static string filePathLocalData;
    public static string filePathUserData;
    public static string filePathInventoryData;
    public static string filePathMailData;
    public static string filePathHunterItemData;
    public static string filePathRankData;

    [HideInInspector] public static bool isLoadedUserData = false;
    public static string inDateUserData;
    public static string inDateInventoryData;
    public static string inDateMailData;
    public static string inDateHunterItemData;
    public static string inDateRankData;
    public static void PathInit()
    {
        filePathLocalData = GetPath("LocalData.dat");
        filePathUserData = GetPath("User.dat");
        filePathInventoryData = GetPath("Inventory.dat");
        filePathMailData = GetPath("Mail.dat");
        filePathHunterItemData = GetPath("HunterItem.dat");
        filePathRankData = GetPath("Rank.dat");
    }
    public static string GetPath()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.persistentDataPath;
            return path;
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Application.persistentDataPath;
            return path;
        }
        else
        {
            string path = Application.dataPath + "/StreamingAssets";
            return path;
        }
    }
    public static void SaveAllData()
    {
        SaveLocalData(false);

        if (!BackendManager.Instance.IsLocal)
        {
            SaveAllWithTransaction();
            RankManager.Instance.UpdateRankData();
        }
        else
        {
            SaveMailData(false);//메일데이터 저장
            SaveUserData(false);//유저제이터 저장
            SaveHunterItemData(false);// 유저 슬롯데이터 저장
            SaveRankData(false);//랭크데이터 세이브
            //인벤토리 저장 (이젠 미사용)
            //SaveInventoryData(false);
        }
        SystemNoticeManager.Instance.SystemNoticeInEditor("‘AllData’ 데이터 저장 성공!", Utill_Enum.SystemNoticeType.NoBackground);
    }

    /// <summary>
    /// 트랜잭션을 사용하여 모든 유저 정보 저장
    /// </summary>
    public static void SaveAllWithTransaction()
    {
        if (BackendManager.Instance.IsLocal) return;

        PlayerDataTransactionWrite transactionWrite = new PlayerDataTransactionWrite();

        SetSaveDataWithTransaction(transactionWrite, ESaveType.User, "User", ref inDateUserData);
        SetSaveDataWithTransaction(transactionWrite, ESaveType.Inventory, "Inventory", ref inDateInventoryData);
        SetSaveDataWithTransaction(transactionWrite, ESaveType.HunterItem, "HunterItem", ref inDateHunterItemData);
        SetSaveDataWithTransaction(transactionWrite, ESaveType.Mail, "Mail", ref inDateMailData);
        SetSaveDataWithTransaction(transactionWrite, ESaveType.Rank, "Rank", ref inDateRankData);

        BackendReturnObject bro = Backend.PlayerData.TransactionWrite(transactionWrite);
        if (bro.IsSuccess())
        {
            Debbuger.Debug($"[Transaction] Server Save Data: {ESaveType.User}, {ESaveType.Inventory}");
            //SystemNoticeManager.Instance.SystemNoticeInEditor("트랜잭션 저장 성공!", Utill_Enum.SystemNoticeType.NoBackground);
        }
        else
        {
            BackendErrorManager.Instance.SettingPopUp(bro);
            Debbuger.Debug($"[Transaction] SaveAll False : {bro.GetStatusCode()}, {bro.GetMessage()}");
            //SystemNoticeManager.Instance.SystemNoticeInEditor("트랜잭션 저장 실패...", Utill_Enum.SystemNoticeType.NoBackground);
        }
    }

    /// <summary>
    /// 저장 타입에 따라 트랜잭션 Add
    /// </summary>
    /// <param name="transactionWrite">트랜잭션 객체</param>
    /// <param name="eSaveType">저장 타입</param>
    /// <param name="tableName">저장할 테이블 이름</param>
    /// <param name="inDate">저장할 테이블의 inDate</param>
    public static void SetSaveDataWithTransaction(PlayerDataTransactionWrite transactionWrite, ESaveType eSaveType, string tableName, ref string inDate)
    {
        try
        {
            var param = NSY.DataManager.Instance.ServerSave(eSaveType);

            if (string.IsNullOrEmpty(inDate)) //inDate 없을 시 따로 Insert하여 inDate 가져옴
            {
                BackendReturnObject returnValue = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.Insert(tableName, param));

                if (returnValue.IsSuccess())
                {
                    var json = returnValue.GetReturnValuetoJSON();
                    inDate = json["inDate"].ToString();

                    Debbuger.Debug("Server Save Data: " + eSaveType.ToString());
                }
                else
                {
                }
            }
            else
            {
                transactionWrite.AddUpdateMyData(tableName, inDate, param);
            }
        }
        catch (Exception ex)
        {
            Debbuger.ErrorDebug($"An error occurred while saving data: {ex.Message}");
        }
    }
    public static string GetPath(string map)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            string path = Application.persistentDataPath + "/";
            path += map;
            return path;
        }
        else if (Application.platform == RuntimePlatform.Android)
        {
            string path = Path.Combine(Application.persistentDataPath, map);
            return path;
        }
        else
        {
            string path = Application.dataPath + "/StreamingAssets/";
            path = path.Substring(0, path.LastIndexOf('/'));
            return Path.Combine(path, map);
        }
    }

    /// <summary>
    /// 테스트용으로 모든 데이터를 삭제하는 함수
    /// </summary>
    public static void DeleteUserData()
    {
        if (Directory.Exists(GetPath()))
        {
            // 디렉토리에 있는 모든 파일의 경로를 배열로 가져옴
            string[] files = Directory.GetFiles(GetPath());
            // 각 파일을 삭제
            foreach (string file in files)
            {
                try
                {
                    File.Delete(file);
                    Debug.Log($"File deleted successfully: {file}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to delete file: {file}. Error: {e.Message}");
                }
            }
        }
        else
        {
            Debug.LogWarning("Directory does not exist.");
        }

    }

    public static void LoadLocalData()
    {
        if (File.Exists(filePathLocalData))
        {
            try
            {
                using (var file = File.OpenRead(filePathLocalData))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);

                    string json = Encoding.UTF8.GetString(bytes);
                    if (json.StartsWith("{") && json.EndsWith("}"))
                    {
                        NSY.DataManager.Instance.Load(json: json, jsonData: null, _isLocal: true, eSaveType: ESaveType.LocalData);
                        Debbuger.Debug("Read data from file: " + json);
                    }
                    else
                    {
                        Debbuger.ErrorDebug("Invalid data format in the file: " + json);
                    }
                }
            }
            catch (IOException ex)
            {
                Debbuger.ErrorDebug("Error loading local data: " + ex.Message);
            }
        }
        else
        {
            //데이터 없을시 필수 데이터 초기화
            NSY.DataManager.Instance.InitLoad(ESaveType.LocalData);

            Debbuger.Debug("File does not exist: " + filePathLocalData);
        }
    }
    public static void SaveLocalData(bool isShowSystemNotice = true)
    {
        ESaveType curSaveType = ESaveType.LocalData;
        try
        {
            if (!Directory.Exists(GetPath()))
            {
                Directory.CreateDirectory(GetPath());

                Debbuger.Debug("Create New Directory: " + curSaveType.ToString());
            }
            if (File.Exists(filePathLocalData))
            {
                File.Delete(filePathLocalData);
            }
            using (var file = File.Create(filePathLocalData))
            {
                string savedData = NSY.DataManager.Instance.Save(true, curSaveType);
                byte[] buffer = Encoding.UTF8.GetBytes(savedData);
                file.Write(buffer, 0, buffer.Length);

                Debbuger.Debug("Local Save Data: " + curSaveType.ToString());
                if(isShowSystemNotice)
                    DataManager.Instance.AddSystemNotice(curSaveType.ToString(), true);
            }
        }
        catch (IOException ex)
        {
            Debbuger.ErrorDebug($"Error saving {curSaveType} data: " + ex.Message);
            DataManager.Instance.AddSystemNotice(curSaveType.ToString(), false);
        }
    }


    public static void LoadUserData()
    {
        if (BackendManager.Instance.IsLocal)
        {
            if (File.Exists(filePathUserData))
            {
                try
                {
                    using (var file = File.OpenRead(filePathUserData))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);

                        string json = Encoding.UTF8.GetString(bytes);
                        if (json.StartsWith("{") && json.EndsWith("}"))
                        {
                            NSY.DataManager.Instance.Load(json: json, jsonData: null, _isLocal: true, eSaveType: ESaveType.User);
                            Debbuger.Debug("Read data from file: " + json);
                        }
                        else
                        {
                            Debbuger.ErrorDebug("Invalid data format in the file: " + json);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Debbuger.ErrorDebug("Error loading user data: " + ex.Message);
                }
            }
            else
            {
                //데이터 없을시 필수 데이터 초기화
                NSY.DataManager.Instance.InitLoad(ESaveType.User);

                Debbuger.Debug("File does not exist: " + filePathUserData);
            }
        }

        if (BackendManager.Instance.IsLocal == false)
        {
            BackendReturnObject data = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.GetMyData("User", new Where(), 1));

            if (data.IsSuccess())
            {
                var json = data.Rows();
                if (json.Count <= 0)
                {
                    return;
                }
                inDateUserData = json[0]["inDate"]["S"].ToString();
                NSY.DataManager.Instance.Load(json: null, jsonData: json[0], _isLocal: false, eSaveType: ESaveType.User);
            }
            Debbuger.Debug(inDateUserData);
        }
    }
    public static void SaveUserData(bool isShowSystemNotice = true)
    {
        ESaveType saveType = ESaveType.User;
        if (BackendManager.Instance.IsLocal)
        {
            try
            {
                if (!Directory.Exists(GetPath()))
                {
                    Directory.CreateDirectory(GetPath());

                    Debbuger.Debug("Create New Directory: " + saveType.ToString());
                }
                if (File.Exists(filePathUserData))
                {
                    File.Delete(filePathUserData);
                }
                using (var file = File.Create(filePathUserData))
                {
                    string savedData = NSY.DataManager.Instance.Save(true, saveType);
                    byte[] buffer = Encoding.UTF8.GetBytes(savedData);
                    file.Write(buffer, 0, buffer.Length);

                    Debbuger.Debug("Local Save Data: " + saveType.ToString());
                    if(isShowSystemNotice)
                        DataManager.Instance.AddSystemNotice(saveType.ToString(), true);
                }
            }
            catch (IOException ex)
            {
                Debbuger.ErrorDebug($"Error saving {saveType} data: " + ex.Message);
                DataManager.Instance.AddSystemNotice(saveType.ToString(), false);
            }
        }

        try
        {
            if (BackendManager.Instance.IsLocal == false)
            {
                var param = NSY.DataManager.Instance.ServerSave(saveType);
                if (string.IsNullOrEmpty(inDateUserData))
                {
                    BackendReturnObject returnValue = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.Insert("User", param));
                    if (returnValue.IsSuccess())
                    {
                        var json = returnValue.GetReturnValuetoJSON();
                        inDateUserData = json["inDate"].ToString();
                        Debbuger.Debug("Server Save Data: " + saveType.ToString());
                        if (isShowSystemNotice)
                            DataManager.Instance.AddSystemNotice(saveType.ToString(), true);
                    }
                }
                else
                {
                    Backend.GameData.UpdateV2("User", inDateUserData, BackendManager.Instance.OwnerIndate, param, callback =>
                    {
                        BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.UpdateV2("User", inDateUserData, BackendManager.Instance.OwnerIndate, param), false, callback);
                        if (callback.IsSuccess())
                        {
                            Debbuger.Debug("Server Save Data: " + saveType.ToString());
                            if (isShowSystemNotice)
                                DataManager.Instance.AddSystemNotice(saveType.ToString(), true);
                        }
                        else
                        {
                            Debbuger.Debug($"Error saving {saveType} data: {callback.GetStatusCode()}, {callback.GetMessage()}");
                            DataManager.Instance.AddSystemNotice(saveType.ToString(), false);
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debbuger.ErrorDebug($"Error saving {saveType} data: " + ex.Message);
            DataManager.Instance.AddSystemNotice(saveType.ToString(), false);
        }
    }

    [Obsolete("아이템 관련 로드시엔 LoadHunterItemData사용 바람")]
    public static void LoadInventoryData()
    {
        if (BackendManager.Instance.IsLocal)
        {
            if (File.Exists(filePathInventoryData))
            {
                try
                {
                    using (var file = File.OpenRead(filePathInventoryData))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);

                        string json = Encoding.UTF8.GetString(bytes);
                        if (json.StartsWith("{") && json.EndsWith("}"))
                        {
                            NSY.DataManager.Instance.Load(json: json, jsonData: null, _isLocal: true, eSaveType: ESaveType.Inventory);
                            Debbuger.Debug("Read data from file: " + json);
                        }
                        else
                        {
                            Debbuger.ErrorDebug("Invalid data format in the file: " + json);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Debbuger.ErrorDebug("Error loading Inventory data: " + ex.Message);
                }
            }
            else
            {
                Debbuger.Debug("File does not exist: " + filePathInventoryData);
            }
        }

        if (BackendManager.Instance.IsLocal == false)
        {
            BackendReturnObject data = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.GetMyData("Inventory", new Where(), 1));

            if (data.IsSuccess())
            {
                var json = data.Rows();
                if (json.Count <= 0)
                {
                    return;
                }
                inDateInventoryData = json[0]["inDate"]["S"].ToString();
                NSY.DataManager.Instance.Load(json: null, jsonData: json[0], _isLocal: false, eSaveType: ESaveType.Inventory);
            }
            Debbuger.Debug(inDateInventoryData);
        }
    }

    [Obsolete("아이템 관련 저장시엔 SaveHunterItemData사용 바람")]
    public static void SaveInventoryData(bool isShowSystemNotice = true)
    {
        if (BackendManager.Instance.IsLocal)
        {
            try
            {
                if (!Directory.Exists(GetPath()))
                {
                    Directory.CreateDirectory(GetPath());

                    Debbuger.Debug("Local Save Data: " + ESaveType.Inventory.ToString());
                    SystemNoticeManager.Instance.SystemNoticeInEditor("인벤토리 로컬 데이터 저장 성공!", Utill_Enum.SystemNoticeType.NoBackground);
                }
                if (File.Exists(filePathInventoryData))
                {
                    File.Delete(filePathInventoryData);
                }
                using (var file = File.Create(filePathInventoryData))
                {
                    string savedData = NSY.DataManager.Instance.Save(true, ESaveType.Inventory);
                    byte[] buffer = Encoding.UTF8.GetBytes(savedData);
                    file.Write(buffer, 0, buffer.Length);

                    Debbuger.Debug("Local Save Data: " + ESaveType.Inventory.ToString());
                    SystemNoticeManager.Instance.SystemNoticeInEditor("인벤토리 로컬 데이터 저장 성공!", Utill_Enum.SystemNoticeType.NoBackground);
                }
            }
            catch (IOException ex)
            {
                Debbuger.ErrorDebug("Error saving user data: " + ex.Message);
                SystemNoticeManager.Instance.SystemNoticeInEditor("인벤토리 로컬 데이터 저장 중 오류 발생", Utill_Enum.SystemNoticeType.NoBackground);
            }
        }

        try
        {
            if (BackendManager.Instance.IsLocal == false)
            {
                var param = NSY.DataManager.Instance.ServerSave(ESaveType.Inventory);
                if (string.IsNullOrEmpty(inDateInventoryData))
                {
                    BackendReturnObject returnValue = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.Insert("Inventory", param));
                    if (returnValue.IsSuccess())
                    {
                        var json = returnValue.GetReturnValuetoJSON();
                        inDateInventoryData = json["inDate"].ToString();
                        Debbuger.Debug("Server Save Data: " + ESaveType.Inventory.ToString());
                        SystemNoticeManager.Instance.SystemNoticeInEditor("인벤토리 서버 데이터 저장 성공!", Utill_Enum.SystemNoticeType.NoBackground);
                    }
                }
                else
                {
                    Backend.GameData.UpdateV2("Inventory", inDateInventoryData, BackendManager.Instance.OwnerIndate, param, callback =>
                    {
                        BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.UpdateV2("Inventory", inDateInventoryData, BackendManager.Instance.OwnerIndate, param), false, callback);
                        if (callback.IsSuccess())
                        {
                            Debbuger.Debug("Server Save Data: " + ESaveType.Inventory.ToString());
                            SystemNoticeManager.Instance.SystemNoticeInEditor("인벤토리 서버 데이터 저장 성공!", Utill_Enum.SystemNoticeType.NoBackground);
                        }
                        else
                        {
                            Debbuger.Debug($"InventoryData Save False : {callback.GetStatusCode()}, {callback.GetMessage()}");
                            SystemNoticeManager.Instance.SystemNoticeInEditor("인벤토리 서버 데이터 저장 실패...", Utill_Enum.SystemNoticeType.NoBackground);
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debbuger.ErrorDebug($"An error occurred while saving Inventory data: {ex.Message}");
            SystemNoticeManager.Instance.SystemNoticeInEditor("인벤토리 서버 데이터 저장 중 오류 발생", Utill_Enum.SystemNoticeType.NoBackground);
        }
    }

    public static void LoadMailData()
    {
        if (BackendManager.Instance.IsLocal)
        {
            if (File.Exists(filePathMailData))
            {
                try
                {
                    using (var file = File.OpenRead(filePathMailData))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);

                        string json = Encoding.UTF8.GetString(bytes);
                        if (json.StartsWith("{") && json.EndsWith("}"))
                        {
                            NSY.DataManager.Instance.Load(json: json, jsonData: null, _isLocal: true, eSaveType: ESaveType.Mail);
                            Debbuger.Debug("Read data from file: " + json);
                        }
                        else
                        {
                            Debbuger.ErrorDebug("Invalid data format in the file: " + json);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Debbuger.ErrorDebug("Error loading user data: " + ex.Message);
                }
            }
            else
            {
                //데이터 없을시 필수 데이터 초기화
                NSY.DataManager.Instance.InitLoad(ESaveType.Mail);

                Debbuger.Debug("File does not exist: " + filePathMailData);
            }
        }

        if (BackendManager.Instance.IsLocal == false)
        {
            BackendReturnObject data = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.GetMyData("Mail", new Where(), 1));

            if (data.IsSuccess())
            {
                var json = data.Rows();
                if (json.Count <= 0)
                {
                    return;
                }
                inDateUserData = json[0]["inDate"]["S"].ToString();
                NSY.DataManager.Instance.Load(json: null, jsonData: json[0], _isLocal: false, eSaveType: ESaveType.Mail);
            }
            Debbuger.Debug(inDateUserData);
        }
    }
    public static void SaveMailData(bool isShowSystemNotice = true)
    {
        ESaveType saveType = ESaveType.Mail;

        if (BackendManager.Instance.IsLocal)
        {
            try
            {
                if (!Directory.Exists(GetPath()))
                {
                    Directory.CreateDirectory(GetPath());

                    Debbuger.Debug("Create New Directory: " + saveType.ToString());
                }
                if (File.Exists(filePathMailData))
                {
                    File.Delete(filePathMailData);
                }
                using (var file = File.Create(filePathMailData))
                {
                    string savedData = NSY.DataManager.Instance.Save(true, saveType);
                    byte[] buffer = Encoding.UTF8.GetBytes(savedData);
                    file.Write(buffer, 0, buffer.Length);

                    Debbuger.Debug("Local Save Data: " + saveType.ToString());
                    if (isShowSystemNotice)
                        DataManager.Instance.AddSystemNotice(saveType.ToString(), true);
                }
            }
            catch (IOException ex)
            {
                Debbuger.ErrorDebug($"Error saving {saveType} data: " + ex.Message);
                DataManager.Instance.AddSystemNotice(saveType.ToString(), false);
            }
        }

        try
        {
            if (BackendManager.Instance.IsLocal == false)
            {
                var param = NSY.DataManager.Instance.ServerSave(saveType);
                if (string.IsNullOrEmpty(inDateMailData))
                {
                    BackendReturnObject returnValue = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.Insert(saveType.ToString(), param));
                    if (returnValue.IsSuccess())
                    {
                        var json = returnValue.GetReturnValuetoJSON();
                        inDateMailData = json["inDate"].ToString();
                        Debbuger.Debug("Server Save Data: " + saveType.ToString());
                        if (isShowSystemNotice)
                            DataManager.Instance.AddSystemNotice(saveType.ToString(), true);
                    }
                }
                else
                {
                    Backend.GameData.UpdateV2(saveType.ToString(), inDateMailData, BackendManager.Instance.OwnerIndate, param, callback =>
                    {
                        BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.UpdateV2(saveType.ToString(), inDateMailData, BackendManager.Instance.OwnerIndate, param), false, callback);
                        if (callback.IsSuccess())
                        {
                            Debbuger.Debug("Server Save Data: " + saveType.ToString());
                            if (isShowSystemNotice)
                                DataManager.Instance.AddSystemNotice(saveType.ToString(), true);
                        }
                        else
                        {
                            Debbuger.Debug($"Error saving {saveType} data: {callback.GetStatusCode()}, {callback.GetMessage()}");
                            DataManager.Instance.AddSystemNotice(saveType.ToString(), false);
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debbuger.ErrorDebug($"Error saving {saveType} data: " + ex.Message);
            DataManager.Instance.AddSystemNotice(saveType.ToString(), false);
        }
    }



    public static void LoadHunterItemData()
    {
        if (BackendManager.Instance.IsLocal)
        {
            if (File.Exists(filePathHunterItemData))
            {
                try
                {
                    using (var file = File.OpenRead(filePathHunterItemData))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);

                        string json = Encoding.UTF8.GetString(bytes);
                        if (json.StartsWith("{") && json.EndsWith("}"))
                        {
                            NSY.DataManager.Instance.Load(json: json, jsonData: null, _isLocal: true, eSaveType: ESaveType.HunterItem);
                            Debbuger.Debug("Read data from file: " + json);
                        }
                        else
                        {
                            Debbuger.ErrorDebug("Invalid data format in the file: " + json);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Debbuger.ErrorDebug("Error loading user data: " + ex.Message);
                }
            }
            else
            {
                //데이터 없을시 필수 데이터 초기화
                NSY.DataManager.Instance.InitLoad(ESaveType.HunterItem);

                Debbuger.Debug("File does not exist: " + filePathHunterItemData);
            }
        }

        if (BackendManager.Instance.IsLocal == false)
        {
            BackendReturnObject data = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.GetMyData("HunterItem", new Where(), 1));

            if (data.IsSuccess())
            {
                var json = data.Rows();
                if (json.Count <= 0)
                {
                    return;
                }
                inDateHunterItemData = json[0]["inDate"]["S"].ToString();
                NSY.DataManager.Instance.Load(json: null, jsonData: json[0], _isLocal: false, eSaveType: ESaveType.HunterItem);
            }
            Debbuger.Debug(inDateHunterItemData);
        }
    }
    public static void SaveHunterItemData(bool isShowSystemNotice = true)
    {
        ESaveType eSaveType = ESaveType.HunterItem;
        if (BackendManager.Instance.IsLocal)
        {
            try
            {
                if (!Directory.Exists(GetPath()))
                {
                    Directory.CreateDirectory(GetPath());

                    Debbuger.Debug("Create New Directory: " + eSaveType.ToString());
                }
                if (File.Exists(filePathHunterItemData))
                {
                    File.Delete(filePathHunterItemData);
                }
                using (var file = File.Create(filePathHunterItemData))
                {
                    string savedData = NSY.DataManager.Instance.Save(true, eSaveType);
                    byte[] buffer = Encoding.UTF8.GetBytes(savedData);
                    file.Write(buffer, 0, buffer.Length);

                    Debbuger.Debug("Local Save Data: " + eSaveType.ToString());
                    if (isShowSystemNotice)
                        DataManager.Instance.AddSystemNotice(eSaveType.ToString(), true);
                }
            }
            catch (IOException ex)
            {
                Debbuger.ErrorDebug($"Error saving {eSaveType} data: " + ex.Message);
                DataManager.Instance.AddSystemNotice(eSaveType.ToString(), false);
            }
        }

        try
        {
            if (BackendManager.Instance.IsLocal == false)
            {
                var param = NSY.DataManager.Instance.ServerSave(eSaveType);
                if (string.IsNullOrEmpty(inDateMailData))
                {
                    BackendReturnObject returnValue = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.Insert(eSaveType.ToString(), param));
                    if (returnValue.IsSuccess())
                    {
                        var json = returnValue.GetReturnValuetoJSON();
                        inDateHunterItemData = json["inDate"].ToString();
                        Debbuger.Debug("Server Save Data: " + eSaveType.ToString());
                        if (isShowSystemNotice)
                            DataManager.Instance.AddSystemNotice(eSaveType.ToString(), true);
                    }
                }
                else
                {
                    Backend.GameData.UpdateV2(eSaveType.ToString(), inDateHunterItemData, BackendManager.Instance.OwnerIndate, param, callback =>
                    {
                        BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.UpdateV2(eSaveType.ToString(), inDateHunterItemData, BackendManager.Instance.OwnerIndate, param), false, callback);
                        if (callback.IsSuccess())
                        {
                            Debbuger.Debug("Server Save Data: " + eSaveType.ToString());
                            if (isShowSystemNotice)
                                DataManager.Instance.AddSystemNotice(eSaveType.ToString(), true);
                        }
                        else
                        {
                            Debbuger.Debug($"Error saving {eSaveType} data: {callback.GetStatusCode()}, {callback.GetMessage()}");
                            DataManager.Instance.AddSystemNotice(eSaveType.ToString(), false);
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debbuger.ErrorDebug($"Error saving {eSaveType} data: " + ex.Message);
            DataManager.Instance.AddSystemNotice(eSaveType.ToString(), false);
        }
    }



    public static void LoadRankData()
    {
        if (BackendManager.Instance.IsLocal)
        {
            if (File.Exists(filePathRankData))
            {
                try
                {
                    using (var file = File.OpenRead(filePathRankData))
                    {
                        byte[] bytes = new byte[file.Length];
                        file.Read(bytes, 0, (int)file.Length);

                        string json = Encoding.UTF8.GetString(bytes);
                        if (json.StartsWith("{") && json.EndsWith("}"))
                        {
                            NSY.DataManager.Instance.Load(json: json, jsonData: null, _isLocal: true, eSaveType: ESaveType.Rank);
                            Debbuger.Debug("Read data from file: " + json);
                        }
                        else
                        {
                            Debbuger.ErrorDebug("Invalid data format in the file: " + json);
                        }
                    }
                }
                catch (IOException ex)
                {
                    Debbuger.ErrorDebug("Error loading user data: " + ex.Message);
                }
            }
            else
            {
                //데이터 없을시 필수 데이터 초기화
                NSY.DataManager.Instance.InitLoad(ESaveType.Rank);

                Debbuger.Debug("File does not exist: " + filePathRankData);
            }
        }

        if (BackendManager.Instance.IsLocal == false)
        {
            BackendReturnObject data = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.GetMyData("Rank", new Where(), 1));

            if (data.IsSuccess())
            {
                var json = data.Rows();
                if (json.Count <= 0)
                {
                    return;
                }
                inDateRankData = json[0]["inDate"]["S"].ToString();
                NSY.DataManager.Instance.Load(json: null, jsonData: json[0], _isLocal: false, eSaveType: ESaveType.Rank);
            }
            Debbuger.Debug(inDateRankData);
        }
    }
    public static void SaveRankData(bool isShowSystemNotice = true)
    {
        ESaveType eSaveType = ESaveType.Rank;
        if (BackendManager.Instance.IsLocal)
        {
            try
            {
                if (!Directory.Exists(GetPath()))
                {
                    Directory.CreateDirectory(GetPath());
                    Debbuger.Debug("Create New Directory: " + eSaveType.ToString());
                }
                if (File.Exists(filePathRankData))
                {
                    File.Delete(filePathRankData);
                }
                using (var file = File.Create(filePathRankData))
                {
                    string savedData = NSY.DataManager.Instance.Save(true, eSaveType);
                    byte[] buffer = Encoding.UTF8.GetBytes(savedData);
                    file.Write(buffer, 0, buffer.Length);

                    Debbuger.Debug("Local Save Data: " + eSaveType.ToString());
                    if (isShowSystemNotice)
                        DataManager.Instance.AddSystemNotice(eSaveType.ToString(), true);
                }
            }
            catch (IOException ex)
            {
                Debbuger.ErrorDebug($"Error saving {eSaveType} data: " + ex.Message);
                DataManager.Instance.AddSystemNotice(eSaveType.ToString(), false);
            }
        }

        try
        {
            if (BackendManager.Instance.IsLocal == false)
            {
                var param = NSY.DataManager.Instance.ServerSave(eSaveType);
                if (string.IsNullOrEmpty(inDateRankData))
                {
                    BackendReturnObject returnValue = BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.Insert(eSaveType.ToString(), param));
                    if (returnValue.IsSuccess())
                    {
                        var json = returnValue.GetReturnValuetoJSON();
                        inDateHunterItemData = json["inDate"].ToString();
                        Debbuger.Debug("Server Save Data: " + eSaveType.ToString());
                        if (isShowSystemNotice)
                            DataManager.Instance.AddSystemNotice(eSaveType.ToString(), true);
                    }
                }
                else
                {
                    Backend.GameData.UpdateV2(eSaveType.ToString(), inDateHunterItemData, BackendManager.Instance.OwnerIndate, param, callback =>
                    {
                        BackendErrorManager.Instance.RetryLogic(() => Backend.GameData.UpdateV2(eSaveType.ToString(), inDateHunterItemData, BackendManager.Instance.OwnerIndate, param), false, callback);
                        if (callback.IsSuccess())
                        {
                            Debbuger.Debug("Server Save Data: " + eSaveType.ToString());
                            if (isShowSystemNotice)
                                DataManager.Instance.AddSystemNotice(eSaveType.ToString(), true);
                        }
                        else
                        {
                            Debbuger.Debug($"Error saving {eSaveType} data: {callback.GetStatusCode()}, {callback.GetMessage()}");
                            DataManager.Instance.AddSystemNotice(eSaveType.ToString(), false);
                        }
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debbuger.ErrorDebug($"Error saving {eSaveType} data: " + ex.Message);
            DataManager.Instance.AddSystemNotice(eSaveType.ToString(), false);
        }
    }

}