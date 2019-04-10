namespace Database.Models
{
    public class Customer
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string email { get; set; }

        public Customer(int id, string name, string email)
        {
            ID = id;
            this.name = name;
            this.email = email;
        }
        
        public Customer()
        {
            
        }
    }
}