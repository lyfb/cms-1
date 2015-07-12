﻿using System.Collections.Generic;
using J6.Cms.Domain.Interface.Value;

namespace J6.Cms.Domain.Interface.User
{
    public interface IUserRepository
    {
        IUser CreateUser(int id,int flag);

        IUser GetUser(int id);

        /// <summary>
        ///   获取应用的角色列表
        /// </summary>
        /// <param name="appId"></param>
        /// <returns></returns>
        IList<RoleValue> GetAppRoles(int appId);

        Credential GetUserCredential(int userId);


        int SaveUser(IUser user);
        int SaveRole(RoleValue role);
        Credential GetCredentialByUserName(string username);
    }
}