namespace TheGrind5_EventManagement.Models
{
    public class UserList
    {
        public List<Users> Users { get; set; }
        //Tạo đọc sửa xóa - CRUD
        public void CreateUser(Users user)
        {
            Users.Add(user);
        }
        public List<Users> GetUsers()
        {
            return Users;
        }
        public Users findUserById(int id)
        {
            return Users.Find(u => u.Id == id);

            /*
                User found = null;
                foreach (var u in Users)
                {
                    if (u.Id == id)
                    {
                        found = u;
                        break;
                    }
                }
             
             */
        }
      
    }
}