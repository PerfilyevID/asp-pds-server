using System;

namespace PDS_Server.Humanity
{
    public class Person
    {
        public static Person[] Persons { get; } = new Person[]
        {
            new Person("Игорь", "Перфильев", "perfilev-id@revit-teams.ru"),
            new Person("Ivan", "Petrov", "support@revit-teams.ru"),
            new Person("Mike", "Schneider",  "support@revit-teams.ru"),
            new Person("Olga", "Holm",  "support@revit-teams.ru"),
            new Person("Mika", "Lindberg",  "support@revit-teams.ru")
        };
        public static Person GetRandomPerson() 
        { 
            return Persons[new Random().Next(0, Persons.Length - 1)]; 
        }
        public Person(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
        }
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
    }
}
