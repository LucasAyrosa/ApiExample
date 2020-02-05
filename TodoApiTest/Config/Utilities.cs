using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ToDoAPI.Domain.Models;
using ToDoAPI.Repository.Data;

namespace TodoApiTest.Config
{
    public class Utilities
    {
        internal static void InitializeDbForTests(TodoContext db)
        {
            db.Users.AddRange(GetUsers());
            db.TodoItems.AddRange(getTodoItems());
            db.SaveChanges();
        }

        internal static void ReinitializeDbForTests(TodoContext db)
        {
            db.Users.RemoveRange(db.Users);
            db.TodoItems.RemoveRange(db.TodoItems);
            InitializeDbForTests(db);
        }

        private static List<TodoItem> getTodoItems()
        {
            return new List<TodoItem>()
            {
                new TodoItem{Name = "Passear com o cachorro", IsComplete = true},
                new TodoItem{Name = "Dar comida para os peixes", IsComplete = false},
                new TodoItem{Name = "Dar comida para os pássaros", IsComplete = false},
                new TodoItem{Name = "Fazer levantamento de custo", IsComplete = true},
                new TodoItem{Name = "Comprar presentes", IsComplete = false},
            };
        }

        private static List<IdentityUser> GetUsers()
        {
            return new List<IdentityUser>
            {
                new IdentityUser
                {
                    UserName = "lucas@example.com",
                    NormalizedUserName = "LUCAS@EXAMPLE.COM",
                    Email = "lucas@example.com",
                    NormalizedEmail = "LUCAS@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAECY+f5AWBnLghiWAuINMFXRF5pQreUutlkfXdF+hmpKADELXYfXIsh47EcOtkvDIdA==",
                    SecurityStamp = "22IBRSRMSJEGUI2342JJIVACRKPO5OXU",
                    ConcurrencyStamp = "5b48a89e-057e-4183-99bd-d5a71ab1c56f",
                    PhoneNumber = null,
                    PhoneNumberConfirmed = false,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = true,
                    AccessFailedCount = 0
                },
                new IdentityUser
                {
                    UserName= "ayrosa@email.com",
                    NormalizedUserName= "AYROSA@EMAIL.COM",
                    Email= "ayrosa@email.com",
                    NormalizedEmail= "AYROSA@EMAIL.COM",
                    EmailConfirmed= true,
                    PasswordHash= "AQAAAAEAACcQAAAAEI0oxx7ob36fW+OMAp//GjBSRMHq682xZx5TAXiQNeLWrsXXpEQOqhzsChN/8rKBKA==",
                    SecurityStamp= "EHOVUQT7SBHIA3SW63FIJVNFNHSJFGFS",
                    ConcurrencyStamp= "0125ec9f-bb6a-41c5-a852-9e6b5fa76a7f",
                    PhoneNumber= null,
                    PhoneNumberConfirmed= false,
                    TwoFactorEnabled= false,
                    LockoutEnd= null,
                    LockoutEnabled= true,
                    AccessFailedCount= 0
                }
            };
        }
    }
}