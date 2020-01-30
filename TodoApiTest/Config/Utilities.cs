using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ToDoAPI.Data.Repository;
using ToDoAPI.Domain.Models;

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
                    // Id = "add901b2-853c-401d-8773-52cd2bd8e638",
                    UserName = "user@example.com",
                    NormalizedUserName = "USER@EXAMPLE.COM",
                    Email = "user@example.com",
                    NormalizedEmail = "USER@EXAMPLE.COM",
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
                }
            };
        }
    }
}