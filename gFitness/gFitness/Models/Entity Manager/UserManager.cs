using System;
using System.Linq;
using gFitness.Models.DB;
using gFitness.Models.ViewModel;
using System.Collections.Generic;

namespace gFitness.Models.EntityManager
{
    public class UserManager
    {
        gFitnessEntities database = new gFitnessEntities();

        public void Register(Register user)
        {
            Users users = new Users();
            users.Username = user.Username;
            users.Password = user.Password;

            database.Users.Add(users);
            database.SaveChanges();

            int userId = database.Users.Max(u => u.UserId);

            UserDetails userDetails = new UserDetails();
            userDetails.UserId = userId;
            userDetails.Name = user.Name;
            userDetails.Surname = user.Surname;
            userDetails.Email = user.Email;
            userDetails.PhoneNumber = user.PhoneNumber;

            database.UserDetails.Add(userDetails);
            database.SaveChanges();
        }

        public bool IsLoginNameExist(string username)
        {
            return database.Users.Where(o => o.Username.Equals(username)).Any();
        }

        public string GetUserPassword(string username)
        {
                var user = database.Users.Where(o => o.Username.ToLower().Equals(username));
                if (user.Any())
                    return user.FirstOrDefault().Password;
                else
                    return string.Empty;
        }

        public bool IsUserInRole(string username, string roleId)
        {
                Users users = database.Users.Where(o => o.Username.ToLower().Equals(username))?.FirstOrDefault();
                if (users != null)
                {
                    var roles = from q in database.Roles
                                join r in database.LOOKUPRoles on q.LOOKUPRoleID equals r.LOOKUPRoleID
                                where r.RoleId.Equals(roleId) && q.SYSUserID.Equals(users.UserId)
                                select r.RoleId;

                    if (roles != null)
                    {
                        return roles.Any();
                    }
                }

                return false;
            }

        public List<LOOKUPAvailableRole> GetAllRoles()
        {
                var roles = database.LOOKUPRoles.Select(o => new LOOKUPAvailableRole
                {
                    LOOKUPRoleID = o.LOOKUPRoleID,
                    RoleName = o.RoleName
                }).ToList();

                return roles;
        }

        public int GetUserID(string username)
        {
                var user = database.Users.Where(o => o.Username.Equals(username));
                if (user.Any())
                    return user.FirstOrDefault().UserId;
            return 0;
        }
        public List<UserProfileView> GetAllUserProfiles()
        {
            List<UserProfileView> profiles = new List<UserProfileView>();

                UserProfileView UPV;
                var users = database.Users.ToList();

                foreach (Users u in database.Users)
                {
                    UPV = new UserProfileView();
                    UPV.SYSUserID = u.UserId;
                    UPV.LoginName = u.Username;
                    UPV.Password = u.Password;

                    var SUP = database.UserDetails.Find(u.UserId);
                    if (SUP != null)
                    {
                        UPV.FirstName = SUP.Name;
                        UPV.LastName = SUP.Surname;
                    }

                    var SUR = database.Roles.Where(o => o.SYSUserID.Equals(u.UserId));
                    if (SUR.Any())
                    {
                        var userRole = SUR.FirstOrDefault();
                        UPV.LOOKUPRoleID = userRole.LOOKUPRoleID;
                        UPV.RoleName = userRole.LOOKUPRole.RoleName;
                        UPV.IsRoleActive = userRole.IsActive;
                    }

                    profiles.Add(UPV);
                }

            return profiles;
        }

        public UserDataView GetUserDataView(string loginName)
        {
            UserDataView UDV = new UserDataView();
            List<UserProfileView> profiles = GetAllUserProfiles();
            List<LOOKUPAvailableRole> roles = GetAllRoles();

            int? userAssignedRoleID = 0, userID = 0;
            string userGender = string.Empty;

            userID = GetUserID(loginName);

                userAssignedRoleID = database.Roles.Where(o => o.SYSUserID == userID)?.FirstOrDefault().LOOKUPRoleID;
                userGender = database.UserDetails.Where(o => o.SYSUserID == userID)?.FirstOrDefault().Gender;

            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender { Text = "Male", Value = "M" });
            genders.Add(new Gender { Text = "Female", Value = "F" });

            UDV.UserProfile = profiles;
            UDV.UserRoles = new UserRoles { SelectedRoleID = userAssignedRoleID, UserRoleList = roles };
            UDV.UserGender = new UserGender { SelectedGender = userGender, Gender = genders };
            return UDV;
        }

        public void UpdateUserAccount(UserProfileView user)
        {
                using (var dbContextTransaction = database.Database.BeginTransaction())
                {
                    try
                    {

                        Users SU = database.Users.Find(user.SYSUserID);
                        SU.Username = user.LoginName;
                        SU.Password = user.Password;

                    database.SaveChanges();

                        var userProfile = database.UserDetails.Where(o => o.UserId == user.SYSUserID);
                        if (userProfile.Any())
                        {
                            UserDetails SUP = userProfile.FirstOrDefault();
                            SUP.UserId = SU.UserId;
                            SUP.Name = user.FirstName;
                            SUP.Surname = user.LastName;

                        database.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
        }

        public void DeleteUser(int userID)
        {
                using (var dbContextTransaction = database.Database.BeginTransaction())
                {
                    try
                    {

                        var SUR = database.Roles.Where(o => o.SYSUserID == userID);
                        if (SUR.Any())
                        {
                        database.Roles.Remove(SUR.FirstOrDefault());
                        database.SaveChanges();
                        }

                        var SUP = database.UserDetails.Where(o => o.SYSUserID == userID);
                        if (SUP.Any())
                        {
                        database.UserDetails.Remove(SUP.FirstOrDefault());
                        database.SaveChanges();
                        }

                        var SU = database.Users.Where(o => o.SYSUserID == userID);
                        if (SU.Any())
                        {
                        database.Users.Remove(SU.FirstOrDefault());
                        database.SaveChanges();
                        }

                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
            }
        }

        public UserProfileView GetUserProfile(int userID)
        {
            UserProfileView UPV = new UserProfileView();

                var user = database.Users.Find(userID);
                if (user != null)
                {
                    UPV.SYSUserID = user.UserId;
                    UPV.LoginName = user.Username;
                    UPV.Password = user.Password;

                    var SUP = database.UserDetails.Find(userID);
                    if (SUP != null)
                    {
                        UPV.FirstName = SUP.Name;
                        UPV.LastName = SUP.Surname;
                    }

                    var SUR = database.Roles.Find(userID);
                    if (SUR != null)
                    {
                        UPV.LOOKUPRoleID = SUR.LOOKUPRoleID;
                        UPV.RoleName = SUR.LOOKUPRole.RoleName;
                        UPV.IsRoleActive = SUR.IsActive;
                    }
            }
            return UPV;
        }
    }
}