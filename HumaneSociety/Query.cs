using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            throw new NotImplementedException();
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();           
        }

        internal static Animal GetAnimalByID(int id)
        {
            return db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal animal = new Animal();

            for(int i = 0; i < updates.Count; i++)
            {
                if(updates[i] == null)
                {
                    //do nothing 
                } else
                {
                    if(i == 1)
                    {
                        animal.Category.Name = updates[1];
                    } else if(i == 2)
                    {
                        animal.Name = updates[2];
                    } else if (i == 3)
                    {
                        animal.Age = Convert.ToInt32(updates[3]);
                    }
                    else if (i == 4)
                    {
                        animal.Demeanor = updates[4];
                    }
                    else if (i == 5)
                    {
                        animal.KidFriendly = Convert.ToBoolean(updates[5]);
                    }
                    else if (i == 6)
                    {
                        animal.PetFriendly = Convert.ToBoolean(updates[6]);
                    }
                    else if (i == 7)
                    {
                        animal.Weight = Convert.ToInt32(updates[7]);
                    }

                }
            }
        }
        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) 
        {
            //linq query to get list of all animals 
            IQueryable<Animal> results = db.Animals;
            foreach(KeyValuePair<int, string> stuff in updates)
            {
                switch (stuff.Key)
                {
                    case 1:
                        results = results.Where(a => a.Category.Name == stuff.Value);
                        break;
                    case 2:
                        results = results.Where(a => a.Name == stuff.Value);
                        break;
                    case 3:
                        results = results.Where(a => a.Age == Convert.ToInt32(stuff.Value));
                        break;
                    case 4:
                        results = results.Where(a => a.Demeanor == stuff.Value);
                        break;
                    case 5:
                        results = results.Where(a => a.KidFriendly == Convert.ToBoolean(stuff.Value));
                        break;
                    case 6:
                        results = results.Where(a => a.PetFriendly == Convert.ToBoolean(stuff.Value));
                        break;
                    case 7:
                        results = results.Where(a => a.Weight == Convert.ToInt32(stuff.Value));
                        break;
                    default:
                        break;
                }
            }            
            return results;
        }
         
        internal static int GetCategoryId(string categoryName)
        {
            var catName = db.Categories.Where(c => c.Name == categoryName).Select(c => c.CategoryId).FirstOrDefault();
            return catName;
        }
        
        internal static Room GetRoom(int animalId)
        {
            var currentRoom = db.Rooms.Where(r =>r.AnimalId == animalId).FirstOrDefault();
            return currentRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietName = db.DietPlans.Where(d => d.Name == dietPlanName).Select(d => d.DietPlanId).FirstOrDefault();
            return dietName;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            //make adoption that has client id and animal id passed in by those params
            Adoption adoption = new Adoption();

            adoption.AdoptionFee = 75;
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Pending";
            adoption.PaymentCollected = false;

            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();

        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            //all adoptions that have approval or payment not collected 
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            var animalShotHistory = db.AnimalShots.Where(s => s.AnimalId==animal.AnimalId);
            return animalShotHistory;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}