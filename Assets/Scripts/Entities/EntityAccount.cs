using System;
using System.Collections.Generic;
using Mogo.Util;
using Mogo.GameData;

namespace Mogo.Game
{
    public class EntityAccount : EntityParent
    {
        #region 实体定义属性
        public LuaTable avatarsInfo
        {
            set
            {
                if (!Mogo.Util.Utils.ParseLuaTable(value, out avatarList))
                {
                    avatarList = new Dictionary<int, AvatarInfo>();
                }
            }
        }
        public Dictionary<int, AvatarInfo> avatarList;

        #endregion

        public EntityAccount()
        {
            entityType = "Account";
            AddListener();
            EventDispatcher.AddEventListener<string, byte, byte>(Events.UIAccountEvent.OnCreateCharacter, CreateCharacter);
            EventDispatcher.AddEventListener<int>(Events.UIAccountEvent.OnDelCharacter, DelCharacter);
            EventDispatcher.AddEventListener<byte>(Events.UIAccountEvent.OnGetRandomName, GetRandomName);
            EventDispatcher.AddEventListener<int>(Events.UIAccountEvent.OnStartGame, StartGame);
        }
        public override void OnEnterWorld()
        {
            if (!SystemConfig.Instance.HasUploadInfo)
            {
                UploadPhoneInfo();
                SystemConfig.Instance.HasUploadInfo = true;
                SystemConfig.SaveConfig();
            }
        }
        public override void OnLeaveWorld()
        {
            EventDispatcher.RemoveEventListener<string, byte, byte>(Events.UIAccountEvent.OnCreateCharacter, CreateCharacter);
            EventDispatcher.RemoveEventListener<int>(Events.UIAccountEvent.OnDelCharacter, DelCharacter);
            EventDispatcher.RemoveEventListener<byte>(Events.UIAccountEvent.OnGetRandomName, GetRandomName);
            EventDispatcher.RemoveEventListener<int>(Events.UIAccountEvent.OnStartGame, StartGame);
            RemoveListener();
        }
        public void CharacterInfoReq() { }
        public void UpdateCharacterList()
        {
            if (avatarList != null && NewLoginUILogicManager.Instance != null)
            {
                var list = new List<ChooseCharacterGridData>();
                foreach (var avatar in avatarList)
                {
                    if (avatar.Value == null)
                    {
                        continue;
                    }
                    list.Add(new ChooseCharacterGridData() 
                    {
                        name = avatar.Value.Name,
                        level = avatar.Value.Level.ToString(),
                        headImg = IconData.dataMap.Get((int)IconOffset.Avatar + avatar.Value.Vocation).path,
                        defaultText = LanguageData.GetContent((int)LangOffset.Character + (int)CharacterCode.CREATE_CHARACTER)
                    });
                }
                NewLoginUILogicManager.Instance.FillChooseCharacterGridData(list);
                NewLoginUILogicManager.Instance.LoadChooseCharacterSceneAfterDelete();
            }
        }
        public AvatarInfo GetAvatarInfo(Int32 index) 
        {
            var order = index + 1;
            if (avatarList != null && avatarList.ContainsKey(order))
            {
                return avatarList[order];
            }
            else
                return null;
        }
        private void CreateCharacter(string name, byte gender, byte vocation) { }
        private void DelCharacter(int gridID) { }
        private void GetRandomName(byte occupation) { }
        private void RandomNameResp(string name) { }
        private void StartGame(int gridID) { }

        private void OnCreateCharacterResp(byte errorID, ulong characterID) { }
        private void OnCharacterInfoResp(LuaTable luaTable) { }
        private void OnDelCharacterResp(byte errorID, ulong characterID) { }
        private void OnLoginResp(byte errorID) { }
        private void OnCheckVersionResp(byte errorID) { }
        public void CheckVersionReq() { }
        private void OnLogoutResp(byte msg_id) { }
        private void OnMultiLogin() { }
        private void UploadPhoneInfo() { }
    }
}
