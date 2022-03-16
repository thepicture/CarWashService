using CarWashService.Web.Models.Entities;
using System;
using System.Data.Entity;
using System.Linq;

namespace CarWashService.ImportApp
{
    class Program
    {
        private static readonly string[] branchNamePartsOne = new string[]
        {
            "Супер",
            "ПЛЮС",
            "Ассоциация",
            "ООО",
        };
        private static readonly string[] branchNamePartsTwo = new string[]
        {
            "Филиал автомоек",
            "Автомойка",
            "Помой сам",
            "Чистые машины",
            "Доеду сам",
        };
        private static readonly string[] branchNamePartsThree = new string[]
        {
            "второй отдел",
            "",
            "главный отдел",
            "(дочерняя)"
        };
        private static readonly string[] discountDescriptions = new string[]
        {
            "только сегодня",
            null,
            "для постоянных клиентов",
        };

        private static readonly string[] firstNames = new string[]
        {
            "Иван",
            "Петр",
            "Владимир",
            "Андрей",
            "Кирилл",
            "Макс",
            "Александр",
            "Дмитрий",
            "Николай",
        };

        private static readonly string[] lastNames = new string[]
        {
            "Иванов",
            "Петров",
            "Владимиров",
            "Андреев",
            "Кириллов",
            "Максов",
            "Александров",
            "Дмитриев",
            "Николаев",
        };

        private static readonly string[] patronymics = new string[]
        {
            "Иваненко",
            "Петренкр",
            "Владимиренко",
            "Андреенкр",
            "Кирилленкр",
            "Максенкр",
            "Александренкр",
            "Дмитриенкр",
            "Николаенко",
        };

        private static readonly Random random = new Random();
        static void Main()
        {
            InsertBranchesIntoDb(50);
            InsertServicesIntoDb(50);
            InsertTypesOfServicesIntoDb(1, 3);
            InsertServicesOfBranchesIntoDb(5, 15);
            InsertServicesDiscounts(25);
            InsertUsersIntoDb(25);
            InsertOrdersIntoBranchesIntoDb(1, 10);
            InsertServicesIntoOrdersIntoDb(1, 5);
        }

        private static void InsertServicesIntoOrdersIntoDb(
            int minServicesInOrderCount,
            int maxServicesInOrderCount)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                int minServiceId = context.Service.Min(s => s.Id);
                int maxServiceId = context.Service.Max(s => s.Id);
                foreach (Order order
                    in context.Order
                    .Include(s => s.Service))
                {
                    for (int i = 0;
                        i < random.Next(minServicesInOrderCount, maxServicesInOrderCount + 1);
                        i++)
                    {
                        var serviceToAdd = context.Service
                            .Find(
                            random.Next(minServiceId, maxServiceId));
                        if (order.Service
                            .Any(s => s.Id == serviceToAdd.Id))
                        {
                            continue;
                        }
                        else
                        {
                            order.Service.Add(serviceToAdd);
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        private static void InsertServicesOfBranchesIntoDb(
            int minServicesCount,
            int maxServicesCount)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                int minServiceId = context.Service.Min(s => s.Id);
                int maxServiceId = context.Service.Max(s => s.Id);
                foreach (Branch branch
                    in context.Branch
                    .Include(s => s.Service))
                {
                    for (int i = 0;
                        i < random.Next(minServicesCount, maxServicesCount + 1);
                        i++)
                    {
                        var service = context.Service.Find(
                          random.Next(minServiceId, maxServiceId + 1)
                        );
                        if (branch.Service
                            .Any(s => s.Id == service.Id))
                        {
                            continue;
                        }
                        else
                        {
                            branch.Service.Add(service);
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        private static void InsertOrdersIntoBranchesIntoDb(
            int minOrdersInBranchCount,
            int maxOrdersInBranchCount)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                var clientIds = context.User
                    .Where(u => u.UserTypeId == 2)
                    .ToList()
                    .Select(u => u.Id);
                var adminIds = context.User
                    .Where(u => u.UserTypeId == 1)
                    .ToList()
                    .Select(u => u.Id);
                foreach (Branch branch
                    in context.Branch
                    .Include(s => s.Service))
                {
                    for (int i = 0;
                        i < random.Next(minOrdersInBranchCount, maxOrdersInBranchCount + 1);
                        i++)
                    {
                        Order order = new Order
                        {
                            SellerId = adminIds.ElementAt(
                                random.Next(0,
                                            adminIds.Count())),
                            ClientId = clientIds.ElementAt(
                                random.Next(0,
                                            clientIds.Count())),
                            Date = DateTime.Now.AddDays(random.Next(-365, 365)),
                            IsConfirmed = random.NextDouble() > 0
                        };
                        branch.Order.Add(order);
                    }
                }
                context.SaveChanges();
            }
        }

        private static void InsertUsersIntoDb(int count)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                for (int i = 0; i < count; i++)
                {
                    int minUserType = context.UserType.Min(ut => ut.Id);
                    int maxUserType = context.UserType.Max(ut => ut.Id);
                    User user = new User
                    {
                        FirstName = firstNames[random.Next(0, firstNames.Length)],
                        LastName = lastNames[random.Next(0, lastNames.Length)],
                        Patronymic = patronymics[random.Next(0, patronymics.Length)],
                        UserTypeId = random.Next(minUserType, maxUserType + 1),
                        Login = Guid
                            .NewGuid()
                            .ToString(),
                        Password = Guid
                            .NewGuid()
                            .ToString(),
                        PassportNumber = random
                            .Next(100000, 999999 + 1)
                            .ToString(),
                        PassportSeries = random
                            .Next(1000, 9999 + 1)
                            .ToString(),
                    };
                    context.User.Add(user);
                }
                context.SaveChanges();
            }
        }

        private static readonly TimeSpan[] workFromValues = new TimeSpan[]
        {
            TimeSpan.Parse("6:30"),
            TimeSpan.Parse("6:50"),
            TimeSpan.Parse("7:00"),
            TimeSpan.Parse("8:30"),
            TimeSpan.Parse("9:00"),
        };
        private static readonly TimeSpan[] workToValues = new TimeSpan[]
        {
            TimeSpan.Parse("16:30"),
            TimeSpan.Parse("20:50"),
            TimeSpan.Parse("23:00"),
            TimeSpan.Parse("13:00"),
            TimeSpan.Parse("15:50"),
        };
        private static readonly string[] servicePartsOne = new string[]
        {
            "Мойка",
            "Химчистика",
            "Уборка",
            "Ополаскивание"
        };
        private static readonly string[] servicePartsTwo = new string[]
        {
            "кузова",
            "кресел",
            "бампера",
            "фар"
        };
        private static readonly string[] servicePartsThree = new string[]
        {
            "ПРО",
            "",
            "Плюс",
            "Эконом",
            "Бизнес"
        };
        private static readonly string[] serviceDescriptions = new string[] {
            "Делаем быстро, качественно",
            "Делаем качественно, за день",
            "В выходные услуга не оказывается",
            "Не доступно для некоторых марок",
            "Только для класса A",
            "Оплата по требованию",
            "Доступно и быстро",
            null
        };

        private static void InsertServicesDiscounts(int count)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                for (int i = 0; i < count; i++)
                {
                    int minServiceId = context.Service.Min(s => s.Id);
                    int maxServiceId = context.Service.Max(s => s.Id);
                    int? everyServiceNumber = null;
                    if (random.NextDouble() > 0.7)
                    {
                        everyServiceNumber = random.Next(0, 5 + 1);
                    }
                    ServiceDiscount discount = new ServiceDiscount
                    {
                        ServiceId = random.Next(minServiceId, maxServiceId + 1),
                        DiscountPercent = random.Next(5, 50),
                        EveryServiceNumber = everyServiceNumber,
                        Description = discountDescriptions
                            [random.Next(0, discountDescriptions.Length)]
                    };
                    context.ServiceDiscount.Add(discount);
                }
                context.SaveChanges();
            }
        }

