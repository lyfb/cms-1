﻿using System;
using System.Data;
using J6.Cms.Domain.Interface.Value;
using J6.Cms.IDAL;
using J6.DevFw.Data;

namespace J6.Cms.Dal
{
    public class UserDal : DalBase, IUserDal
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public void GetUserCredential(string username, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.User_GetUserCredentialByUserName), new object[,] {{"@userName", username}}),
                func
                );
        }


        public void GetUserById(int id, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.User_GetUserById),
                    new object[,]
                    {
                        {"@id", id}
                    }),
                func
                );
        }

        /// <summary>
        /// 设置用户最后登录时间
        /// </summary>
        public void UpdateUserLastLoginDate(string username, DateTime date)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Member_UpdateUserLastLoginDate),
                    new object[,]
                    {
                        {"@username", username},
                        {"@LastLoginDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", date)}
                    }
                    ));
        }

        public DataTable GetAllUser()
        {
            return base.GetDataSet(new SqlQuery(base.OptimizeSql(DbSql.User_GetAllUsers), null)).Tables[0];
        }

        public bool UserIsExist(string username)
        {
            return base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.User_DectUserNameIsExist),
                    new object[,]
                    {
                        {"@username", username}
                    })
                ) != null;
        }

        public void CreateUser(int siteId, string username, string password, string name, int groupId, bool available)
        {
            DateTime dt = DateTime.Now;
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.User_CreateUser),
                    new object[,]
                    {
                        {"@siteid", siteId},
                        {"@Username", username},
                        {"@Password", password},
                        {"@Name", name},
                        {"@GroupId", groupId},
                        {"@available", available},
                        {"@CreateDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt)},
                        {"@LastLoginDate", String.Format("{0:yyyy-MM-dd HH:mm:ss}", dt)}
                    })
                );
        }


        public void ModifyPassword(string username, string newPassword)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.User_ModifyPassword),
                    new object[,]
                    {
                        {"@Password", newPassword},
                        {"@UserName", username}
                    }));
        }

        public void UpdateUser(string username, int siteid, string name, int groupId, bool available)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.User_UpdateUser),
                    new object[,]
                    {
                        {"@siteid", siteid},
                        {"@Name", name},
                        {"@GroupId", groupId},
                        {"@available", available},
                        {"@username", username}
                    }));
        }

        public bool DeleteUser(string username)
        {
            return base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.User_DeleteUser),
                    new object[,]
                    {
                        {"@username", username}
                    })) == 1;
        }


        public bool CreateOperation(string name, string path, bool available)
        {

            //TODO:优化

            //如果存在则返回false
            object obj = base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_CheckPathExist),
                    new object[,]
                    {
                        {"@Path", path}
                    })
                );
            if (obj != null) return false;

            base.ExecuteScalar(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_CreateOperation),
                    new object[,]
                    {
                        {"@Name", name},
                        {"@Path", path},
                        {"@available", available}
                    })
                );
            return true;
        }

        public void DeleteOperation(int id)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_DeleteOperation),
                    new object[,]
                    {
                        {"@id", id}
                    }
                    ));
        }

        public void GetOperation(int id, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_GetOperation),
                    new object[,]
                    {
                        {"@id", id}
                    }),
                func
                );
        }

        public void GetOperations(DataReaderFunc func)
        {
            base.ExecuteReader(new SqlQuery(base.OptimizeSql(DbSql.Operation_GetOperations), null), func);
        }

        public void UpdateOperation(int id, string name, string path, bool available)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.Operation_UpdateOperation),
                    new object[,]
                    {
                        {"@Name", name},
                        {"@Path", path},
                        {"@available", available},
                        {"@id", id}
                    })
                );
        }


        public DataTable GetPagedOperationList(int pageSize, int currentPageIndex, out int recordCount,
            out int pageCount)
        {
            const string sql1 = "SELECT TOP $[pagesize] * FROM $PREFIX_operation";

            //计算页码
            recordCount =
                int.Parse(
                    base.ExecuteScalar(new SqlQuery(base.OptimizeSql(DbSql.Operation_GetOperationCount), null))
                        .ToString());
            pageCount = recordCount/pageSize;
            if (recordCount%pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;

            int skipCount = pageSize*(currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            string sql = skipCount == 0 && base.DbType == DataBaseType.OLEDB
                ? base.OptimizeSql(sql1)
                : base.OptimizeSql(DbSql.Archive_GetPagedOperations);


            sql = SQLRegex.Replace(sql,
                (match) =>
                {
                    switch (match.Groups[1].Value)
                    {
                        case "pagesize":
                            return pageSize.ToString();
                        case "skipsize":
                            return skipCount.ToString();
                    }
                    return null;
                });

            return base.GetDataSet(
                new SqlQuery(sql, null)
                ).Tables[0];
        }

        public DataTable GetPagedAvailableOperationList(bool available, int pageSize, int currentPageIndex,
            out int recordCount, out int pageCount)
        {
            const string sql1 = "SELECT TOP $[pagesize] * FROM $PREFIX_operation WHERE $[condition]";

            string condition = available ? "available" : "available=false";

            recordCount = int.Parse(
                base.ExecuteScalar(
                    new SqlQuery(
                        String.Format(base.OptimizeSql(DbSql.Operation_GetOperationsCountByAvailable), condition), null)
                    ).ToString());

            //计算页码
            pageCount = recordCount/pageSize;
            if (recordCount%pageSize != 0) pageCount++;

            if (currentPageIndex > pageCount && currentPageIndex != 1) currentPageIndex = pageCount;
            if (currentPageIndex < 1) currentPageIndex = 1;


            int skipCount = pageSize*(currentPageIndex - 1);

            //如果调过记录为0条，且为OLEDB时候，则用sql1
            string sql = skipCount == 0 && base.DbType == DataBaseType.OLEDB
                ? base.OptimizeSql(sql1)
                : base.OptimizeSql(DbSql.Archive_GetPagedOperationsByAvialble);

            sql = SQLRegex.Replace(sql, (match) =>
            {
                switch (match.Groups[1].Value)
                {
                    case "pagesize":
                        return pageSize.ToString();
                    case "skipsize":
                        return (pageSize*(currentPageIndex - 1)).ToString();
                    case "condition":
                        return condition;
                }
                return null;
            });

            return base.GetDataSet(new SqlQuery(sql, null)).Tables[0];
        }

        public DataTable GetUserGroups()
        {
            return base.GetDataSet(new SqlQuery(base.OptimizeSql(DbSql.UserGroup_GetAll), null)).Tables[0];
        }

        public void UpdateUserGroupPermissions(int groupId, string permissions)
        {
            base.ExecuteNonQuery(
                new SqlQuery(base.OptimizeSql(DbSql.UserGroup_UpdatePermissions),
                    new object[,]
                    {
                        {"@Permissions", permissions},
                        {"@GroupId", groupId}
                    })
                );
        }

        public void RenameUserGroup(int groupId, string groupName)
        {
            base.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(DbSql.UserGroup_RenameGroup),
                new object[,]
                {
                    {"@Name", groupName},
                    {"@GroupId", groupId}
                }));
        }


        public DataTable GetMyUserTable(int appId, int userId)
        {
            return this.GetDataSet(new SqlQuery(
                this.OptimizeSql(@"SELECT u.id,name,avatar,user_name,phone,last_login_time,create_time,r.role_flag, 
                c.enabled FROM $PREFIX_user u INNER JOIN  $PREFIX_user_role r ON r.user_id = u.id
                LEFT JOIN  $PREFIX_credential c ON c.user_id=u.id WHERE app_id=@appId AND  r.role_flag <=
                (SELECT role_flag FROM $PREFIX_user_role WHERE user_id=@userId AND app_id=@appId)"),
                new object[,]
                {
                    {"@appId", appId},
                    {"@userId", userId}
                }
                )).Tables[0];
        }


        public int SaveCredential(Credential credential)
        {
            var data = new object[,]
            {
                {"@id", credential.Id},
                {"@userId", credential.UserId},
                {"@userName", credential.UserName},
                {"@password", credential.Password},
                {"@enabled", credential.Password},
            };

            if (credential.Id <= 0)
            {
                int affer = this.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(
                    "INSERT INTO $PREFIX_credential(user_id,user_name,password,enabled)VALUES(@userId,@userName,@password,@enabled)"),
                    data));
                if (affer > 0)
                {
                    credential.Id =
                        int.Parse(
                            this.ExecuteScalar(
                                new SqlQuery(
                                    base.OptimizeSql("SELECT id FROM $PREFIX_credential WHERE user_id=@userId"),
                                    new object[,]
                                    {
                                        {"@userId", credential.UserId},
                                    })).ToString());
                }
            }
            else
            {
                this.ExecuteNonQuery(new SqlQuery(base.OptimizeSql(
                    "INSERT INTO $PREFIX_credential(user_name=@userName,password=@password,enabled=@enabled WHERE user_id=@userId"),
                    new object[,]
                    {
                        {"@userId", credential.UserId},
                    }));
            }
            return credential.Id;
        }

        public void GetUserCredentialById(int userId, DataReaderFunc func)
        {
            base.ExecuteReader(
                new SqlQuery(base.OptimizeSql(DbSql.User_GetUserCredential), new object[,] { { "@userId", userId } }),
                func
                );
        }
    }
}