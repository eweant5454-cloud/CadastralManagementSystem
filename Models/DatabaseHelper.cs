using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace CadastralManagementSystem.Models
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        // ================== USERS ==================

        // Проверка существования email
        public bool UserExists(string email)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // Добавление нового пользователя (регистрация)
        public void AddUser(UserTemp user)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Users (FullName, Email, Password, Role, RegisteredAt, IsActive) 
                               VALUES (@FullName, @Email, @Password, @Role, @RegisteredAt, @IsActive)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@RegisteredAt", user.RegisteredAt);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Получение пользователя по Email и Паролю (для входа)
        public User GetUser(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT Id, FullName, Email, Password, Role, RegisteredAt, IsActive 
                               FROM Users 
                               WHERE Email = @Email AND Password = @Password AND IsActive = 1";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                FullName = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                Role = reader.GetString(4),
                                RegisteredAt = reader.GetDateTime(5),
                                IsActive = reader.GetBoolean(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Получение всех пользователей (для админа)
        public List<User> GetAllUsers()
        {
            var list = new List<User>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, FullName, Email, Password, Role, RegisteredAt, IsActive FROM Users ORDER BY Id DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            FullName = reader.GetString(1),
                            Email = reader.GetString(2),
                            Password = reader.GetString(3),
                            Role = reader.GetString(4),
                            RegisteredAt = reader.GetDateTime(5),
                            IsActive = reader.GetBoolean(6)
                        });
                    }
                }
            }
            return list;
        }

        // ================== (СПРАВОЧНИК) ==================

        public List<CadastralObjectType> GetAllTypes()
        {
            var list = new List<CadastralObjectType>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT Id, Name, Description FROM CadastralObjectTypes ORDER BY Name";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CadastralObjectType
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.IsDBNull(2) ? null : reader.GetString(2)
                        });
                    }
                }
            }
            return list;
        }

        // ================== (КАДАСТРОВЫЕ ОБЪЕКТЫ) ==================

        public List<CadastralObject> GetAllObjects(string searchTerm = "")
        {
            var list = new List<CadastralObject>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT o.Id, o.CadastralNumber, o.TypeId, t.Name, o.Address, o.Area, o.OwnerName, o.RegistrationDate, o.Status
                               FROM CadastralObjects o
                               JOIN CadastralObjectTypes t ON o.TypeId = t.Id
                               WHERE (@Search = '' OR o.CadastralNumber LIKE @SearchPattern OR o.Address LIKE @SearchPattern OR o.OwnerName LIKE @SearchPattern)
                               ORDER BY o.RegistrationDate DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Search", searchTerm ?? "");
                    cmd.Parameters.AddWithValue("@SearchPattern", "%" + (searchTerm ?? "") + "%");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new CadastralObject
                            {
                                Id = reader.GetInt32(0),
                                CadastralNumber = reader.GetString(1),
                                TypeId = reader.GetInt32(2),
                                TypeName = reader.GetString(3),
                                Address = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Area = reader.IsDBNull(5) ? (double?)null : Convert.ToDouble(reader.GetValue(5)),
                                OwnerName = reader.IsDBNull(6) ? null : reader.GetString(6),
                                RegistrationDate = reader.IsDBNull(7) ? (DateTime?)null : reader.GetDateTime(7),
                                Status = reader.GetString(8)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void AddObject(CadastralObject obj)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO CadastralObjects (CadastralNumber, TypeId, Address, Area, OwnerName, RegistrationDate, Status) 
                               VALUES (@CadastralNumber, @TypeId, @Address, @Area, @OwnerName, @RegistrationDate, @Status)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CadastralNumber", obj.CadastralNumber);
                    cmd.Parameters.AddWithValue("@TypeId", obj.TypeId);
                    cmd.Parameters.AddWithValue("@Address", obj.Address ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Area", obj.Area ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@OwnerName", obj.OwnerName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RegistrationDate", obj.RegistrationDate ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", obj.Status ?? "Active");
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ================== (ЗАЯВКИ) ==================

        public List<Application> GetAllApplications(string statusFilter = "")
        {
            var list = new List<Application>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT a.Id, a.ApplicantId, u.FullName, a.CadastralObjectTypeId, t.Name, 
                                      a.Address, a.Area, a.ApplicantComment, a.Status, a.SubmissionDate, 
                                      a.ProcessedDate, a.EmployeeComment, a.ResultingCadastralNumber
                               FROM Applications a
                               JOIN Users u ON a.ApplicantId = u.Id
                               JOIN CadastralObjectTypes t ON a.CadastralObjectTypeId = t.Id
                               WHERE (@Status = '' OR a.Status = @Status)
                               ORDER BY a.SubmissionDate DESC";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", statusFilter ?? "");
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Application
                            {
                                Id = reader.GetInt32(0),
                                ApplicantId = reader.GetInt32(1),
                                ApplicantName = reader.GetString(2),
                                CadastralObjectTypeId = reader.GetInt32(3),
                                CadastralObjectTypeName = reader.GetString(4),
                                Address = reader.GetString(5),
                                Area = reader.IsDBNull(6) ? (double?)null : Convert.ToDouble(reader.GetValue(6)),
                                ApplicantComment = reader.IsDBNull(7) ? null : reader.GetString(7),
                                Status = reader.GetString(8),
                                SubmissionDate = reader.GetDateTime(9),
                                ProcessedDate = reader.IsDBNull(10) ? (DateTime?)null : reader.GetDateTime(10),
                                EmployeeComment = reader.IsDBNull(11) ? null : reader.GetString(11),
                                ResultingCadastralNumber = reader.IsDBNull(12) ? null : reader.GetString(12)
                            });
                        }
                    }
                }
            }
            return list;
        }

        public void AddApplication(Application app)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Applications (ApplicantId, CadastralObjectTypeId, Address, Area, ApplicantComment, Status, SubmissionDate) 
                               VALUES (@ApplicantId, @CadastralObjectTypeId, @Address, @Area, @ApplicantComment, @Status, @SubmissionDate)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ApplicantId", app.ApplicantId);
                    cmd.Parameters.AddWithValue("@CadastralObjectTypeId", app.CadastralObjectTypeId);
                    cmd.Parameters.AddWithValue("@Address", app.Address);
                    cmd.Parameters.AddWithValue("@Area", app.Area ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApplicantComment", app.ApplicantComment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                    cmd.Parameters.AddWithValue("@SubmissionDate", app.SubmissionDate);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateApplicationStatus(int id, string status, string employeeComment, string cadastralNumber = null)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Applications 
                               SET Status = @Status, ProcessedDate = @ProcessedDate, EmployeeComment = @EmployeeComment, ResultingCadastralNumber = @CadastralNumber 
                               WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@ProcessedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@EmployeeComment", employeeComment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CadastralNumber", cadastralNumber ?? (object)DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}