        private static void InsertTypesOfServicesIntoDb(
            int minTypesCount,
            int maxTypesCount)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                int minServiceTypeId = context.ServiceType.Min(st => st.Id);
                int maxServiceTypeId = context.ServiceType.Max(st => st.Id);
                foreach (Service service
                    in context.Service
                    .Include(s => s.ServiceType))
                {
                    for (int i = 0;
                        i < random.Next(minTypesCount, maxTypesCount + 1);
                        i++)
                    {
                        var serviceType = context.ServiceType.Find(
                          random.Next(minServiceTypeId, maxServiceTypeId + 1)
                        );
                        if (service.ServiceType
                            .Any(s => s.Id == serviceType.Id))
                        {
                            continue;
                        }
                        else
                        {
                            service.ServiceType.Add(serviceType);
                        }
                    }
                }
                context.SaveChanges();
            }
        }

        private static void InsertServicesIntoDb(int count)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                for (int i = 0; i < count; i++)
                {
                    int minServiceTypeId = context.ServiceType.Min(st => st.Id);
                    int maxServiceTypeId = context.ServiceType.Max(st => st.Id);
                    string title = $"{servicePartsOne[random.Next(0, servicePartsOne.Length)]} " +
                        $"{servicePartsTwo[random.Next(0, servicePartsTwo.Length)]} " +
                        $"{servicePartsThree[random.Next(0, servicePartsThree.Length)]}";
                    if (title[title.Length - 1] == ' ')
                    {
                        title = title.Substring(0, title.Length - 1);
                    }
                    Service service = new Service
                    {
                        Name = title,
                        Description = serviceDescriptions[random.Next(0, serviceDescriptions.Length)],
                        Price = decimal.Parse(
                            random
                                .Next(1000, 10000)
                                .ToString()
                                ),
                    };
                    context.Service.Add(service);
                }
                context.SaveChanges();
            }
        }

        private static void InsertBranchesIntoDb(int count)
        {
            using (CarWashBaseEntities context = new CarWashBaseEntities())
            {
                for (int i = 0; i < count; i++)
                {
                    int minAddressId = context.Address.Min(a => a.Id);
                    int maxAddressId = context.Address.Max(a => a.Id);
                    string title = $"{branchNamePartsOne[random.Next(0, branchNamePartsOne.Length)]} " +
                        $"{branchNamePartsTwo[random.Next(0, branchNamePartsTwo.Length)]} " +
                        $"{branchNamePartsThree[random.Next(0, branchNamePartsThree.Length)]}";
                    if (title[title.Length - 1] == ' ')
                    {
                        title = title.Substring(0, title.Length - 1);
                    }
                    Branch branch = new Branch
                    {
                        Title = title,
                        AddressId = random.Next(minAddressId, maxAddressId + 1),
                        WorkFrom = workFromValues[random.Next(0, workFromValues.Length)],
                        WorkTo = workToValues[random.Next(0, workToValues.Length)],
                    };
                    context.Branch.Add(branch);
                }
                context.SaveChanges();
            }
        }
    }
}
