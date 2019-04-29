namespace Database.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string Address { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }

        public Customer(int id, string name, string email, string phone, string address, string gender, int age)
        {
            ID = id;
            this.name = name;
            this.email = email;
            this.phone = phone;
            Address = address;
            Gender = gender;
            Age = age;
        }
        
        public Customer()
        {
            
        }
    }
}